using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Interfaces.Subsystem;
using Unity.TinyCharacterController.UI;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

namespace Unity.TinyCharacterController.Manager
{
    /// <summary>
    /// A system for batch processing <see cref="IndicatorPin"/> components.
    /// Aggregates <see cref="IndicatorPin"/> components and calculates their coordinates in bulk based on information from <see cref="Camera.main"/>.
    /// Coordinates are asynchronously applied to Transforms.
    /// </summary>
    [BurstCompile]
    public sealed class IndicatorPinSystem : SystemBase<IndicatorPin, IndicatorPinSystem>, IPostUpdate
    {
        private Matrix4x4 _preFrameCameraMatrix;
        private NativeList<float3> _positions;
        private NativeList<float2> _uiSize;
        private TransformAccessArray _transforms;
        private JobHandle _handle;

        private static readonly CustomSampler PrepareBufferSampler = CustomSampler.Create("Prepare Buffer");
        private static readonly CustomSampler CalculatePositionSampler = CustomSampler.Create("Calculate Position");
        private static readonly CustomSampler ApplyUiSampler = CustomSampler.Create("Apply Ui");

        private void Awake()
        {
            _transforms = new TransformAccessArray(0);
            _positions = new NativeList<float3>(Allocator.Persistent);
            _uiSize = new NativeList<float2>(Allocator.Persistent);
        }

        private void OnDestroy()
        {
            _handle.Complete();
            UnregisterAllComponents();
            _transforms.Dispose();
            _positions.Dispose();
            _uiSize.Dispose();
        }

        /// <summary>
        /// Update a specific element.
        /// </summary>
        /// <param name="index">Element's ID</param>
        /// <param name="position">New world position of the target</param>
        public void SetPosition(int index, in Vector3 position)
        {
            // Force completion if a job is in progress
            _handle.Complete();

            // Overwrite the element
            _positions[index] = position;
        }

        protected override void OnRegisterComponent(IndicatorPin component, int index)
        {
            // Force completion if a job is in progress
            _handle.Complete();

            // Add the element
            _transforms.Add(component.transform);
            _positions.Add(component.CorrectedPosition);
            _uiSize.Add(component.UiSize);
        }

        protected override void OnUnregisterComponent(IndicatorPin component, int index)
        {
            // Force completion if a job is in progress
            _handle.Complete();

            // Remove the element
            _transforms.RemoveAtSwapBack(index);
            _positions.RemoveAtSwapBack(index);
            _uiSize.RemoveAtSwapBack(index);
        }

        int ISystemBase.Order => 0;

        void IPostUpdate.OnLateUpdate()
        {
            _handle.Complete();

            if (CameraUtility.TryGetMainCamera(out var camera) == false)
                return;

            // Prepare the elements needed for calculations
            var uiPositions = new NativeArray<float3>(Components.Count, Allocator.TempJob);
            var uiVisible = new NativeArray<bool>(Components.Count, Allocator.Temp);
            PrepareProcess(camera, out var screenSize, out var cameraWorldToCameraMatrix);

            // Calculate UI coordinates
            CalculateUi(
                screenSize, camera.projectionMatrix, cameraWorldToCameraMatrix,
                ref uiPositions, ref uiVisible);

            // Update UI visibility and coordinates
            _handle = ApplyUi(cameraWorldToCameraMatrix, uiPositions.AsReadOnly(), uiVisible);

            // Release the buffer
            uiPositions.Dispose(_handle);
            uiVisible.Dispose();
        }

        /// <summary>
        /// Calculate UI coordinates.
        /// </summary>
        /// <param name="screenSize"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="cameraWorldToCameraMatrix"></param>
        /// <param name="uiPositions"></param>
        /// <param name="uiVisible"></param>
        private void CalculateUi(in int2 screenSize,
            in Matrix4x4 projectionMatrix, in Matrix4x4 cameraWorldToCameraMatrix,
            ref NativeArray<float3> uiPositions, ref NativeArray<bool> uiVisible)
        {
            CalculatePositionSampler.Begin();
            // Calculate UI coordinates
            CalculateUiPosition(
                _positions.AsArray(),
                screenSize, projectionMatrix, cameraWorldToCameraMatrix,
                ref uiPositions);
            // Determine UI visibility
            CalculateUiVisible(uiPositions, _uiSize.AsArray(), screenSize, ref uiVisible);
            CalculatePositionSampler.End();
        }

        /// <summary>
        /// Apply UI updates.
        /// </summary>
        /// <param name="cameraWorldToCameraMatrix">Camera matrix</param>
        /// <param name="uiPositions">List of UI coordinates</param>
        /// <param name="uiVisible">List of UI visibility flags</param>
        private JobHandle ApplyUi(
            in Matrix4x4 cameraWorldToCameraMatrix,
            in NativeArray<float3>.ReadOnly uiPositions,
            in NativeArray<bool> uiVisible)
        {
            ApplyUiSampler.Begin();

            // Update UI coordinates
            var handle = new ApplyUiPositionJob
            {
                Positions = uiPositions
            }.Schedule(_transforms);
            JobHandle.ScheduleBatchedJobs();

            var isChangeCameraMatrix = UpdateCameraMatrix(cameraWorldToCameraMatrix);
            for (var i = 0; i < Components.Count; i++)
            {
                var component = Components[i];

                // If the camera matrix has changed or the coordinates have changed, consider the UI as changed
                if (component.IsChangePosition || isChangeCameraMatrix)
                    Components[i].ApplyUi(uiVisible[i]);
            }

            ApplyUiSampler.End();

            return handle;
        }

        /// <summary>
        /// Prepare elements for calculations.
        /// </summary>
        /// <param name="camera">Camera</param>
        /// <param name="screenSize">Screen size</param>
        /// <param name="cameraWorldToCameraMatrix">Camera's world matrix</param>
        private static void PrepareProcess(in Camera camera,
            out int2 screenSize, out Matrix4x4 cameraWorldToCameraMatrix)
        {
            PrepareBufferSampler.Begin();
            screenSize = new int2(Screen.width, Screen.height);
            cameraWorldToCameraMatrix = camera.worldToCameraMatrix;
            PrepareBufferSampler.End();
        }

        /// <summary>
        /// Update the camera matrix.
        /// </summary>
        /// <param name="cameraWorldToCameraMatrix">New matrix</param>
        /// <returns>True if the matrix has changed</returns>
        private bool UpdateCameraMatrix(Matrix4x4 cameraWorldToCameraMatrix)
        {
            var isChangeCameraMatrix = !_preFrameCameraMatrix.Equals(cameraWorldToCameraMatrix);
            _preFrameCameraMatrix = cameraWorldToCameraMatrix;
            return isChangeCameraMatrix;
        }

        /// <summary>
        /// Apply Transform updates.
        /// </summary>
        [BurstCompile]
        private struct ApplyUiPositionJob : IJobParallelForTransform
        {
            public NativeArray<float3>.ReadOnly Positions;

            public void Execute(int index, TransformAccess transform)
            {
                transform.position = Positions[index];
            }
        }

        /// <summary>
        /// Calculate screen coordinates from world coordinates in batch processing.
        /// </summary>
        /// <param name="positions">List of world coordinates</param>
        /// <param name="screenSize">Screen size</param>
        /// <param name="projectionMatrix">Camera's projection matrix</param>
        /// <param name="worldMatrix">Camera's world matrix</param>
        /// <param name="uiPositions">Updated UI coordinates</param>
        [BurstCompile]
        private static void CalculateUiPosition(
            in NativeArray<float3> positions, in int2 screenSize,
            in Matrix4x4 projectionMatrix, in Matrix4x4 worldMatrix,
            ref NativeArray<float3> uiPositions)
        {
            // Prepare the camera matrix
            var matrix = projectionMatrix * worldMatrix;

            for (var i = 0; i < positions.Length; i++)
            {
                // Convert UI coordinates to screen coordinates and apply
                CameraUtility.WorldToScreenPosition(
                    positions[i], matrix, screenSize, out var screenPosition);
                uiPositions[i] = screenPosition;
            }
        }

        /// <summary>
        /// Determine whether UI elements are within the screen boundaries in batch processing.
        /// </summary>
        /// <param name="uiPositions">UI coordinates</param>
        /// <param name="uiSizes">UI sizes</param>
        /// <param name="screenSize">Screen size</param>
        /// <param name="uiVisible">List of UI visibility</param>
        [BurstCompile]
        private static void CalculateUiVisible(
            in NativeArray<float3> uiPositions, in NativeArray<float2> uiSizes,
            in int2 screenSize, ref NativeArray<bool> uiVisible)
        {
            for (var i = 0; i < uiPositions.Length; i++)
            {
                uiVisible[i] = InRange(uiPositions[i], uiSizes[i], screenSize);
            }
        }

        /// <summary>
        /// Check if UI elements are within the screen boundaries.
        /// </summary>
        /// <param name="screenPosition">UI coordinates</param>
        /// <param name="uiSize">UI size</param>
        /// <param name="screenSize">Screen size</param>
        /// <returns></returns>
        [BurstCompile]
        private static bool InRange(in float3 screenPosition, in float2 uiSize, in int2 screenSize)
        {
            return !(screenPosition.z < 0 ||
                     screenPosition.x + uiSize.x < 0 ||
                     screenPosition.x - uiSize.x > screenSize.x ||
                     screenPosition.y + uiSize.y < 0 ||
                     screenPosition.y - uiSize.y > screenSize.y);
        }
    }
}

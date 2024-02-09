using System;
using UnityEngine;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Utility;

namespace TinyCharacterController
{
    /// <summary>
    ///     This component retrieves the distance from objects with the "Player" tag.
    ///     It emits a callback when the player enters a specified range.
    /// 
    ///     It collects a list of objects with this component, calculates the distance to objects with the "Player" tag, and stores it in <see cref="Distance"/>.
    ///     It stores the distance index in <see cref="DistanceIndex"/>. The distance index is determined from <see cref="_ranges"/>, starting with the first element as 0 and increasing by 1, 2, 3, and so on based on the distance of elements.
    ///     When the distance index changes, <see cref="OnChangeDistanceIndex"/> is called.
    ///
    ///     When the component is disabled, no calculations are performed. <see cref="DistanceIndex"/> is -1, and <see cref="Distance"/> is set to <see cref="float.MaxValue"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu(MenuList.MenuCheck + nameof(DistanceFromPlayerCheck))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.DistanceFromPlayerCheck")]
    [DefaultExecutionOrder(Order.Check)]
    [Obsolete]
    public class DistanceFromPlayerCheck : ComponentBase
    {
//         /// <summary>
//         /// Timing for updating the component.
//         /// </summary>
//         private const UpdateTiming Timing = UpdateTiming.LateUpdate;
//
//         /// <summary>
//         ///     Range of distances from the player.
//         ///     If specified as 1, 5, and 8, it means that if the distance from the player is 2.3m, it's 1, and if it's 7m, it's 2.
//         /// </summary>
//         [SerializeField] [ReadOnlyOnRuntime] private float[] _ranges = {1};
//
//         /// <summary>
//         ///     Callback when the range of distances from the player changes.
//         /// </summary>
//         public UnityEvent OnChangeDistanceIndex;
//
//         private DistanceFromPlayerSystem _system;
//
//         /// <summary>
//         ///     Current distance (range) from the player.
//         ///     It returns -1 if the component is not registered or the player does not exist.
//         /// </summary>
//         public int DistanceIndex => IsRegistered ? _system.GetRangeIndex(Index) : -1;
//
//         /// <summary>
//         ///     Distance from the player.
//         ///     It returns float.MaxValue if the component is not registered or the player does not exist.
//         /// </summary>
//         public float Distance => IsRegistered ? _system.GetDistance(Index) : float.MaxValue;
//
//         private static List<GameObject> _objects = new List<GameObject>();
//
//         public static List<GameObject> GetIndexObjects(int distanceIndex)
//         {
//             if (DistanceFromPlayerSystem.IsCreated(Timing) == false)
//                 return _objects;
//
//             DistanceFromPlayerSystem.GetInstance(Timing).GetIndexObjects(distanceIndex, ref _objects);
//
//             return _objects;
//         }
//         
//         private void Awake()
//         {
//             _system = DistanceFromPlayerSystem.GetInstance(Timing);
//         }
//
//         private void OnEnable()
//         {
//             DistanceFromPlayerSystem.Register(this, Timing);
//         }
//
//         private void OnDisable()
//         {
//             DistanceFromPlayerSystem.Unregister(this, Timing);
//         }
//
// #if UNITY_EDITOR
//         private void OnDrawGizmosSelected()
//         {
//             if (_ranges == null)
//                 return;
//             
//             // Represent the ranges that the component recognizes.
//             var center = transform.position;
//             foreach (var range in _ranges)
//                 Gizmos.DrawWireSphere(center, range);
//         }
// #endif
//
//         /// <summary>
//         ///     A class for batch processing distance calculations to the player.
//         /// </summary>
//         [BurstCompile]
//         private class DistanceFromPlayerSystem :
//             SystemBase<DistanceFromPlayerCheck, DistanceFromPlayerSystem>,
//             IEarlyUpdate
//         {
//             private Transform _player;
//             private TransformAccessArray _transforms;
//             private NativeList<NativeArray<float>> _ranges;
//             
//             private NativeList<float> _distances;
//             private NativeList<int> _rangeIndex;
//
//             /// <summary>
//             /// Retrieves a list of object indexes for the specified range index.
//             /// </summary>
//             /// <param name="rangeIndex">List of target indexes to search for</param>
//             /// <param name="objs"></param>
//             public void GetIndexObjects(int rangeIndex, ref List<GameObject> objs)
//             {
//                 objs.Clear();
//                 var array = new NativeList<int>(Allocator.TempJob);
//
//                 // Get the list of target indexes for the range index
//                 GetTargetRangeObjectIndexes(rangeIndex, _rangeIndex.AsArray(), ref array);
//
//                 // Add matching elements
//                 for (var i = 0; i < array.Length; i++)
//                 {
//                     var index = array[i];
//                     objs.Add(Components[index].gameObject);
//                 }
//
//                 // Release the buffer
//                 array.Dispose();
//             }
//
//             /// <summary>
//             /// Get a list of element numbers belonging to RangeIndex.
//             /// </summary>
//             /// <param name="rangeIndex">Number of elements to narrow down</param>
//             /// <param name="rangeIndexes">List of current elements</param>
//             /// <param name="array"></param>
//             [BurstCompile]
//             private static void GetTargetRangeObjectIndexes(int rangeIndex, in NativeArray<int> rangeIndexes,
//                 ref NativeList<int> array)
//             {
//                 for (var i = 0; i < rangeIndexes.Length; i++)
//                 {
//                     if (rangeIndexes[i] == rangeIndex)
//                     {
//                         array.Add(i);
//                     }
//                 }
//             }
//
//             /// <summary>
//             /// Get the distance.
//             /// </summary>
//             /// <param name="index">Component index</param>
//             /// <returns>Distance</returns>
//             public float GetDistance(int index) => _distances[index];
//
//             /// <summary>
//             /// Component number.
//             /// </summary>
//             /// <param name="index">Component index</param>
//             /// <returns>DistanceIndex</returns>
//             public int GetRangeIndex(int index) => _rangeIndex[index];
//
//             // Custom Samplers for performance profiling
//             private static readonly CustomSampler DistanceSampler = CustomSampler.Create("Calculate _distances");
//             private static readonly CustomSampler RangeIndexSampler = CustomSampler.Create("Calculate Range Index");
//             private static readonly CustomSampler InvokeEventSampler = CustomSampler.Create("Invoke Callbacks");
//
//             private void Awake()
//             {
//                 // Initialize the buffers
//                 _transforms = new TransformAccessArray(0);
//                 _distances = new NativeList<float>(Allocator.Persistent);
//                 _rangeIndex = new NativeList<int>(Allocator.Persistent);
//                 _ranges = new NativeList<NativeArray<float>>(Allocator.Persistent);
//             }
//
//             private void OnDestroy()
//             {
//                 // Release the buffers
//                 _rangeIndex.Dispose();
//                 _transforms.Dispose();
//                 _distances.Dispose();
//                 _ranges.Dispose();
//             }
//
//             int ISystemBase.Order => 0;
//
//             void IEarlyUpdate.OnUpdate()
//             {
//                 // Get the player. Abort processing if the player cannot be retrieved.
//                 if (TryGetPlayer(out var player) == false)
//                     return;
//
//                 Profiler.BeginSample("gather positions");
//                 using var positions = GetComponentPositions();
//                 Profiler.EndSample();
//                 using var preRangeIndexes = new NativeArray<int>(_rangeIndex.AsArray(), Allocator.Temp);
//
//                 // Calculate distances to targets
//                 DistanceSampler.Begin();
//                 CalculateDistances(Components.Count, player.position, positions, ref _distances);
//                 DistanceSampler.End();
//
//                 // Get the range indexes of targets
//                 RangeIndexSampler.Begin();
//                 CalculateRangeIndex(Components.Count, _distances.AsArray(), _ranges.AsArray(), ref _rangeIndex);
//                 RangeIndexSampler.End();
//
//                 // Call events if distances have changed
//                 InvokeEventSampler.Begin();
//                 for (var i = 0; i < Components.Count; i++)
//                     if (preRangeIndexes[i] != _rangeIndex[i])
//                         Components[i].OnChangeDistanceIndex?.Invoke();
//                 InvokeEventSampler.End();
//             }
//
//             /// <summary>
//             /// Copy the current positions.
//             /// Avoid using IJobParallelForTransform to prevent errors.
//             /// </summary>
//             /// <returns>List of component positions</returns>
//             private NativeArray<float3> GetComponentPositions()
//             {
//                 var array = new NativeArray<float3>(Components.Count, Allocator.Temp);
//
//                 for (var i = 0; i < array.Length; i++) array[i] = (float3)_transforms[i].position;
//
//                 return array;
//             }
//
//             /// <summary>
//             /// Calculate the distance and range index to the player. Batch processing.
//             /// </summary>
//             /// <param name="count">Buffer size</param>
//             /// <param name="playerPosition">Player's position</param>
//             /// <param name="positions">List of component positions</param>
//             /// <param name="distances">Distances to components</param>
//             [BurstCompile]
//             private static void CalculateDistances(int count,
//                 in float3 playerPosition, in NativeArray<float3> positions, ref NativeList<float> distances)
//             {
//                 for (var i = 0; i < count; i++)
//                 {
//                     var distance = math.distance(positions[i], playerPosition);
//                     distances[i] = distance;
//                 }
//             }
//
//             /// <summary>
//             /// Calculate RangeIndex in batch.
//             /// </summary>
//             /// <param name="count">Number of elements</param>
//             /// <param name="distances">List of distances</param>
//             /// <param name="ranges">List of ranges</param>
//             /// <param name="rangeIndexes">List of calculated results</param>
//             [BurstCompile]
//             private static void CalculateRangeIndex(int count, in NativeArray<float> distances,
//                 in NativeArray<NativeArray<float>> ranges, ref NativeList<int> rangeIndexes)
//             {
//                 for (var index = 0; index < count; index++)
//                 {
//                     GetRangeIndex(distances[index], ranges[index], out var rangeIndex);
//                     rangeIndexes[index] = rangeIndex;
//                 }
//             }
//
//             /// <summary>
//             /// Callback when a component is registered.
//             /// </summary>
//             /// <param name="component">Registered component</param>
//             /// <param name="index">Component index</param>
//             protected override void OnRegisterComponent(DistanceFromPlayerCheck component, int index)
//             {
//                 var trs = component.transform;
//
//                 // Calculate distance to the target if the player exists
//                 CalculateDistanceAndRangeIndex(component, out var distance, out var rangeIndex);
//
//                 // Register the component
//                 _distances.Add(distance);
//                 _rangeIndex.Add(rangeIndex);
//                 _transforms.Add(trs);
//                 _ranges.Add(new NativeArray<float>(component._ranges, Allocator.Persistent));
//             }
//
//             /// <summary>
//             /// Callback when a component is unregistered.
//             /// </summary>
//             /// <param name="component">Unregistered component</param>
//             /// <param name="index">Component index</param>
//             protected override void OnUnregisterComponent(DistanceFromPlayerCheck component, int index)
//             {
//                 // Unregister and organize the buffer
//                 _distances.RemoveAtSwapBack(index);
//                 _rangeIndex.RemoveAtSwapBack(index);
//                 _transforms.RemoveAtSwapBack(index);
//
//                 // Release the buffer in Ranges and remove from _ranges
//                 _ranges[index].Dispose();
//                 _ranges.RemoveAtSwapBack(index);
//             }
//
//             /// <summary>
//             /// Update the distance and range index to the player for a specific component.
//             /// Used to process individually.
//             /// </summary>
//             /// <param name="component">Component for processing</param>
//             /// <param name="distance">Distance to the player</param>
//             /// <param name="rangeIndex">Index of the distance to the player</param>
//             private void CalculateDistanceAndRangeIndex(
//                 DistanceFromPlayerCheck component,
//                 out float distance, out int rangeIndex)
//             {
//                 distance = float.MaxValue;
//                 rangeIndex = -1;
//
//                 if (TryGetPlayer(out var player) == false)
//                     return;
//
//                 // Register initial values
//                 using var ranges = new NativeArray<float>(component._ranges, Allocator.Temp);
//                 distance = Vector3.Distance(component.transform.position, player.position);
//                 GetRangeIndex(distance, ranges, out rangeIndex);
//             }
//
//             /// <summary>
//             /// Get objects with the "Player" tag.
//             /// Use cached values if already retrieved.
//             /// </summary>
//             /// <returns>True if objects are found</returns>
//             private bool TryGetPlayer(out Transform player)
//             {
//                 if (_player == null)
//                     GameObject.FindWithTag("Player")?.TryGetComponent(out _player);
//
//                 player = _player;
//                 return _player != null;
//             }
//
//             /// <summary>
//             /// Get the range.
//             /// </summary>
//             /// <param name="distance">Distance from the target</param>
//             /// <param name="range"></param>
//             /// <param name="rangeIndex"></param>
//             /// <returns>Index on the range</returns>
//             [BurstCompile]
//             private static void GetRangeIndex(float distance, in NativeArray<float> range, out int rangeIndex)
//             {
//                 // Find the index where distance is out of range
//                 for (var i = 0; i < range.Length; i++)
//                 {
//                     if (distance < range[i] == false)
//                         continue;
//
//                     rangeIndex = i;
//                     return;
//                 }
//
//                 // If out of range, return the maximum value
//                 rangeIndex = range.Length;
//             }
//         }
    }
}
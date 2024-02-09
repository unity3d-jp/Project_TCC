using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UTJ
{
    using Inspector;

    // https://docs.unity3d.com/ScriptReference/Editor.html

    [CustomEditor(typeof(SpringBone))]
    [CanEditMultipleObjects]
    public class SpringBoneInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SpringBoneGUIStyles.ReacquireStyles();

            var bone = (SpringBone)target;

            if (GUILayout.Button("基点を選択", SpringBoneGUIStyles.ButtonStyle))
            {
                Selection.objects = targets
                    .Select(item => ((SpringBone)item).pivotNode)
                    .Where(pivotNode => pivotNode != null)
                    .Select(pivotNode => pivotNode.gameObject)
                    .ToArray();
            }

            GUILayout.BeginVertical("box");
            var managerCount = managers.Length;
            for (int managerIndex = 0; managerIndex < managerCount; managerIndex++)
            {
                EditorGUILayout.ObjectField("マネージャー", managers[managerIndex], typeof(SpringManager), true);
            }
            var newEnabled = EditorGUILayout.Toggle("有効", bone.enabled);
            GUILayout.EndVertical();

            if (newEnabled != bone.enabled)
            {
                var targetBones = serializedObject.targetObjects
                    .Select(target => target as SpringBone)
                    .Where(targetBone => targetBone != null);
                if (targetBones.Any())
                {
                    Undo.RecordObjects(targetBones.ToArray(), "SpringBoneの有効状態を変更");
                    foreach (var targetBone in targetBones)
                    {
                        targetBone.enabled = newEnabled;
                    }
                }
            }

            var setCount = propertySets.Length;
            for (int setIndex = 0; setIndex < setCount; setIndex++)
            {
                propertySets[setIndex].Show();
            }
            GUILayout.Space(Spacing);

            serializedObject.ApplyModifiedProperties();

            if (targets.Length == 1)
            {
                RenderAngleLimitVisualization();
            }

            showOriginalInspector = EditorGUILayout.Toggle("標準インスペクター表示", showOriginalInspector);
            GUILayout.Space(Spacing);
            if (showOriginalInspector)
            {
                base.OnInspectorGUI();
            }
        }

        // private

        private const int ButtonHeight = 30;
        private const float Spacing = 16f;

        private SpringManager[] managers;
        private PropertySet[] propertySets;
        private bool showOriginalInspector = false;
        private Inspector3DRenderer renderer;

        private void RenderAngleLimits
        (
            Vector2 origin, 
            float lineLength, 
            Vector2 pivotSpaceVector, 
            AngleLimits angleLimits, 
            Color limitColor
        )
        {
            Inspector3DRenderer.DrawArrow(origin, new Vector2(origin.x, origin.y + lineLength), Color.gray);

            System.Func<float, Vector2> getLimitEndPoint = degrees =>
            {
                var minRadians = Mathf.Deg2Rad * degrees;
                var offset = new Vector2(Mathf.Sin(minRadians), Mathf.Cos(minRadians));
                return origin + lineLength * offset;
            };

            if (!angleLimits.active) { limitColor = Color.grey; }
            var minPosition = getLimitEndPoint(angleLimits.min);
            Inspector3DRenderer.DrawArrow(origin, minPosition, limitColor);
            var maxPosition = getLimitEndPoint(angleLimits.max);
            Inspector3DRenderer.DrawArrow(origin, maxPosition, limitColor);

            if (Application.isPlaying)
            {
                Inspector3DRenderer.DrawArrow(origin, origin + lineLength * pivotSpaceVector, Color.white);
            }
        }

        private void RenderAngleLimitVisualization()
        {
            var bone = (SpringBone)target;
            if (bone.yAngleLimits.active == false
                && bone.zAngleLimits.active == false)
            {
                return;
            }

            if (renderer == null) { renderer = new Inspector3DRenderer(); }

            var useDoubleHeightRect = bone.yAngleLimits.min < -90f
                || bone.yAngleLimits.max > 90f
                || bone.zAngleLimits.min < -90f
                || bone.zAngleLimits.max > 90f;
            
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Y 軸角度制限");
            GUILayout.Label("Z 軸角度制限");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            const float DefaultRectHeight = 100f;
            var rect = GUILayoutUtility.GetRect(
                200f, useDoubleHeightRect ? (2f * DefaultRectHeight) : DefaultRectHeight);
            GUILayout.EndVertical();

            if (Event.current.type != EventType.Repaint) { return; }

            renderer.BeginRender(rect);
            GL.Begin(GL.LINES);

            rect.x = 0f;
            rect.y = 0f;
            Inspector3DRenderer.DrawHollowRect(rect, Color.white);

            const float LineLength = 0.8f * DefaultRectHeight;
            var xOffset = 0.25f * rect.width;
            var yOffset = useDoubleHeightRect ? rect.center.y : (rect.y + 0.1f * DefaultRectHeight);
            var halfWidth = rect.width * 0.5f;
            var pivotTransform = bone.GetPivotTransform();
            var pivotSpaceVector = pivotTransform.InverseTransformVector(
                (bone.CurrentTipPosition - bone.transform.position).normalized);

            var yLimitColor = new Color(0.2f, 1f, 0.2f);
            var yLimitVector = new Vector2(-pivotSpaceVector.y, -pivotSpaceVector.x);
            var yOrigin = new Vector2(xOffset, yOffset);
            RenderAngleLimits(yOrigin, LineLength, yLimitVector, bone.yAngleLimits, yLimitColor);

            var zLimitColor = new Color(0.7f, 0.7f, 1f);
            var zLimitVector = new Vector2(-pivotSpaceVector.z, -pivotSpaceVector.x);
            var zOrigin = new Vector2(xOffset + halfWidth, yOffset);
            RenderAngleLimits(zOrigin, LineLength, zLimitVector, bone.zAngleLimits, zLimitColor);

            GL.End();
            renderer.EndRender();
        }

        private class PropertySet
        {
            public PropertySet(string newTitle, PropertyInfo[] newProperties)
            {
                title = newTitle;
                properties = newProperties;
            }

            public void Initialize(SerializedObject serializedObject)
            {
                var propertyCount = properties.Length;
                for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                {
                    properties[propertyIndex].Initialize(serializedObject);
                }
            }

            public void Show()
            {
                GUILayout.Space(Spacing);
                GUILayout.BeginVertical("box");
                GUILayout.Label(title, SpringBoneGUIStyles.HeaderLabelStyle, GUILayout.Height(ButtonHeight));
                var propertyCount = properties.Length;
                for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                {
                    properties[propertyIndex].Show();
                }
                EditorGUILayout.Space();
                GUILayout.EndVertical();
            }

            private string title;
            private PropertyInfo[] properties;
        }

        private void InitializeData()
        {
            if (managers != null && managers.Length > 0)
            {
                return;
            }

            managers = targets
                .Select(target => target as Component)
                .Where(target => target != null)
                .Select(target => target.GetComponentInParent<SpringManager>())
                .Where(manager => manager != null)
                .Distinct()
                .ToArray();
        }

        private void OnEnable()
        {
            InitializeData();

            var forceProperties = new PropertyInfo[] {
                new PropertyInfo("stiffnessForce", "硬さ"),
                new PropertyInfo("dragForce", "空気抵抗"),
                new PropertyInfo("springForce", "重力"),
                new PropertyInfo("windInfluence", "風の影響値")
            };

            var angleLimitProperties = new PropertyInfo[] {
                new PropertyInfo("pivotNode", "基点"),
                new PropertyInfo("angularStiffness", "回転の硬さ"),
                new AngleLimitPropertyInfo("yAngleLimits", "Y 軸角度制限"),
                new AngleLimitPropertyInfo("zAngleLimits", "Z 軸角度制限")
            };

            var lengthLimitProperties = new PropertyInfo[] {
                new PropertyInfo("lengthLimitTargets", "ターゲット")
            };

            var collisionProperties = new PropertyInfo[] {
                new PropertyInfo("radius", "半径"),
                new PropertyInfo("sphereColliders", "球体"),
                new PropertyInfo("capsuleColliders", "カプセル"),
                new PropertyInfo("panelColliders", "板")
            };

            propertySets = new PropertySet[] {
                new PropertySet("力", forceProperties), 
                new PropertySet("角度制限", angleLimitProperties),
                new PropertySet("距離制限", lengthLimitProperties),
                new PropertySet("当たり判定", collisionProperties),
            };

            foreach (var set in propertySets)
            {
                set.Initialize(serializedObject);
            }
        }

        private static void SelectSpringManager(SpringBone bone)
        {
            var manager = bone.gameObject.GetComponentInParent<SpringManager>();
            if (manager != null)
            {
                Selection.objects = new Object[] { manager.gameObject };
            }
        }

        private static void SelectPivotNode(SpringBone bone)
        {
            var pivotObjects = new List<GameObject>();
            foreach (var gameObject in Selection.gameObjects)
            {
                var springBone = gameObject.GetComponent<SpringBone>();
                if (springBone != null
                    && springBone.pivotNode != null)
                {
                    pivotObjects.Add(springBone.pivotNode.gameObject);
                }
            }
            Selection.objects = pivotObjects.ToArray();
        }
    }
}
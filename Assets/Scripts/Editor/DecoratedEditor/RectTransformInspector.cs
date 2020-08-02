using System;
using UnityEditor;
using UnityEngine;

namespace Bonio.Editor
{
    [CustomEditor(typeof(RectTransform), true)]
    [CanEditMultipleObjects]
    public class RectTransformInspector : TransformResetter
    {
        private const string ResetSizeDelta = "RESET_SIZE_DELTA";
        private static Vector2 _mResetSizeDelta = Vector2.zero;

        private SerializedProperty _position;
        private SerializedProperty _positionZ;
        private SerializedProperty _sizeDelta;
        private SerializedProperty _rotation;
        private SerializedProperty _scale;

        public RectTransformInspector() : base("RectTransformEditor")
        {
        }

        private new void LoadCustomValues()
        {
            base.LoadCustomValues();
            if (EditorPrefs.HasKey(ResetSizeDelta))
                _mResetSizeDelta = JsonUtility.FromJson<Vector3>(EditorPrefs.GetString(ResetSizeDelta));
        }

        private new void DrawCustomValues()
        {
            base.DrawCustomValues();
            if (UnFold)
            {
                EditorGUI.BeginChangeCheck();
                GUI.backgroundColor = new Color(.8f, .8f, 1f, 1f);
                if (GUILayout.Button("Clear Rect Origin", EditorStyles.miniButton))
                {
                    _mResetSizeDelta = Vector2.zero;
                    GUI.FocusControl(null);
                }

                GUI.backgroundColor = Color.white;

                _mResetSizeDelta = EditorGUILayout.Vector2Field("Size Delta", _mResetSizeDelta);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(ResetSizeDelta, EditorJsonUtility.ToJson(_mResetSizeDelta));
                }
            }
        }

        private void OnEnable()
        {
            _position = serializedObject.FindProperty("m_AnchoredPosition");
            _positionZ = serializedObject.FindProperty("m_LocalPosition.z");
            _sizeDelta = serializedObject.FindProperty("m_SizeDelta");
            _rotation = serializedObject.FindProperty("m_LocalRotation");
            _scale = serializedObject.FindProperty("m_LocalScale");

            LoadCustomValues();
        }

        private void OnSceneGUI()
        {
            CallInspectorMethod("OnSceneGUI");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(.8f, .8f, 1f, 1f);

            if (GUILayout.Button("Size Delta", EditorStyles.miniButton))
            {
                _sizeDelta.vector2Value = _mResetSizeDelta;
                serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Position", EditorStyles.miniButtonLeft))
            {
                _position.vector2Value = MResetPosition;
                _positionZ.floatValue = MResetPosition.z;
                serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Rotation", EditorStyles.miniButtonMid))
            {
                _rotation.quaternionValue = Quaternion.Euler(MResetRotation);
                serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

            if (GUILayout.Button("Scale", EditorStyles.miniButtonRight))
            {
                _scale.vector3Value = MResetScale;
                serializedObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();

            DrawCustomValues();
        }
    }
}
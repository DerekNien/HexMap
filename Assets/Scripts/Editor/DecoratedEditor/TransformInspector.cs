using UnityEditor;
using UnityEngine;

namespace Bonio.Editor
{
    [CustomEditor(typeof(Transform))]
    [CanEditMultipleObjects]
    public class TransformInspector : TransformResetter
    {
        private SerializedProperty _position;
        private SerializedProperty _rotation;
        private SerializedProperty _scale;

        public TransformInspector() : base("TransformInspector")
        {
        }

        private void OnEnable()
        {
            _position = serializedObject.FindProperty("m_LocalPosition");
            _rotation = serializedObject.FindProperty("m_LocalRotation");
            _scale = serializedObject.FindProperty("m_LocalScale");

            LoadCustomValues();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(.8f, .8f, 1f, 1f);

            if (GUILayout.Button("Position", EditorStyles.miniButtonLeft))
            {
                _position.vector3Value = MResetPosition;
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
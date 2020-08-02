using UnityEditor;
using UnityEngine;

namespace Bonio.Editor
{
    public abstract class TransformResetter : DecoratorEditor
    {
        private const string ResetPosition = "RESET_POSITION";
        private const string ResetRotation = "RESET_ROTATION";
        private const string ResetScale = "RESET_SCALE";

        protected bool UnFold;

        protected static Vector3 MResetPosition = Vector3.zero;
        protected static Vector3 MResetRotation = Vector3.zero;
        protected static Vector3 MResetScale = Vector3.one;

        public TransformResetter(string name) : base(name)
        {
        }

        protected void LoadCustomValues()
        {
            if (EditorPrefs.HasKey(ResetPosition))
                MResetPosition = JsonUtility.FromJson<Vector3>(EditorPrefs.GetString(ResetPosition));

            if (EditorPrefs.HasKey(ResetRotation))
                MResetRotation = JsonUtility.FromJson<Vector3>(EditorPrefs.GetString(ResetRotation));

            if (EditorPrefs.HasKey(ResetScale))
                MResetScale = JsonUtility.FromJson<Vector3>(EditorPrefs.GetString(ResetScale));
        }

        protected void DrawCustomValues()
        {
            string originLabel;
            if (MResetPosition != Vector3.zero || MResetRotation != Vector3.zero || MResetScale != Vector3.one)
                originLabel = "Set Origin [Custom]";
            else
                originLabel = "Set Origin [Default]";

            UnFold = EditorGUILayout.Foldout(UnFold, originLabel);
            if (UnFold)
            {
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Clear Custom Origin", EditorStyles.miniButton))
                {
                    MResetPosition = Vector3.zero;
                    MResetRotation = Vector3.zero;
                    MResetScale = Vector3.one;
                    GUI.FocusControl(null);
                }

                GUI.backgroundColor = Color.white;

                MResetPosition = EditorGUILayout.Vector3Field("Position", MResetPosition);
                MResetRotation = EditorGUILayout.Vector3Field("Rotation", MResetRotation);
                MResetScale = EditorGUILayout.Vector3Field("Scale", MResetScale);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(ResetPosition, EditorJsonUtility.ToJson(MResetPosition));
                    EditorPrefs.SetString(ResetRotation, EditorJsonUtility.ToJson(MResetRotation));
                    EditorPrefs.SetString(ResetScale, EditorJsonUtility.ToJson(MResetScale));
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bonio.Editor
{
    /// <summary>
    ///     A base class for creating editors that decorate Unity's built-in editor types.
    ///     Credits for this class goes to its author Mr.Lior Tal.
    ///     http://www.tallior.com/extending-unity-inspectors/
    /// </summary>
    public abstract class DecoratorEditor : UnityEditor.Editor
    {
        // empty array for invoking methods using reflection
        private static readonly object[] EmptyArray = new object[0];

        #region Editor Fields

        /// <summary>
        ///     Type object for the internally used (decorated) editor.
        /// </summary>
        private readonly Type _decoratedEditorType;

        /// <summary>
        ///     Type object for the object that is edited by this editor.
        /// </summary>
        private Type _editedObjectType;

        private UnityEditor.Editor _editorInstance;

        #endregion

        private static readonly Dictionary<string, MethodInfo> DecoratedMethods = new Dictionary<string, MethodInfo>();

        private static readonly Assembly EditorAssembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));

        protected UnityEditor.Editor EditorInstance
        {
            get
            {
                if (_editorInstance == null && targets != null && targets.Length > 0)
                    _editorInstance = CreateEditor(targets, _decoratedEditorType);

                if (_editorInstance == null) Debug.LogError("Could not create editor !");

                return _editorInstance;
            }
        }

        public DecoratorEditor(string editorTypeName)
        {
            _decoratedEditorType = EditorAssembly.GetTypes().FirstOrDefault(t => t.Name == editorTypeName);

            Init();

            // Check CustomEditor types.
            var originalEditedType = GetCustomEditorType(_decoratedEditorType);

            if (originalEditedType != _editedObjectType)
                throw new ArgumentException(
                    $"Type {_editedObjectType} does not match the editor {editorTypeName} type {originalEditedType}");
        }

        private Type GetCustomEditorType(Type type)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var attributes = (CustomEditor[])type.GetCustomAttributes(typeof(CustomEditor), true);
            var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

            return field.GetValue(attributes[0]) as Type;
        }

        private void Init()
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var attributes = (CustomEditor[])GetType().GetCustomAttributes(typeof(CustomEditor), true);
            var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

            _editedObjectType = field.GetValue(attributes[0]) as Type;
        }

        private void OnDisable()
        {
            if (_editorInstance != null) DestroyImmediate(_editorInstance);
        }

        /// <summary>
        ///     Delegates a method call with the given name to the decorated editor instance.
        /// </summary>
        protected void CallInspectorMethod(string methodName)
        {
            MethodInfo method;

            // Add MethodInfo to cache
            if (!DecoratedMethods.ContainsKey(methodName))
            {
                var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

                method = _decoratedEditorType.GetMethod(methodName, flags);

                if (method != null)
                    DecoratedMethods[methodName] = method;
                else
                    Debug.LogError($"Could not find method {methodName}");
            }
            else
            {
                method = DecoratedMethods[methodName];
            }

            if (method != null) method.Invoke(EditorInstance, EmptyArray);
        }

        protected override void OnHeaderGUI()
        {
            CallInspectorMethod("OnHeaderGUI");
        }

        public override void OnInspectorGUI()
        {
            EditorInstance.OnInspectorGUI();
        }

        public override void DrawPreview(Rect previewArea)
        {
            EditorInstance.DrawPreview(previewArea);
        }

        public override string GetInfoString()
        {
            return EditorInstance.GetInfoString();
        }

        public override GUIContent GetPreviewTitle()
        {
            return EditorInstance.GetPreviewTitle();
        }

        public override bool HasPreviewGUI()
        {
            return EditorInstance.HasPreviewGUI();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            EditorInstance.OnInteractivePreviewGUI(r, background);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            EditorInstance.OnPreviewGUI(r, background);
        }

        public override void OnPreviewSettings()
        {
            EditorInstance.OnPreviewSettings();
        }

        public override void ReloadPreviewInstances()
        {
            EditorInstance.ReloadPreviewInstances();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            return EditorInstance.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override bool RequiresConstantRepaint()
        {
            return EditorInstance.RequiresConstantRepaint();
        }

        public override bool UseDefaultMargins()
        {
            return EditorInstance.UseDefaultMargins();
        }
    }
}
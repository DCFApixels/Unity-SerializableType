using System;
using UnityEditor;
using UnityEngine;

namespace DCFAPixels
{
    [CustomPropertyDrawer(typeof(SerializableType), true)]
    [CanEditMultipleObjects]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private bool _needApply;
        private string _lastSelectedTypeNmae = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assemblyQualifiedNameProperty = property.FindPropertyRelative("assemblyQualifiedName");

            Rect rect1 = position;
            rect1.width -= 20f;
            Rect rect2 = position;
            rect2.width = 20f;
            rect2.x = rect1.xMax;

            //if (_lastSelectedTypeNmae == null)
            //{
            //    _lastSelectedTypeNmae = assemblyQualifiedNameProperty.stringValue;
            //}

            EditorGUI.BeginProperty(position, label, property);
            if (_needApply)
            {
                assemblyQualifiedNameProperty.stringValue = _lastSelectedTypeNmae;
                _needApply = false;
                _lastSelectedTypeNmae = "";
            }
            EditorGUI.EndProperty();

            if (GUI.Button(rect2, "+"))
                TypeBrowserWindow.Open(GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                    o => {
                        _lastSelectedTypeNmae = o;
                        _needApply = true;
                    }, Type.GetType(assemblyQualifiedNameProperty.stringValue));

            EditorGUI.PropertyField(rect1, assemblyQualifiedNameProperty, label);
        }
    }
}

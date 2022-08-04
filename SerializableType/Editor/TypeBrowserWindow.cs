using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DCFAPixels
{
    public class TypeBrowserWindow : EditorWindow
    {
        [InitializeOnLoad]
        private class TypeCache
        {
            public static readonly List<string> namespaces = new List<string>();
            public static readonly Dictionary<string, Type> nameTypes = new Dictionary<string, Type>();

            static TypeCache()
            {
                Apply();
            }

            private static void Apply()
            {
                nameTypes.Clear();
                namespaces.Clear();
                HashSet<string> namespacesTmp = new HashSet<string>();

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    Type[] types = assembly.GetTypes();

                    foreach (var type in types)
                    {
                        nameTypes[type.FullName] = type;
                        namespacesTmp.Add(type.Namespace);
                    }
                }
                namespaces.AddRange(namespacesTmp);
                namespaces.Sort();
            }
        }


        public static void Open(Vector2 position, Action<string> onOKCallback, Type type)
        {
            var window = CreateInstance<TypeBrowserWindow>();
            window.SetType(type);
            window.onOKCallback = onOKCallback;
            window.ShowAsDropDown(new Rect(position.x, position.y, 1, 1), modalSize);
        }

        private int namespaceIndex = -1;
        private string filter;
        private readonly List<string> filteredNames = new List<string>();
        private int selectedIndex = -1;

        private Action<string> onOKCallback;


        private static readonly Vector2 modalSize = new Vector2(260f, 12f + EditorGUIUtility.singleLineHeight * 4f);

        private void SetType(Type type)
        {
            if (type != null)
            {
                namespaceIndex = TypeCache.namespaces.IndexOf(type.Namespace);
                ApplyFilter(namespaceIndex >= 0 ? TypeCache.namespaces[namespaceIndex] : "", "");
                selectedIndex = filteredNames.IndexOf(type.FullName);
            }
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 80f;
            namespaceIndex = EditorGUILayout.Popup("Namespace", namespaceIndex, TypeCache.namespaces.ToArray());
            filter = EditorGUILayout.TextField("Search filter", filter);
            ApplyFilter(namespaceIndex >= 0 ? TypeCache.namespaces[namespaceIndex] : "", filter);

            if (selectedIndex < 0 || selectedIndex >= filteredNames.Count)
                selectedIndex = -1;

            selectedIndex = EditorGUILayout.Popup("Type", selectedIndex, filteredNames.ToArray());

            GUI.enabled = selectedIndex >= 0 && filteredNames.Count > 0;
            if (GUILayout.Button("OK"))
            {
                onOKCallback?.Invoke(TypeCache.nameTypes[filteredNames[selectedIndex]].AssemblyQualifiedName);
                Close();
            }
        }

        private void ApplyFilter(string @namespace, string filter)
        {
            filteredNames.Clear();
            if (string.IsNullOrEmpty(@namespace) && string.IsNullOrEmpty(filter))
            {
                filteredNames.AddRange(TypeCache.nameTypes.Keys);
                return;
            }
            if (string.IsNullOrEmpty(@namespace))
            {
                filteredNames.AddRange(TypeCache.nameTypes.Keys.Where(o => o.ToLower().Contains(filter.ToLower())));
                return;
            }
            if (string.IsNullOrEmpty(filter))
            {
                filteredNames.AddRange(TypeCache.nameTypes.Where(o => o.Value.Namespace == @namespace).Select(o => o.Key));
                return;
            }
            filteredNames.AddRange(TypeCache.nameTypes.Where(o => 
                o.Value.Namespace == @namespace && o.Key.ToLower().Contains(filter.ToLower())
            ).Select(o => o.Key));
        }
    }
}

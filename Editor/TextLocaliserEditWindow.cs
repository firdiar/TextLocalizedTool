﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace personaltools.textlocalizedtool.editor
{
    public class TextLocaliserEditWindow : EditorWindow
    {
        public string key;
        public string value;
        private string language;

        public static void Open(string key, string value = null, string language = null)
        {
            TextLocaliserEditWindow window = ScriptableObject.CreateInstance<TextLocaliserEditWindow>();
            window.name = "Localisation Edit";

            window.key = key;
            window.value = value;
            window.language = language ?? LocalizationSystem.Language.GetStringValuesAtribute();

            window.ShowUtility();
        }

        public void OnGUI()
        {
            key = EditorGUILayout.TextField("Key : ", key);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Value:", GUILayout.MaxWidth(50));

            EditorStyles.textArea.wordWrap = true;
            value = EditorGUILayout.TextArea
                (value, EditorStyles.textArea, GUILayout.Height(100), GUILayout.Width(400));
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add"))
            {
                if (LocalizationSystem.GetLocalisedValue(key) != null)
                {
                    LocalizationSystem.Replace(key, value, language);
                }
                else
                {
                    LocalizationSystem.Add(key, value, language);
                }
                Close();
            }

            minSize = new Vector2(460, 250);
            maxSize = minSize;
        }
    }

    public class TextLocaliserSearchWindow : EditorWindow
    {
        public static void Open()
        {
            TextLocaliserSearchWindow window = ScriptableObject.CreateInstance<TextLocaliserSearchWindow>();
            window.name = "Localisation Search";

            Vector2 mouse = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect rect = new Rect(mouse.x - 450, mouse.y + 10, 10, 10);
            window.ShowAsDropDown(rect, new Vector2(500, 300));
        }

        public string value;
        public Vector2 scroll;
        public Dictionary<string, string> dictionary;

        public void OnEnable()
        {
            dictionary = LocalizationSystem.GetDictionartForEditor();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("Search: ", EditorStyles.boldLabel);
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();

            GetSearchResults();
        }

        public void GetSearchResults()
        {
            if (value == null) { return; }

            EditorGUILayout.BeginVertical();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (KeyValuePair<string, string> element in dictionary)
            {
                bool isKey = element.Key.ToLower().Contains(value.ToLower());
                bool isValue = element.Value.ToLower().Contains(value.ToLower());
                if (isKey || isValue)
                {
                    EditorGUILayout.BeginHorizontal("box");

                    GUIContent content = EditorGUIUtility.IconContent("winbtn_win_close", "Remove");

                    if (GUILayout.Button(content, GUILayout.MaxWidth(20), GUILayout.MaxHeight(20)))
                    {
                        if (EditorUtility.DisplayDialog("Remove Key " + element.Key + "?",
                            "This will remove the element from localisation, are sure?", "Do it"))
                        {
                            LocalizationSystem.Remove(element.Key);
                            AssetDatabase.Refresh();
                            LocalizationSystem.Init();
                            dictionary = LocalizationSystem.GetDictionartForEditor();
                        }
                    }

                    EditorGUILayout.TextField(element.Key);
                    EditorGUILayout.LabelField(element.Value);
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}

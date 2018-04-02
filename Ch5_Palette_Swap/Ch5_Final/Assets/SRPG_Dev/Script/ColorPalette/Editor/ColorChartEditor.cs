#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ColorChartEditor.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 31 Jan 2018 12:12:47 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEditor;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ColorPalette
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorChart))]
    public class ColorChartEditor : Editor
    {
        public ColorChart chart
        {
            get { return target as ColorChart; }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            if (GUILayout.Button("Load Colors From Texture"))
            {
                string path = EditorUtility.OpenFilePanelWithFilters("Create Chart", "Assets", new string[] { "PNG Image", "png", "JPG Image", "jpg", "GIF Image", "gif" });
                if (!string.IsNullOrEmpty(path) && path.Contains(Application.dataPath))
                {
                    path = path.Replace(Application.dataPath, "Assets");
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    ColorChart.LoadColorsFromTexture(chart, texture);
                }
            }

            if (GUILayout.Button("Open Edit Window"))
            {
                ColorChartEditorWindow window = ColorChartEditorWindow.OpenColorChartEditorWindow();
                window.src = chart;
            }
        }
    }
}
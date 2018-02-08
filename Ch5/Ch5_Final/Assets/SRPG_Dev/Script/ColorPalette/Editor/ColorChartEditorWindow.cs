#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ColorChartEditorWindow.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 31 Jan 2018 12:35:19 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEditor;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ColorPalette
{
    public class ColorChartEditorWindow : EditorWindow
    {
        #region Static Window Method
        private static ColorChartEditorWindow s_Window;
        [MenuItem("Window/SRPG/Color Chart")]
        public static ColorChartEditorWindow OpenColorChartEditorWindow()
        {
            if (s_Window != null)
            {
                s_Window.Focus();
                return s_Window;
            }
            s_Window = EditorWindow.GetWindow<ColorChartEditorWindow>(false, "Color Chart");
            s_Window.Show();
            return s_Window;
        }
        #endregion

        #region Field
        private ColorChart m_SrcChart;
        private ColorChart m_SwapChart;

        private Vector2 m_Scroll = Vector2.zero;
        #endregion

        #region Property
        public ColorChart src
        {
            get { return m_SrcChart; }
            set { m_SrcChart = value; }
        }

        public ColorChart swap
        {
            get { return m_SwapChart; }
            set { m_SwapChart = value; }
        }
        #endregion

        #region Unity Callback
        private void OnEnable()
        {
            Repaint();
        }

        private void OnGUI()
        {
            // 设置src与swap
            src = EditorGUILayout.ObjectField("Source Chart", src, typeof(ColorChart), false) as ColorChart;
            swap = EditorGUILayout.ObjectField("Swap Chart", swap, typeof(ColorChart), false) as ColorChart;
            if (src == null || swap == null)
            {
                EditorGUILayout.HelpBox("Please select source chart and swap chart.", MessageType.Info);
                return;
            }

            // 设置Color的数量
            EditorGUI.BeginChangeCheck();
            int count = Mathf.Max(0, EditorGUILayout.DelayedIntField("Color Count", src.count));
            if (EditorGUI.EndChangeCheck() && src.count != count)
            {
                src.count = count;
                EditorUtility.SetDirty(src);
            }

            if (swap.count != src.count)
            {
                int swapCount = swap.count;
                swap.count = src.count;
                if (swapCount < src.count)
                {
                    ColorChart.CopyColors(src, swapCount, swap, swapCount, src.count - swapCount);
                }
                EditorUtility.SetDirty(swap);
            }

            EditorGUILayout.Space();

            // 设置颜色
            EditorGUI.BeginChangeCheck();
            m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, "box");
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index", GUILayout.MaxWidth(50));
                EditorGUILayout.LabelField("Source Chart", GUILayout.MaxWidth(150));
                EditorGUILayout.LabelField("Swap Chart", GUILayout.MaxWidth(150));
                EditorGUILayout.EndHorizontal();
                for (int i = 0; i < count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.MaxWidth(50));
                    src[i] = EditorGUILayout.ColorField(src[i], GUILayout.MaxWidth(150));
                    swap[i] = EditorGUILayout.ColorField(swap[i], GUILayout.MaxWidth(150));
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(src);
                EditorUtility.SetDirty(swap);
            }
        }

        private void OnDestroy()
        {
            if (s_Window != null)
            {
                s_Window = null;
            }
        }
        #endregion
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MapGraphEditor.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 28 Jan 2018 00:38:28 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace DR.Book.SRPG_Dev.Maps
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MapGraph), true)]
    public class MapGraphEditor : Editor
    {
        #region Create MapGraph GameObject
        [MenuItem("GameObject/SRPG/Map Graph", priority = -1)]
        public static MapGraph CreateMapGraphGameObject()
        {
            GameObject mapGraph = new GameObject("MapGraph", typeof(Grid));
            GameObject tilemap = new GameObject("Tilemap", typeof(Tilemap), typeof(TilemapRenderer));
            tilemap.transform.SetParent(mapGraph.transform, false);
            Selection.activeObject = mapGraph;
            return mapGraph.AddComponent<MapGraph>();
        }
        #endregion

        #region Property
        public MapGraph map
        {
            get { return target as MapGraph; }
        }
        #endregion

        #region Unity Callback
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // 检测地图长宽是否正确，如果不正确就修正
            if (map.mapRect.width < 2 || map.mapRect.height < 2)
            {
                RectInt fix = map.mapRect;
                fix.width = Mathf.Max(map.mapRect.width, 2);
                fix.height = Mathf.Max(map.mapRect.height, 2);
                map.mapRect = fix;
            }
        }

        protected virtual void OnSceneGUI()
        {
            if (!map.m_EditorDrawGizmos)
            {
                return;
            }

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = map.m_EditorCellColor;

            // Scene面板左上角显示信息
            Handles.BeginGUI();
            {
                Rect areaRect = new Rect(50, 50, 200, 200);
                GUILayout.BeginArea(areaRect);
                {
                    // 你的GUILayout代码
                    DrawHorizontalLabel("Object Name:", map.gameObject.name, textStyle);
                    DrawHorizontalLabel("Map Name:", map.mapName, textStyle);
                    DrawHorizontalLabel("Map Size:", map.width + "x" + map.height, textStyle);
                    DrawHorizontalLabel("Cell Size:", map.grid.cellSize.x + "x" + map.grid.cellSize.y, textStyle);
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();

            // 立即刷新Scene面板
            UpdateSceneGUI();
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// 立即刷新Scene面板，这保证了每帧都运行（包括Gizmos）。
        /// 如果在OnSceneGUI或Gizmos里获取鼠标，需要每帧都运行。
        /// </summary>
        protected void UpdateSceneGUI()
        {
            HandleUtility.Repaint();
        }

        protected void DrawHorizontalLabel(string name, string value, GUIStyle style = null, int nameMaxWidth = 80, int valueMaxWdith = 120)
        {
            EditorGUILayout.BeginHorizontal();
            if (style == null)
            {
                EditorGUILayout.LabelField(name, GUILayout.MaxWidth(nameMaxWidth));
                EditorGUILayout.LabelField(value, GUILayout.MaxWidth(valueMaxWdith));
            }
            else
            {
                EditorGUILayout.LabelField(name, style, GUILayout.MaxWidth(nameMaxWidth));
                EditorGUILayout.LabelField(value, style, GUILayout.MaxWidth(valueMaxWdith));
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }
}
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

using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace DR.Book.SRPG_Dev.Maps
{
    using DR.Book.SRPG_Dev.Framework;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MapGraph), true)]
    public class MapGraphEditor : Editor
    {
        #region Create MapGraph GameObject
        [MenuItem("GameObject/SRPG/Map Graph", priority = -1)]
        public static MapGraph CreateMapGraphGameObject()
        {
            MapGraph mapGraph = new GameObject("MapGraph", typeof(Grid)).AddComponent<MapGraph>();

            GameObject tilemapBg = new GameObject("TilemapBackground", typeof(Tilemap), typeof(TilemapRenderer));
            tilemapBg.transform.SetParent(mapGraph.transform, false);

            Tilemap tilemapTerrain = new GameObject("Terrain", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
            tilemapTerrain.transform.SetParent(mapGraph.transform, false);
            mapGraph.terrainTilemap = tilemapTerrain;

            GameObject mapCursorPoolGo = new GameObject("MapCursor");
            mapCursorPoolGo.transform.SetParent(mapGraph.transform, false);

            mapCursorPoolGo.SetActive(false);
            {
                ObjectPool[] pools = GameObject.FindObjectsOfType<ObjectPool>()
                    .Where(p => p.poolName == mapCursorPoolGo.name || (p.poolName == string.Empty && p.name == mapCursorPoolGo.name))
                    .ToArray();
                ObjectPool mapCursorPool = mapCursorPoolGo.AddComponent<ObjectPool>();
                mapCursorPool.poolName = mapCursorPool.name + (pools.Length == 0 ? string.Empty : pools.Length.ToString());
                mapCursorPool.dontDestroy = false;
                mapGraph.mapCursorPool = mapCursorPool;
            }
            mapCursorPoolGo.SetActive(true);

            GameObject mapObjectPoolGo = new GameObject("MapObject");
            mapObjectPoolGo.transform.SetParent(mapGraph.transform, false);
            mapObjectPoolGo.SetActive(false);
            {
                ObjectPool[] pools = GameObject.FindObjectsOfType<ObjectPool>()
                    .Where(p => p.poolName == mapObjectPoolGo.name || (p.poolName == string.Empty && p.name == mapObjectPoolGo.name))
                    .ToArray();
                ObjectPool mapObjectPool = mapObjectPoolGo.AddComponent<ObjectPool>();
                mapObjectPool.poolName = mapObjectPool.name + (pools.Length == 0 ? string.Empty : pools.Length.ToString());
                mapObjectPool.dontDestroy = false;
                mapGraph.mapObjectPool = mapObjectPool;
            }
            mapObjectPoolGo.SetActive(true);

            GameObject tilemapfg = new GameObject("TilemapForeground", typeof(Tilemap), typeof(TilemapRenderer));
            tilemapfg.transform.SetParent(mapGraph.transform, false);

            Selection.activeObject = mapGraph;
            return mapGraph;
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

            EditorGUILayout.Space();

            if (GUILayout.Button("Update MapObject SortingLayer"))
            {
                UpdateMaoObjectSortingLayer();
            }

            if (GUILayout.Button("Clear MapObject"))
            {
                ClearMapObjects();
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

        #region Method
        /// <summary>
        /// 更新地图对象的sortingLayer
        /// </summary>
        private void UpdateMaoObjectSortingLayer()
        {
            if (map.mapObjectPool == null)
            {
                return;
            }

            MapObject[] mapObjects = map.mapObjectPool.gameObject.GetComponentsInChildren<MapObject>(true);

            if (mapObjects != null)
            {
                foreach (MapObject mapObject in mapObjects)
                {
                    // 我们的地图对象不应包含Cursor相关的物体
                    if (mapObject.mapObjectType == MapObjectType.MouseCursor
                        || mapObject.mapObjectType == MapObjectType.Cursor)
                    {
                        continue;
                    }

                    if (mapObject.renderer != null)
                    {   
                        // 更新坐标
                        Vector3 world = mapObject.transform.position;
                        Vector3Int cellPosition = map.grid.WorldToCell(world);
                        mapObject.renderer.sortingOrder = MapObject.CalcSortingOrder(map, cellPosition);
                    }
                }
            }
        }

        /// <summary>
        /// 删除MapObjects
        /// </summary>
        private void ClearMapObjects()
        {
            if (map.mapObjectPool == null)
            {
                return;
            }

            MapObject[] mapObjects = map.mapObjectPool.gameObject.GetComponentsInChildren<MapObject>(true);

            if (mapObjects != null)
            {
                foreach (MapObject mapObject in mapObjects)
                {
                    // 我们的地图对象不应包含Cursor相关的物体
                    if (mapObject.mapObjectType == MapObjectType.MouseCursor
                        || mapObject.mapObjectType == MapObjectType.Cursor)
                    {
                        continue;
                    }

                    Undo.DestroyObjectImmediate(mapObject.gameObject);
                }
            }
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
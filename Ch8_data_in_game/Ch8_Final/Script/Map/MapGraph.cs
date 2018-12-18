#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MapGraph.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 24 Jan 2018 15:53:18 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
using SceneView = UnityEditor.SceneView;
#endif

namespace DR.Book.SRPG_Dev.Maps
{
    using DR.Book.SRPG_Dev.Framework;
    using DR.Book.SRPG_Dev.Maps.FindPath;
    using DR.Book.SRPG_Dev.Models;

    [RequireComponent(typeof(Grid))]
    public class MapGraph : MonoBehaviour
    {

#if UNITY_EDITOR
        #region Gizmos
        [Header("Editor Gizmos")]
        public bool m_EditorDrawGizmos = true;
        public Color m_EditorBorderColor = Color.white;
        public Color m_EditorCellColor = Color.green;
        public Color m_EditorErrorColor = Color.red;

        private void OnDrawGizmos()
        {
            if (m_EditorDrawGizmos)
            {
                EditorDrawBorderGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_EditorDrawGizmos)
            {
                EditorDrawCellGizmos();
            }
        }

        /// <summary>
        /// 绘制Border的Gizmos
        /// </summary>
        protected void EditorDrawBorderGizmos()
        {
            Color old = Gizmos.color;

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = m_EditorBorderColor;

            // 获取边框左下角与右上角的世界坐标
            Vector3 leftDown = grid.GetCellCenterWorld(leftDownPosition) - halfCellSize;
            Vector3 rightUp = grid.GetCellCenterWorld(rightUpPosition) + halfCellSize;

            // 绘制左下角Cell与右上角Cell的Position
            Handles.Label(leftDown, (new Vector2Int(leftDownPosition.x, leftDownPosition.y)).ToString(), textStyle);
            Handles.Label(rightUp, (new Vector2Int(rightUpPosition.x, rightUpPosition.y)).ToString(), textStyle);

            if (mapRect.width > 0 && mapRect.height > 0)
            {
                Gizmos.color = m_EditorBorderColor;

                // 边框的长与宽
                Vector3 size = Vector3.Scale(new Vector3(width, height), grid.cellSize);

                // 边框的中心坐标
                Vector3 center = leftDown + size / 2f;

                // 绘制边框
                Gizmos.DrawWireCube(center, size);
            }

            Gizmos.color = old;
        }

        /// <summary>
        /// 绘制Cell的Gizmos
        /// </summary>
        protected void EditorDrawCellGizmos()
        {
            // 用于获取鼠标位置
            Event e = Event.current;
            if (e.type != EventType.Repaint)
            {
                return;
            }

            // 获取当前操作Scene面板
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null)
            {
                return;
            }

            Color old = Gizmos.color;

            /// 获取鼠标世界坐标：
            /// Event是从左上角(Left Up)开始，
            /// 而Camera是从左下角(Left Down)，
            /// 需要转换才能使用Camera的ScreenToWorldPoint方法。
            Vector2 screenPosition = new Vector2(e.mousePosition.x, sceneView.camera.pixelHeight - e.mousePosition.y);
            Vector2 worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);

            // 当前鼠标所在Cell的Position
            Vector3Int cellPostion = grid.WorldToCell(worldPosition);
            // 当前鼠标所在Cell的Center坐标
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPostion);

            /// 绘制当前鼠标下的Cell边框与Position
            /// 如果包含Cell，正常绘制
            /// 如果不包含Cell，改变颜色，并多绘制一个叉
            if (Contains(cellPostion))
            {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = m_EditorCellColor;
                Gizmos.color = m_EditorCellColor;

                Handles.Label(cellCenter - halfCellSize, (new Vector2Int(cellPostion.x, cellPostion.y)).ToString(), textStyle);
                Gizmos.DrawWireCube(cellCenter, grid.cellSize);
            }
            else
            {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = m_EditorErrorColor;
                Gizmos.color = m_EditorErrorColor;

                Handles.Label(cellCenter - halfCellSize, (new Vector2Int(cellPostion.x, cellPostion.y)).ToString(), textStyle);
                Gizmos.DrawWireCube(cellCenter, grid.cellSize);

                // 绘制Cell对角线
                Vector3 from = cellCenter - halfCellSize;
                Vector3 to = cellCenter + halfCellSize;
                Gizmos.DrawLine(from, to);
                float tmpX = from.x;
                from.x = to.x;
                to.x = tmpX;
                Gizmos.DrawLine(from, to);
            }

            Gizmos.color = old;
        }
        #endregion
#endif

        #region Map Setting Field
        [Header("Map Setting")]
        [SerializeField]
        private string m_MapName;
        [SerializeField]
        private RectInt m_MapRect = new RectInt(0, 0, 10, 10);
        [SerializeField]
        private Tilemap m_TerrainTilemap;

        /// <summary>
        /// 地图每个格子的信息
        /// </summary>
        private Dictionary<Vector3Int, CellData> m_DataDict = new Dictionary<Vector3Int, CellData>();

        private Grid m_Grid;
        #endregion

        #region Map Setting Property
        /// <summary>
        /// 地图的名称
        /// </summary>
        public string mapName
        {
            get { return m_MapName; }
            set { m_MapName = value; }
        }

        /// <summary>
        /// 地图的矩形框
        /// </summary>
        public RectInt mapRect
        {
            get { return m_MapRect; }
            set { m_MapRect = value; }
        }

        /// <summary>
        /// 地图左下角Position
        /// </summary>
        public Vector3Int leftDownPosition
        {
            get { return new Vector3Int(m_MapRect.xMin, m_MapRect.yMin, 0); }
        }

        /// <summary>
        /// 地图右上角Position
        /// </summary>
        public Vector3Int rightUpPosition
        {
            get { return new Vector3Int(m_MapRect.xMax - 1, m_MapRect.yMax - 1, 0); }
        }

        /// <summary>
        /// 地图宽
        /// </summary>
        public int width
        {
            get { return m_MapRect.width; }
        }

        /// <summary>
        /// 地图高
        /// </summary>
        public int height
        {
            get { return m_MapRect.height; }
        }

        /// <summary>
        /// 计算的Tilemap
        /// </summary>
        public Tilemap terrainTilemap
        {
            get { return m_TerrainTilemap; }
            set { m_TerrainTilemap = value; }
        }

        /// <summary>
        /// Grid组件
        /// </summary>
        public Grid grid
        {
            get
            {
                if (m_Grid == null)
                {
                    m_Grid = GetComponent<Grid>();
                }
                return m_Grid;
            }
        }

        /// <summary>
        /// 地图每个cell尺寸的一半
        /// </summary>
        public Vector3 halfCellSize
        {
            get { return grid.cellSize / 2f; }
        }
        #endregion

        #region Map Object Field
        [Header("Map Object Setting")]
        [SerializeField]
        private ObjectPool m_MapObjectPool;
        [SerializeField]
        private ObjectPool m_MapCursorPool;
        [SerializeField]
        private MapMouseCursor m_MouseCursorPrefab;
        [SerializeField]
        private MapCursor m_CursorPrefab;

        /// <summary>
        /// 生成的MapMouseCursor
        /// </summary>
        private MapMouseCursor m_MouseCursor;

        /// <summary>
        /// 运行时，MapCursor的预制体
        /// </summary>
        private MapCursor m_RuntimeCursorPrefab;

        ///// <summary>
        ///// 移动范围光标集合
        ///// </summary>
        //private List<MapCursor> m_MapMoveCursors = new List<MapCursor>();

        ///// <summary>
        ///// 攻击范围光标集合
        ///// </summary>
        //private List<MapCursor> m_MapAttackCursors = new List<MapCursor>();

        /// <summary>
        /// 光标集合
        /// </summary>
        private HashSet<MapCursor> m_Cursors = new HashSet<MapCursor>();

        /// <summary>
        /// 职业集合
        /// </summary>
        private List<MapClass> m_Classes = new List<MapClass>();
        #endregion

        #region Map Object Property
        /// <summary>
        /// MapObject父对象
        /// </summary>
        public ObjectPool mapObjectPool
        {
            get { return m_MapObjectPool; }
            set { m_MapObjectPool = value; }
        }

        /// <summary>
        /// MapCursor父对象
        /// </summary>
        public ObjectPool mapCursorPool
        {
            get { return m_MapCursorPool; }
            set { m_MapCursorPool = value; }
        }

        /// <summary>
        /// 默认mouse cursor的prefab
        /// </summary>
        public MapMouseCursor mouseCursorPrefab
        {
            get { return m_MouseCursorPrefab; }
            set { m_MouseCursorPrefab = value; }
        }

        /// <summary>
        /// 默认cursor的prefab
        /// </summary>
        public MapCursor cursorPrefab
        {
            get { return m_CursorPrefab; }
            set { m_CursorPrefab = value; }
        }

        /// <summary>
        /// 用户光标
        /// </summary>
        public MapMouseCursor mouseCursor
        {
            get
            {
                // 只有在测试时，才会都使用默认prefab创建
                // 正式游戏，这里不会为null
                // 将在初始化地图时创建用户光标
                // 如果游戏无法初始化光标，则需要检查代码是否正确
                if (m_MouseCursor == null)
                {
                    m_MouseCursor = CreateMapObject(mouseCursorPrefab) as MapMouseCursor;
                }
                return m_MouseCursor;
            }
            private set
            {
                m_MouseCursor = value;
            }
        }


        /// <summary>
        /// 运行时，MapCursor的预制体
        /// </summary>
        public MapCursor runtimeCursorPrefab
        {
            get
            {
                // 只有在测试时，才会都使用默认prefab
                // 正式游戏，这里不会为null
                // 将在初始化地图时会加载预制体
                // 如果游戏无法加载预制体，则需要检查代码是否正确
                if (m_RuntimeCursorPrefab == null)
                {
                    m_RuntimeCursorPrefab = cursorPrefab;
                }
                return m_RuntimeCursorPrefab;
            }
            private set
            {
                m_RuntimeCursorPrefab = value;
            }
        }
        #endregion

        #region Path Finding Field
        /// <summary>
        /// 寻路核心
        /// </summary>
        private PathFinding m_SearchPath;

        [Header("Path Finding")]
        [SerializeField]
        private FindRange m_FindAttackRange;

        [SerializeField]
        private FindRange m_FindMoveRange;

        [SerializeField]
        private FindRange m_FindPathDirect;

        private CellPositionEqualityComparer m_CellPositionEqualityComparer = new CellPositionEqualityComparer();
        #endregion

        #region Path Finding Property
        /// <summary>
        /// 寻路核心
        /// </summary>
        public PathFinding searchPath
        {
            get { return m_SearchPath; }
        }

        /// <summary>
        /// 寻找攻击范围
        /// </summary>
        public FindRange findAttackRange
        {
            get { return m_FindAttackRange; }
            set { m_FindAttackRange = value; }
        }

        /// <summary>
        /// 寻找移动范围
        /// </summary>
        public FindRange findMoveRange
        {
            get { return m_FindMoveRange; }
            set { m_FindMoveRange = value; }
        }

        /// <summary>
        /// 无视移动力，直接寻找路径
        /// </summary>
        public FindRange findPathDirect
        {
            get { return m_FindPathDirect; }
            set { m_FindPathDirect = value; }
        }

        /// <summary>
        /// 判断两个Cell的Position是否相等
        /// </summary>
        public CellPositionEqualityComparer cellPositionEqualityComparer
        {
            get
            {
                if (m_CellPositionEqualityComparer == null)
                {
                    m_CellPositionEqualityComparer = new CellPositionEqualityComparer();
                }
                return m_CellPositionEqualityComparer;
            }
        }
        #endregion

        #region Unity Callback
        private void OnApplicationQuit()
        {
            if (mapObjectPool != null && mapObjectPool.gameObject != null)
            {
                mapObjectPool.DespawnAll();
                GameObject.DestroyImmediate(mapObjectPool.gameObject);
            }
        }

        private void OnDestroy()
        {
            ClearCellDatas();
        }
        #endregion

        #region Init Map Method
        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <returns></returns>
        public void InitMap(bool reinit = false)
        {
            if (!reinit && m_DataDict.Count > 0)
            {
                return;
            }

            InitCellDatas();

            InitPathfinding();

            InitMapObjectsInMap();

            // TODO other init
        }

        private void InitCellDatas()
        {
            ClearCellDatas();
            CreateCellDatas();
        }

        /// <summary>
        /// 删除已有的CellData
        /// </summary>
        public void ClearCellDatas()
        {
            if (m_DataDict.Count > 0)
            {
                if (mapObjectPool != null)
                {
                    mapObjectPool.DespawnAll();
                }
                foreach (CellData cell in m_DataDict.Values)
                {
                    cell.Dispose();
                }
                m_DataDict.Clear();
            }
        }

        /// <summary>
        /// 建立CellData
        /// </summary>
        private void CreateCellDatas()
        {
            if (m_DataDict.Count != 0)
            {
                return;
            }

            for (int y = mapRect.yMin; y < mapRect.yMax; y++)
            {
                for (int x = mapRect.xMin; x < mapRect.xMax; x++)
                {
                    CellData cell = new CellData(x, y);
                    m_DataDict.Add(cell.position, cell);
                }
            }

            foreach (CellData cell in m_DataDict.Values)
            {
                cell.hasTile = GetTile(cell.position) != null;
                FindAdjacents(cell);
            }
        }

        /// <summary>
        /// 添加邻居
        /// </summary>
        /// <param name="cell"></param>
        private void FindAdjacents(CellData cell)
        {
            cell.adjacents.Clear();
            Vector3Int position = cell.position;
            Vector3Int pos;

            // up
            pos = new Vector3Int(position.x, position.y + 1, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }

            // right
            pos = new Vector3Int(position.x + 1, position.y, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }

            // down
            pos = new Vector3Int(position.x, position.y - 1, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }

            // left
            pos = new Vector3Int(position.x - 1, position.y, position.z);
            if (Contains(pos))
            {
                cell.adjacents.Add(m_DataDict[pos]);
            }
        }

        /// <summary>
        /// 初始化寻路
        /// </summary>
        private void InitPathfinding()
        {
            if (m_SearchPath == null)
            {
                m_SearchPath = new PathFinding(this);
            }

            if (Application.isPlaying)
            {
                if (m_FindAttackRange == null)
                {
                    m_FindAttackRange = ScriptableObject.CreateInstance<FindRange>();
                }

                if (m_FindMoveRange == null)
                {
                    m_FindMoveRange = ScriptableObject.CreateInstance<FindMoveRange>();
                }

                if (m_FindPathDirect == null)
                {
                    m_FindPathDirect = ScriptableObject.CreateInstance<FindPathDirect>();
                }
            }

            if (m_CellPositionEqualityComparer == null)
            {
                m_CellPositionEqualityComparer = new CellPositionEqualityComparer();
            }
        }

        /// <summary>
        /// 初始化加载地图时地图对象
        /// </summary>
        private void InitMapObjectsInMap()
        {
            if (mapObjectPool == null)
            {
                Debug.LogError("MapGraph -> MapObject Pool is null.");
                return;
            }

            MapObject[] mapObjects = mapObjectPool.gameObject.GetComponentsInChildren<MapObject>();
            if (mapObjects != null)
            {
                foreach (MapObject mapObject in mapObjects)
                {
                    // 我们的地图对象不应包含Cursor相关的物体
                    if (mapObject.mapObjectType == MapObjectType.MouseCursor
                        || mapObject.mapObjectType == MapObjectType.Cursor)
                    {
                        GameObject.Destroy(mapObject.gameObject);
                        continue;
                    }

                    // 初始化
                    mapObject.InitMapObject(this);

                    // 更新坐标
                    Vector3 world = mapObject.transform.position;
                    Vector3Int cellPosition = grid.WorldToCell(world);
                    mapObject.cellPosition = cellPosition;

                    // 设置CellData
                    CellData cellData = GetCellData(cellPosition);
                    if (cellData != null)
                    {
                        if (cellData.hasMapObject)
                        {
                            Debug.LogErrorFormat("MapObject in Cell {0} already exists.", cellPosition.ToString());
                            continue;
                        }
                        cellData.mapObject = mapObject;
                    }

                    // 如果是Class
                    // 可选项（可忽略）：
                    //      请将Prefab加入到mapObjectPool的PrePrefabs中防止动态重复读取Prefab
                    //      为Prefab添加RuntimePrePoolObject组件：
                    //          如果此Prefab也用于动态读取，一定将组件enable设置为false；
                    //          将 Prefab Name 设置成对应的Prefab名称；
                    //          删除时会Despawn回池子。
                    //      这些不是必须的，一些需要这样干的情况：
                    //          绘制此Prefab的实例（比如某些杂兵）数量非常多（一般 > 20），
                    //          且在消灭之后会由于事件触发再次生成大量此Prefab的实例。
                    if (mapObject.mapObjectType == MapObjectType.Class)
                    {
                        RuntimePrePoolObject runtime = mapObject.GetComponent<RuntimePrePoolObject>();
                        if (runtime != null && !runtime.enabled)
                        {
                            runtime.m_PoolName = mapObjectPool.poolName;
                            runtime.enabled = true;
                        }
                        MapClass cls = mapObject as MapClass;
                        cls.Load(0, RoleType.Following); // TODO Load Data
                        if (!m_Classes.Contains(cls))
                        {
                            m_Classes.Add(cls);
                        } 
                    }

                    mapObject.gameObject.name += mapObject.cellPosition.ToString();
                }
            }
        }
        #endregion

        #region Map Object Method
        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public MapObject CreateMapObject(MapObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogErrorFormat("MapGraph -> CreateMapObject ERROR!! {0}",
                    "Prefab is null.");
                return null;
            }

            MapObjectType type = prefab.mapObjectType;

            // 用户光标在整个地图中只能有且只有一个
            if (type == MapObjectType.MouseCursor && m_MouseCursor != null)
            {
                ObjectPool.DespawnUnsafe(m_MouseCursor.gameObject, true);
                m_MouseCursor = null;
            }

            // 实例化 map object
            GameObject instance;
            if (type == MapObjectType.Cursor || type == MapObjectType.MouseCursor)
            {
                instance = mapCursorPool.Spawn(prefab.gameObject);
            }
            else
            {
                instance = mapObjectPool.Spawn(prefab.gameObject);
            }

            MapObject mapObject = instance.GetComponent<MapObject>();

            mapObject.InitMapObject(this);

            if (type == MapObjectType.MouseCursor)
            {
                m_MouseCursor = mapObject as MapMouseCursor;
            }

            return mapObject;
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public MapObject CreateMapObject(string prefabName)
        {
            MapObject prefab = LoadMapObjectPrefab(prefabName);
            return CreateMapObject(prefab);
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cellPosition"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public MapObject CreateMapObject(MapObject prefab, Vector3Int cellPosition)
        {
            MapObject mapObject = CreateMapObject(prefab);
            if (mapObject != null)
            {
                mapObject.UpdatePosition(cellPosition);
            }
            return mapObject;
        }

        /// <summary>
        /// 创建地图对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cellPosition"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public MapObject CreateMapObject(string prefabName, Vector3Int cellPosition)
        {
            MapObject prefab = LoadMapObjectPrefab(prefabName);
            MapObject mapObject = CreateMapObject(prefab);
            if (mapObject != null)
            {
                mapObject.UpdatePosition(cellPosition);
            }
            return mapObject;
        }

        /// <summary>
        /// 读取Prefab
        /// </summary>
        /// <param name="type"></param>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        private MapObject LoadMapObjectPrefab(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                Debug.LogError("MapGraph -> LoadMapObjectPrefab ERROR!!" +
                    " Prefab name is null or empty.");
                return null;
            }

            // TODO ResourcesManager 读取prefab
            MapObject prefab = Resources.Load<MapObject>(prefabName);
            return prefab;
        }

        /// <summary>
        /// 显示cursor
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="type"></param>
        public void ShowRangeCursors(IEnumerable<CellData> cells, MapCursor.CursorType type)
        {
            if (type == MapCursor.CursorType.Mouse)
            {
                return;
            }

            foreach (CellData cell in cells)
            {
                MapCursor cursor = CreateMapObject(runtimeCursorPrefab, cell.position) as MapCursor;
                if (cursor != null)
                {
                    //cursor.name = string.Format(
                    //    "{0} Cursor {1}",
                    //    type.ToString(),
                    //    cell.position.ToString());
                    cursor.cursorType = type;
                    if (type == MapCursor.CursorType.Move)
                    {
                        //m_MapMoveCursors.Add(cursor);
                        cell.hasMoveCursor = true;
                    }
                    else if (type == MapCursor.CursorType.Attack)
                    {
                        //m_MapAttackCursors.Add(cursor);
                        cell.hasAttackCursor = true;
                    }

                    m_Cursors.Add(cursor);
                }
            }
        }

        /// <summary>
        /// 隐藏cursor
        /// </summary>
        public void HideRangeCursors()
        {
            //if (m_MapMoveCursors.Count > 0)
            //{
            //    for (int i = 0; i < m_MapMoveCursors.Count; i++)
            //    {
            //        ObjectPool.DespawnUnsafe(m_MapMoveCursors[i].gameObject, true);
            //    }
            //    m_MapMoveCursors.Clear();
            //}

            //if (m_MapAttackCursors.Count > 0)
            //{
            //    for (int i = 0; i < m_MapAttackCursors.Count; i++)
            //    {
            //        ObjectPool.DespawnUnsafe(m_MapAttackCursors[i].gameObject, true);
            //    }
            //    m_MapAttackCursors.Clear();
            //}

            if (m_Cursors.Count > 0)
            {
                foreach (MapCursor cursor in m_Cursors)
                {
                    //CellData cellData = GetCellData(cursor.cellPosition);
                    //if (cellData != null)
                    //{
                    //    cellData.hasCursor = false;
                    //}
                    ObjectPool.DespawnUnsafe(cursor.gameObject, true);
                }
                m_Cursors.Clear();
            }
        }
        #endregion

        #region Path Finding Method
        /// <summary>
        /// 搜寻移动范围
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="movePoint"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        public List<CellData> SearchMoveRange(CellData cell, float movePoint, MoveConsumption consumption)
        {
            if (findMoveRange == null)
            {
                Debug.LogError("Error: Find move range is null.");
                return null;
            }

            if (!m_SearchPath.SearchMoveRange(findMoveRange, cell, movePoint, consumption))
            {
                Debug.LogErrorFormat("Error: Move Range({0}) is Not Found.", 5f);
                return null;
            }

            return m_SearchPath.result;
        }

        /// <summary>
        /// 搜寻攻击范围
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public List<CellData> SearchAttackRange(CellData cell, int minRange, int maxRange, bool useEndCell = false)
        {
            if (findAttackRange == null)
            {
                Debug.LogError("Error: Find attack range is null.");
                return null;
            }

            if (!m_SearchPath.SearchAttackRange(findAttackRange, cell, minRange, maxRange, useEndCell))
            {
                Debug.LogErrorFormat("Error: Attack Range({0} - {1}) is Not Found.", 2, 3);
                return null;
            }

            return m_SearchPath.result;
        }

        /// <summary>
        /// 搜寻路径
        /// </summary>
        /// <param name="startCell"></param>
        /// <param name="endCell"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        public List<CellData> SearchPath(CellData startCell, CellData endCell, MoveConsumption consumption)
        {
            if (findPathDirect == null)
            {
                Debug.LogError("Error: Find path is null.");
                return null;
            }

            if (!m_SearchPath.SearchPath(findPathDirect, startCell, endCell, consumption))
            {
                Debug.LogError("Error: Search Path Error. Maybe some cells are out of range.");
                return null;
            }

            return m_SearchPath.result;
        }

        /// <summary>
        /// 搜寻移动范围与攻击范围
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="nAtk">是否包含攻击范围</param>
        /// <param name="moveCells"></param>
        /// <param name="atkCells"></param>
        /// <returns></returns>
        public bool SearchMoveRange(
            MapClass cls, 
            bool nAtk, 
            out IEnumerable<CellData> moveCells, 
            out IEnumerable<CellData> atkCells)
        {
            moveCells = null;
            atkCells = null;

            if (cls == null)
            {
                Debug.LogError("MapGraph -> SearchMoveRange: `cls` is null.");
                return false;
            }

            CellData cell = GetCellData(cls.cellPosition);
            if (cell == null)
            {
                Debug.LogError("MapGraph -> SearchMoveRange: `cls.cellPosition` is out of range.");
                return false;
            }

            // 搜索移动范围，从MapClass中读取数据
            Role role = cls.role;
            if (role == null)
            {
                Debug.LogErrorFormat(
                    "MapGraph -> SearchMoveRange: `cls.role` is null. Pos: {0}", 
                    cell.position.ToString());
                return false;
            }

            float movePoint = role.movePoint;
            MoveConsumption consumption = role.cls.moveConsumption;

            List<CellData> rangeCells = SearchMoveRange(cell, movePoint, consumption);
            if (rangeCells == null)
            {
                return false;
            }

            HashSet<CellData> moveRangeCells = new HashSet<CellData>(
                rangeCells, cellPositionEqualityComparer);
            moveCells = moveRangeCells;

            if (nAtk && role.equipedWeapon != null /* 是否有武器 */)
            {
                // 搜索攻击范围，从MapClass中读取数据
                Vector2Int atkRange = new Vector2Int(
                    role.equipedWeapon.uniqueInfo.minRange, 
                    role.equipedWeapon.uniqueInfo.maxRange);

                HashSet<CellData> atkRangeCells = new HashSet<CellData>(cellPositionEqualityComparer);
                foreach (CellData moveCell in moveRangeCells)
                {
                    rangeCells = SearchAttackRange(moveCell, atkRange.x, atkRange.y, true);

                    if (rangeCells == null)
                    {
                        return false;
                    }

                    if (rangeCells.Count > 0)
                    {
                        atkRangeCells.UnionWith(rangeCells.Where(c => !moveRangeCells.Contains(c)));
                    }
                }

                atkCells = atkRangeCells;
            }

            return true;
        }

        /// <summary>
        /// 搜寻和显示范围
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="nAtk">包含攻击范围</param>
        /// <returns></returns>
        public bool SearchAndShowMoveRange(MapClass cls, bool nAtk)
        {
            IEnumerable<CellData> moveCells, atkCells;
            if (!SearchMoveRange(cls, nAtk, out moveCells, out atkCells))
            {
                return false;
            }

            if (moveCells != null)
            {
                ShowRangeCursors(moveCells, MapCursor.CursorType.Move);
            }

            if (atkCells != null)
            {
                ShowRangeCursors(atkCells, MapCursor.CursorType.Attack);
            }

            return true;
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// 地图是否包含Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector3Int position)
        {
            return mapRect.Contains(new Vector2Int(position.x, position.y));
        }
        public bool Contains(int x, int y)
        {
            return mapRect.Contains(new Vector2Int(x, y));
        }

        /// <summary>
        /// 获取Cell的位置
        /// </summary>
        /// <param name="cellPosition">网格坐标</param>
        /// <param name="center">是否是中心位置</param>
        /// <param name="world">是否是世界坐标</param>
        /// <returns></returns>
        public Vector3 GetCellPosition(Vector3Int cellPosition, bool world = true, bool center = false)
        {
            Vector3 pos;

            if (world)
            {
                pos = grid.GetCellCenterWorld(cellPosition);
            }
            else
            {
                pos = grid.GetCellCenterLocal(cellPosition);
            }

            if (!center)
            {
                pos.y -= halfCellSize.y;
            }
            
            return pos;
        }

        /// <summary>
        /// 获取Terrain层的Tile
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public SrpgTile GetTile(Vector3Int position)
        {
            return terrainTilemap.GetTile<SrpgTile>(position);
        }

        /// <summary>
        /// 改变地形
        /// </summary>
        /// <param name="position"></param>
        /// <param name="newTile"></param>
        public void ChangeTile(Vector3Int position, SrpgTile newTile)
        {
            SrpgTile old = GetTile(position);
            if (old == newTile)
            {
                return;
            }

            terrainTilemap.SetTile(position, newTile);
            terrainTilemap.RefreshTile(position);

            if (Contains(position))
            {
                m_DataDict[position].hasTile = newTile != null;
            }
        }

        /// <summary>
        /// 获取CellData
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CellData GetCellData(Vector3Int position)
        {
            if (!Contains(position))
            {
                return null;
            }
            return m_DataDict[position];
        }
        #endregion

        #region class CellPositionEqualityComparer
        /// <summary>
        /// 判断两个Cell的Position是否相等
        /// </summary>
        public class CellPositionEqualityComparer : IEqualityComparer<CellData>
        {
            public bool Equals(CellData x, CellData y)
            {
                return x.position == y.position;
            }

            public int GetHashCode(CellData obj)
            {
                return obj.position.GetHashCode();
            }
        }
        #endregion
    }
}
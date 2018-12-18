#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				EditorTestPathFinding.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 02 Sep 2018 01:49:51 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps.Testing
{
    using DR.Book.SRPG_Dev.Maps.FindPath;
    using DR.Book.SRPG_Dev.Models;
    using DR.Book.SRPG_Dev.Framework;

    public class EditorTestPathFinding : MonoBehaviour
    {
        [Serializable]
        public enum TestPathfindingType
        {
            Attack,
            Move,
            MoveAndAttack,
            Path
        }

        public string m_ConfigDirectory;

        public MapGraph m_Map;
        public MapClass m_TestClassPrefab;
        //public GameObject m_TestCursorPrefab;

        /// <summary>
        /// 选择测试的寻路类型
        /// </summary>
        public TestPathfindingType m_PathfindingType;

        /// <summary>
        /// 移动点数
        /// </summary>
        public float m_MovePoint = 9f;

        /// <summary>
        /// 攻击范围
        /// </summary>
        public Vector2Int m_AttackRange = new Vector2Int(2, 3);

        /// <summary>
        /// 是否打印不关键的信息
        /// </summary>
        public bool m_DebugInfo = true;

        /// <summary>
        /// 是否打印寻路的每一步
        /// </summary>
        public bool m_DebugStep = false;

        //private List<GameObject> m_TestCursors;

        private MapClass m_TestClass;
        //private MoveConsumption m_MoveConsumption;
        //private List<CellData> m_CursorCells;

        private CellData m_StartCell;
        private CellData m_EndCell;

#if UNITY_EDITOR
        private void Awake()
        {
            if (string.IsNullOrEmpty(m_ConfigDirectory))
            {
                m_ConfigDirectory = Application.streamingAssetsPath + "/Config";
            }

            ConfigLoader.rootDirectory = m_ConfigDirectory;
        }

        private void Start()
        {
            if (m_Map == null)
            {
                m_Map = GameObject.FindObjectOfType<MapGraph>();
            }

            if (m_Map == null)
            {
                Debug.LogError("EditorTestPathFinding -> Map was not found.");
                return;
            }

            m_Map.InitMap();
            m_Map.searchPath.onStep += MapPathfinding_OnStep;

            //if (m_TestCursorPrefab == null)
            //{
            //    Debug.LogError("EditorTestPathFinding -> Cursor Prefab is null.");
            //    return;
            //}

            //m_TestCursors = new List<GameObject>();

            if (m_TestClassPrefab == null)
            {
                Debug.LogError("EditorTestPathFinding -> Class Prefab is null.");
                return;
            }

            //m_TestClass = GameObject.Instantiate<MapClass>(
            //    m_TestClassPrefab,
            //    m_Map.transform.Find("MapObject"),
            //    false);
            //m_TestClass.map = m_Map;
            //m_TestClass.UpdatePosition(new Vector3Int(-1, -1, 0));
            m_TestClass = m_Map.CreateMapObject(m_TestClassPrefab, new Vector3Int(-1, -1, 0)) as MapClass;
            m_TestClass.onMovingEnd += M_TestClass_onMovingEnd;
            //RecreateMoveConsumption();
            //m_CursorCells = new List<CellData>();

            // 读取一个角色
            m_TestClass.Load(0, RoleType.Unique);

            // 我们需要一个武器来显示攻击范围
            ItemModel model = ModelManager.models.Get<ItemModel>();
            m_TestClass.role.AddItem(model.CreateItem(0));
        }

        private void M_TestClass_onMovingEnd(CellData endCell)
        {
            m_TestClass.animatorController.StopMove();
        }

        private void MapPathfinding_OnStep(PathFinding searchPath)
        {
            if (m_DebugInfo && m_DebugStep)
            {
                Debug.LogFormat("{0}: The G value of Cell {1} is {2}.",
                    searchPath.searchCount,
                    searchPath.currentCell.position.ToString(),
                    searchPath.currentCell.g.ToString());
            }
        }

        private void Update()
        {
            if (m_Map == null /*|| m_TestCursorPrefab == null*/)
            {
                return;
            }

            if (Input.anyKeyDown && m_TestClass.moving)
            {
                Debug.Log("Wait for moving.");
                return;
            }

            Vector3 mousePos = Input.mousePosition;
            Vector3 world = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3Int cellPosition = m_Map.grid.WorldToCell(world);
            CellData selectedCell = m_Map.GetCellData(cellPosition);

            // 左键建立范围
            if (Input.GetMouseButtonDown(0) && selectedCell != null && selectedCell.canMove)
            {
                if (m_AttackRange.x < 1 || m_AttackRange.y < m_AttackRange.x)
                {
                    Debug.LogError("EditorTestPathFinding -> Check the attack range.");
                    return;
                }

                if (selectedCell != null)
                {
                    switch (m_PathfindingType)
                    {
                        case TestPathfindingType.Move:
                        case TestPathfindingType.MoveAndAttack:
                            ShowMoveRangeCells(selectedCell);
                            break;
                        case TestPathfindingType.Path:
                            ClearTestCursors();
                            if (!ShowPathCells(selectedCell))
                            {
                                return;
                            }
                            break;
                        default:
                            ClearTestCursors();
                            ShowAttackRangeCells(selectedCell);
                            break;
                    }
                }
            }

            // 右键删除建立的cursor
            if (Input.GetMouseButtonDown(1))
            {
                ClearTestCursors();
                ClearMoveRangeAndPath();
            }

            // 中建重新生成移动消耗
            if (Input.GetMouseButtonDown(2))
            {
                //RecreateMoveConsumption();
            }

            // 刷新鼠标位置
            m_Map.mouseCursor.UpdatePosition(cellPosition);
        }

        private void OnDestroy()
        {
            if (m_Map != null)
            {
                m_Map.searchPath.onStep -= MapPathfinding_OnStep;
                m_Map = null;
            }
        }

        /// <summary>
        /// 生成Cursors
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="atk"></param>
        public void CreateTestCursors(IEnumerable<CellData> cells, bool atk)
        {
            if (cells != null)
            {
                MapCursor.CursorType type = atk ? MapCursor.CursorType.Attack : MapCursor.CursorType.Move;
                m_Map.ShowRangeCursors(cells, type);

                //Color color = Color.clear;
                //if (atk)
                //{
                //    color = Color.red;
                //    color.a = 0.5f;
                //}

                //foreach (var cell in cells)
                //{
                //    CreateTestCursor(cell, color);
                //}
            }
        }

        ///// <summary>
        ///// 生成单个Cursor
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <param name="color"></param>
        //public void CreateTestCursor(CellData cell, Color color)
        //{
        //    Vector3 pos = m_Map.GetCellPosition(cell.position);
        //    GameObject find = GameObject.Instantiate(m_TestCursorPrefab, transform, false);
        //    find.name = cell.position.ToString();
        //    find.transform.position = pos;
        //    if (color != Color.clear)
        //    {
        //        find.GetComponent<SpriteRenderer>().color = color;
        //        find.name += " atk";
        //    }
        //    m_TestCursors.Add(find);
        //}

        public void ClearTestCursors()
        {
            m_Map.HideRangeCursors();
            
            //if (m_TestCursors != null && m_TestCursors.Count > 0)
            //{
            //    foreach (var find in m_TestCursors)
            //    {
            //        GameObject.Destroy(find);
            //    }
            //    m_TestCursors.Clear();
            //}
        }

        public void ClearMoveRangeAndPath()
        {
            m_StartCell = null;
            m_EndCell = null;

            m_TestClass.UpdatePosition(new Vector3Int(-1, -1, 0));
            //m_CursorCells.Clear();
        }

        /// <summary>
        /// 重新生成移动消耗
        /// </summary>
        //public void RecreateMoveConsumption()
        //{
        //    // 在构造函数内部用的Random初始化，
        //    // 有了数据后，这个地方将进行修改
        //    // m_MoveConsumption = new MoveConsumption(ClassType.Foot);
        //    MoveConsumptionInfoConfig config = MoveConsumptionInfoConfig.Get<MoveConsumptionInfoConfig>();
        //    MoveConsumptionInfo info = config[ClassType.Foot];
        //    m_MoveConsumption = new MoveConsumption(info);

        //    //Debug.LogFormat("{0}={1}, {2}={3}",
        //    //    TerrainType.Plain.ToString(),
        //    //    m_MoveConsumption[TerrainType.Plain].ToString(),
        //    //    TerrainType.Road.ToString(),
        //    //    m_MoveConsumption[TerrainType.Road].ToString());
        //}

        /// <summary>
        /// 当左键按下时，Move类型的活动
        /// </summary>
        /// <param name="selectedCell"></param>
        /// <returns></returns>
        public List<CellData> ShowMoveRangeCells(CellData selectedCell)
        {
            //List<CellData> cells = null;
            ClearTestCursors();
            
            //if (m_CursorCells.Count == 0 || !m_CursorCells.Contains(selectedCell))
            if (!selectedCell.hasMoveCursor)
            {
                if (m_DebugInfo)
                {
                    Debug.LogFormat("MoveRange: start {0}, move point {1}, status ({2})",
                        selectedCell.position.ToString(),
                        m_MovePoint.ToString(),
                        selectedCell.GetStatus().ToString());
                }

                //ClearTestCursors();

                //m_CursorCells.Clear();
                m_TestClass.UpdatePosition(selectedCell.position);

                m_Map.SearchAndShowMoveRange(m_TestClass, m_PathfindingType == TestPathfindingType.MoveAndAttack);

                //cells = m_Map.SearchMoveRange(selectedCell, m_MovePoint, m_MoveConsumption);
                ////m_CursorCells.AddRange(cells);
                //CreateTestCursors(cells, false);

                //if (m_PathfindingType == TestPathfindingType.MoveAndAttack)
                //{
                //    // 移动范围后，进行查找攻击范围
                //    HashSet<CellData> attackCells = new HashSet<CellData>();
                //    foreach (var cell in cells.ToArray())
                //    {
                //        //foreach (var c in m_Map.SearchAttackRange(cell, m_AttackRange.x, m_AttackRange.y, true))
                //        //{
                //        //    //if (!cells.Contains(c) && !attackCells.Contains(c))
                //        //    if (!c.hasCursor)
                //        //    {
                //        //        attackCells.Add(c);
                //        //    }
                //        //}
                //        attackCells.UnionWith(
                //            m_Map.SearchAttackRange(cell, m_AttackRange.x, m_AttackRange.y, true)
                //            .Where(c => !c.hasCursor));
                //    }
                //    CreateTestCursors(attackCells, true);
                //}
            }
            else
            {
                if (m_DebugInfo)
                {
                    Debug.LogFormat("Selected end {0} status ({1})", 
                        selectedCell.position.ToString(),
                        selectedCell.GetStatus().ToString());
                }
                //ClearTestCursors();
                //m_CursorCells.Clear();
                Stack<CellData> pathCells = m_Map.searchPath.BuildPath(selectedCell);
                //cells = new List<CellData>(pathCells);
                m_TestClass.animatorController.PlayMove();
                m_TestClass.StartMove(pathCells);

                //CreateTestCursors(cells, false);
                CreateTestCursors(pathCells, false);
            }

            //return cells;
            return null;
        }

        /// <summary>
        /// 当左键按下时，Attack类型的活动
        /// </summary>
        /// <param name="selectedCell"></param>
        /// <returns></returns>
        public List<CellData> ShowAttackRangeCells(CellData selectedCell)
        {
            if (m_DebugInfo)
            {
                Debug.LogFormat("AttackRange: start position is {0}, range is {1}",
                    selectedCell.position.ToString(),
                    m_AttackRange.ToString());
            }

            List<CellData> cells = m_Map.SearchAttackRange(selectedCell, m_AttackRange.x, m_AttackRange.y);
            CreateTestCursors(cells, true);
            return cells;
        }

        /// <summary>
        /// 当左键按下时，Path类型的活动
        /// </summary>
        /// <param name="selectedCell"></param>
        /// <returns></returns>
        public bool ShowPathCells(CellData selectedCell)
        {
            if (m_StartCell == null)
            {
                m_StartCell = selectedCell;
                if (m_DebugInfo)
                {
                    Debug.LogFormat("Selected start position {0}", m_StartCell.position);
                }
                m_TestClass.UpdatePosition(selectedCell.position);
                return false;
            }

            m_EndCell = selectedCell;
            if (m_DebugInfo)
            {
                Debug.LogFormat("Selected end position {0}", m_EndCell.position);
            }
            List<CellData> cells = m_Map.SearchPath(m_StartCell, m_EndCell, m_TestClass.role.moveConsumption);
            m_TestClass.animatorController.PlayMove();
            m_TestClass.StartMove(new Stack<CellData>(cells));
            m_StartCell = null;
            m_EndCell = null;
            CreateTestCursors(cells, false);
            return true;
        }

#endif
    }
}
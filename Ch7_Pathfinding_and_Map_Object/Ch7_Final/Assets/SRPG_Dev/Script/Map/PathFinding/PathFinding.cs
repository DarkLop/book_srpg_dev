#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				PathFinding.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 05 Apr 2018 22:27:32 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps.FindPath
{
    using DR.Book.SRPG_Dev.Models;

    public class PathFinding
    {
        #region Delegate/Event
        public delegate void OnStepDelegate(PathFinding search);
        public event OnStepDelegate onStep;
        #endregion

        #region Field
        private MapGraph m_Map;

        /// <summary>
        /// 开放列表
        /// </summary>
        private List<CellData> m_Reachable = new List<CellData>();

        /// <summary>
        /// 关闭列表
        /// </summary>
        private List<CellData> m_Explored = new List<CellData>();

        /// <summary>
        /// 结果
        /// </summary>
        private List<CellData> m_Result = new List<CellData>();

        private Vector2 m_Range;
        private CellData m_StartCell;
        private CellData m_EndCell;
        private CellData m_CurrentCell;
        private bool m_Finished;

        private IHowToFind m_HowToFind;
        private MoveConsumption m_MoveConsumption;

        private int m_SearchCount = 0;
        public int m_MaxSearchCount = 2000;
        #endregion

        #region Property
        public MapGraph map
        {
            get { return m_Map; }
        }

        public List<CellData> reachable
        {
            get { return m_Reachable; }
        }

        public List<CellData> explored
        {
            get { return m_Explored; }
        }

        public List<CellData> result
        {
            get { return m_Result; }
        }

        public Vector2 range
        {
            get { return m_Range; }
        }

        public CellData startCell
        {
            get { return m_StartCell; }
        }

        public CellData endCell
        {
            get { return m_EndCell; }
        }

        public bool finished
        {
            get { return m_Finished; }
        }

        public CellData currentCell
        {
            get { return m_CurrentCell; }
        }

        public int searchCount
        {
            get { return m_SearchCount; }
        }
        #endregion

        #region Constructor
        public PathFinding(MapGraph map)
        {
            m_Map = map;
        }
        #endregion

        #region Reset Method
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            m_Reachable.Clear();
            m_Explored.Clear();
            m_Result.Clear();

            m_Range = Vector2.zero;
            m_StartCell = null;
            m_EndCell = null;
            m_CurrentCell = null;
            m_Finished = false;
            m_HowToFind = null;
            m_MoveConsumption = null;

            m_SearchCount = 0;
        }
        #endregion

        #region Main Method
        /// <summary>
        /// 寻找移动范围
        /// </summary>
        /// <param name="howToFind"></param>
        /// <param name="start"></param>
        /// <param name="movePoint"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        public bool SearchMoveRange(IHowToFind howToFind, CellData start, float movePoint, MoveConsumption consumption)
        {
            if (howToFind == null || start == null || movePoint < 0)
            {
                return false;
            }

            Reset();

            m_HowToFind = howToFind;
            m_MoveConsumption = consumption;

            m_StartCell = start;
            m_StartCell.ResetAStar();
            m_Range.y = movePoint;

            m_Reachable.Add(m_StartCell);

            return SearchRangeInternal();
        }

        /// <summary>
        /// 搜寻攻击范围
        /// </summary>
        /// <param name="howToFind"></param>
        /// <param name="start"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <param name="useEndCell"></param>
        /// <returns></returns>
        public bool SearchAttackRange(IHowToFind howToFind, CellData start, int minRange, int maxRange, bool useEndCell = false)
        {
            if (howToFind == null || start == null || minRange < 1 || maxRange < minRange)
            {
                return false;
            }

            Reset();

            m_HowToFind = howToFind;
            m_Range = new Vector2(minRange, maxRange);

            if (useEndCell)
            {
                m_EndCell = start;
                m_EndCell.g = 0f;
                m_EndCell.h = 0f;
                m_Reachable.Add(m_EndCell);
            }
            else
            {
                m_StartCell = start;
                m_StartCell.g = 0f;
                m_StartCell.h = 0f;
                m_Reachable.Add(m_StartCell);
            }

            return SearchRangeInternal();
        }

        /// <summary>
        /// 搜寻路径
        /// </summary>
        /// <param name="howToFind"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        public bool SearchPath(IHowToFind howToFind, CellData start, CellData end, MoveConsumption consumption = null)
        {
            if (howToFind == null || start == null || end == null)
            {
                return false;
            }

            Reset();

            m_HowToFind = howToFind;
            m_MoveConsumption = consumption;

            m_StartCell = start;
            m_StartCell.ResetAStar();
            m_EndCell = end;
            m_EndCell.ResetAStar();

            m_Reachable.Add(m_StartCell);

            m_StartCell.h = m_HowToFind.CalcH(this, m_StartCell);

            return SearchRangeInternal();
        }

        /// <summary>
        /// 开始迭代
        /// </summary>
        /// <returns></returns>
        private bool SearchRangeInternal()
        {
            while (!m_Finished)
            {
                m_SearchCount++;
                m_Finished = FindNext();

                if (!m_Finished && onStep != null)
                {
                    onStep(this);
                }

                if (m_SearchCount >= m_MaxSearchCount)
                {
                    Debug.LogError("Search is timeout. MaxCont: " + m_MaxSearchCount.ToString());
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 搜寻下一次，return finished
        /// </summary>
        /// <returns></returns>
        private bool FindNext()
        {
            // 已经有结果
            if (m_Result.Count > 0)
            {
                return true;
            }

            // 选择节点
            m_CurrentCell = m_HowToFind.ChoseCell(this);

            // 判断是否搜索结束
            if (m_HowToFind.IsFinishedOnChose(this))
            {
                // 如果结束，建立结果
                m_HowToFind.BuildResult(this);
                return true;
            }

            // 当前选择的节点不为null
            if (m_CurrentCell != null)
            {
                for (int i = 0; i < m_CurrentCell.adjacents.Count; i++)
                {
                    // 是否可以加入到开放集中
                    if (m_HowToFind.CanAddAdjacentToReachable(this, m_CurrentCell.adjacents[i]))
                    {
                        m_Reachable.Add(m_CurrentCell.adjacents[i]);
                    }
                }
            }
            return false;
        }


        #endregion

        #region Helper
        /// <summary>
        /// 是否在Reachable列表中。
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsCellInReachable(CellData cell)
        {
            return m_Reachable.Contains(cell);
        }

        /// <summary>
        /// 是否在Expored列表中
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool IsCellInExpored(CellData cell)
        {
            return m_Explored.Contains(cell);
        }

        /// <summary>
        /// 获取移动消耗
        /// </summary>
        /// <param name="terrainType"></param>
        /// <returns></returns>
        public float GetMoveConsumption(TerrainType terrainType)
        {
            if (m_MoveConsumption == null)
            {
                return 1f;
            }
            return m_MoveConsumption[terrainType];
        }

        /// <summary>
        /// 建立路径List
        /// </summary>
        /// <param name="endCell"></param>
        /// <param name="useResult"></param>
        /// <returns></returns>
        public List<CellData> BuildPath(CellData endCell, bool useResult)
        {
            if (endCell == null)
            {
                Debug.LogError("PathFinding -> Argument named `endCell` is null.");
                return null;
            }

            List<CellData> path = useResult ? m_Result : new List<CellData>();

            CellData current = endCell;
            path.Add(current);
            while (current.previous != null)
            {
                current = current.previous;
                path.Add(current);
            }
            return path;
        }

        /// <summary>
        /// 建立路径Stack
        /// </summary>
        /// <param name="endCell"></param>
        /// <returns></returns>
        public Stack<CellData> BuildPath(CellData endCell)
        {
            if (endCell == null)
            {
                Debug.LogError("PathFinding -> Argument named `endCell` is null.");
                return null;
            }

            Stack<CellData> path = new Stack<CellData>();

            CellData current = endCell;
            path.Push(current);
            while (current.previous != null)
            {
                current = current.previous;
                path.Push(current);
            }
            return path;
        }
        #endregion
    }
}
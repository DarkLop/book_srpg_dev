#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CellData.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 05 Apr 2018 22:06:08 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps
{
    /// <summary>
    /// 地图上每个格子的信息
    /// </summary>
    public class CellData : IDisposable
    {
        #region Common Field/Property
        private Vector3Int m_Position;
        private MapObject m_MapObject;
        private CellStatus m_Status = CellStatus.None;

        /// <summary>
        /// 坐标位置
        /// </summary>
        public Vector3Int position
        {
            get { return m_Position; }
        }

        /// <summary>
        /// 是否有Tile
        /// </summary>
        public bool hasTile
        {
            get { return CheckStatus(CellStatus.TerrainTile, false); }
            set { SwitchStatus(CellStatus.TerrainTile, value); }
        }

        /// <summary>
        /// 是否有Cursor
        /// </summary>
        public bool hasCursor
        {
            get { return CheckStatus(CellStatus.MoveCursor | CellStatus.AttackCursor, true); }
            set { SwitchStatus(CellStatus.MoveCursor | CellStatus.AttackCursor, value); }
        }

        /// <summary>
        /// 是否有移动范围光标
        /// </summary>
        public bool hasMoveCursor
        {
            get { return CheckStatus(CellStatus.MoveCursor, false); }
            set { SwitchStatus(CellStatus.MoveCursor, value); }
        }

        /// <summary>
        /// 是否有攻击范围光标
        /// </summary>
        public bool hasAttackCursor
        {
            get { return CheckStatus(CellStatus.AttackCursor, false); }
            set { SwitchStatus(CellStatus.AttackCursor, value); }
        }

        /// <summary>
        /// 地图对象
        /// </summary>
        public MapObject mapObject
        {
            get { return m_MapObject; }
            set
            {
                m_MapObject = value;
                SwitchStatus(CellStatus.MapObject, value != null);
            }
        }

        /// <summary>
        /// 是否有地图对象
        /// </summary>
        public bool hasMapObject
        {
            get { return mapObject != null; }
        }

        /// <summary>
        /// 是否可移动
        /// </summary>
        public bool canMove
        {
            get { return hasTile && !hasMapObject; }
        }

        /// <summary>
        /// 获取状态开关
        /// </summary>
        /// <returns></returns>
        public CellStatus GetStatus()
        {
            return m_Status;
        }

        /// <summary>
        /// 开关是否开启 
        /// any：
        ///     true 表示，判断在status中是否存在开启项
        ///     false 表示，判断status中是否全部开启
        /// </summary>
        /// <param name="status"></param>
        /// <param name="any"></param>
        /// <returns></returns>
        public bool CheckStatus(CellStatus status, bool any)
        {
            return any ? (m_Status & status) != 0 : (m_Status & status) == status;
        }

        /// <summary>
        /// 设置状态开关
        /// </summary>
        /// <param name="status"></param>
        /// <param name="isOn"></param>
        public void SwitchStatus(CellStatus status, bool isOn)
        {
            if (isOn)
            {
                m_Status |= status;
            }
            else
            {
                m_Status &= ~status;
            }
        }
        #endregion

        #region AStar Field/Property
        private List<CellData> m_Adjacents = new List<CellData>();
        private CellData m_Previous;
        private Vector2 m_AStarGH;

        /// <summary>
        /// 邻居CellData
        /// </summary>
        public List<CellData> adjacents
        {
            get { return m_Adjacents; }
        }

        /// <summary>
        /// 寻找的前一个CellData
        /// </summary>
        public CellData previous
        {
            get { return m_Previous; }
            set { m_Previous = value; }
        }

        /// <summary>
        /// AStar的G值，移动消耗
        /// </summary>
        public float g
        {
            get { return m_AStarGH.x; }
            set { m_AStarGH.x = value; }
        }

        /// <summary>
        /// AStar的H值，预计消耗
        /// </summary>
        public float h
        {
            get { return m_AStarGH.y; }
            set { m_AStarGH.y = value; }
        }

        /// <summary>
        /// AStar的F值，F=G+H
        /// </summary>
        public float f
        {
            get { return m_AStarGH.x + m_AStarGH.y; }
        }
        #endregion

        #region Constructor
        public CellData(Vector3Int position)
        {
            m_Position = position;
        }

        public CellData(int x, int y)
        {
            m_Position = new Vector3Int(x, y, 0);
        }
        #endregion

        #region Reset AStar Method
        public void ResetAStar ()
        {
            m_Previous = null;
            m_AStarGH = Vector2.zero;
        }
        #endregion

        #region Method
        public void Dispose()
        {
            m_Position = Vector3Int.zero;
            m_MapObject = null;
            m_Adjacents = null;
            m_Previous = null;
            m_AStarGH = Vector2.zero;
            m_Status = CellStatus.None;
        }
        #endregion
    }
}
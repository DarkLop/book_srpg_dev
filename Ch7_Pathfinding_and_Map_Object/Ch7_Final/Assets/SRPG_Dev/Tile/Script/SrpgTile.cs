#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				SrpgTile.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 24 Jan 2018 01:59:26 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New SRPG Tile.asset", menuName = "SRPG/Tile")]
    public class SrpgTile : RuleTile
    {
        [SerializeField]
        private TerrainType m_TerrainType = TerrainType.Plain;
        [SerializeField]
        private int m_AvoidRate = 0;

        /// <summary>
        /// 地形类型
        /// </summary>
        public TerrainType terrainType
        {
            get { return m_TerrainType; }
            set { m_TerrainType = value; }
        }

        /// <summary>
        /// 回避率
        /// </summary>
        public int avoidRate
        {
            get { return m_AvoidRate; }
            set { m_AvoidRate = value; }
        }
    }
}
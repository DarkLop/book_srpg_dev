#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CellStatus.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 10 Sep 2018 18:55:52 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Maps
{
    /// <summary>
    /// 格子状态
    /// </summary>
    [Serializable, Flags]
    public enum CellStatus : byte
    {
        /// <summary>
        /// 没有任何东西， 0000 0000
        /// </summary>
        None = 0,

        /// <summary>
        /// Tile， 0000 0001
        /// </summary>
        TerrainTile = 0x01,

        /// <summary>
        /// 移动光标， 0000 0010
        /// </summary>
        MoveCursor = 0x02,

        /// <summary>
        /// 攻击光标， 0000 0100
        /// </summary>
        AttackCursor = 0x04,

        /// <summary>
        /// 地图对象， 0000 1000
        /// </summary>
        MapObject = 0x08,

        // 如果有其它需求，在这里添加其余4个开关属性

        /// <summary>
        /// 全部8个开关， 1111 1111
        /// </summary>
        All = byte.MaxValue
    }
}
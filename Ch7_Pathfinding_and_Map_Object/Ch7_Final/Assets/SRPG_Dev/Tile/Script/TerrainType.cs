#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TerrainType.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 24 Jan 2018 01:59:26 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Maps
{
    /// <summary>
    /// 地形类型
    /// </summary>
    [Serializable]
    public enum TerrainType : byte
    {
        /// <summary>
        /// 高山
        /// </summary>
        Peak,

        /// <summary>
        /// 密林
        /// </summary>
        DeepForest,

        /// <summary>
        /// 悬崖
        /// </summary>
        Cliff,

        /// <summary>
        /// 平地
        /// </summary>
        Plain,

        /// <summary>
        /// 森林
        /// </summary>
        Forest,

        /// <summary>
        /// 海
        /// </summary>
        Sea,

        /// <summary>
        /// 河
        /// </summary>
        River,

        /// <summary>
        /// 山
        /// </summary>
        Mountain,

        /// <summary>
        /// 沙漠
        /// </summary>
        Desert,

        /// <summary>
        /// 城堡
        /// </summary>
        CastleDefence,

        /// <summary>
        /// 营寨
        /// </summary>
        [Obsolete("Game Not Used", true)]
        MilitaryCamp,

        /// <summary>
        /// 街
        /// </summary>
        [Obsolete("Game Not Used", true)]
        Street,

        /// <summary>
        /// 城门 （城堡用）
        /// </summary>
        [Obsolete("Game Not Used", true)]
        Gate,

        /// <summary>
        /// 城墙
        /// </summary>
        CastleWall,

        /// <summary>
        /// 砂地
        /// </summary>
        Sand,

        /// <summary>
        /// 桥
        /// </summary>
        Bridge,

        /// <summary>
        /// 湿地（沼泽）
        /// </summary>
        [Obsolete("Game Not Used", true)]
        Swamp,

        /// <summary>
        /// 城门2 （营寨用）
        /// </summary>
        [Obsolete("Game Not Used", true)]
        Gate2,

        /// <summary>
        /// 村庄
        /// </summary>
        Village,

        /// <summary>
        /// 废墟
        /// </summary>
        Ruins,

        /// <summary>
        /// 仓库
        /// </summary>
        [Obsolete("Game Not Used", true)]
        Warehouse,

        /// <summary>
        /// 废墟2 （仓库用）
        /// </summary>
        [Obsolete("Game Not Used", true)]
        Ruins2,

        /// <summary>
        /// 教会
        /// </summary>
        Church,

        /// <summary>
        /// 神殿
        /// </summary>
        [Obsolete("Game Not Used", true)]
        Temple,

        /// <summary>
        /// 布拉基之塔
        /// </summary>
        BlaggiTower,

        /// <summary>
        /// 道路
        /// </summary>
        Road,

        /// <summary>
        /// 地形类型数量长度
        /// </summary>
        MaxLength
    }
}
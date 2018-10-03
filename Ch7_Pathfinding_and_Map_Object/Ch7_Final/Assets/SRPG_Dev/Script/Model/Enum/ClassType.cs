#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ClassType.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 23:06:39 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    [Serializable]
    public enum ClassType : int
    {
        /// <summary>
        /// 骑兵1
        /// </summary>
        Knight1 = 0,

        /// <summary>
        /// 骑兵2
        /// </summary>
        Knight2,

        /// <summary>
        /// 飞行兵
        /// </summary>
        Flier,

        /// <summary>
        /// 步兵
        /// </summary>
        Foot,

        /// <summary>
        /// 装甲兵
        /// </summary>
        Armor,

        /// <summary>
        /// 士兵
        /// </summary>
        Soldier,

        /// <summary>
        /// 战士
        /// </summary>
        Fighter,

        /// <summary>
        /// 蛮族
        /// </summary>
        Brigand,

        /// <summary>
        /// 海盗
        /// </summary>
        Pirate,

        /// <summary>
        /// 贵族
        /// </summary>
        Nobility,

        /// <summary>
        /// 魔法师
        /// </summary>
        Mage,

        /// <summary>
        /// 盗贼
        /// </summary>
        Thief,

        /// <summary>
        /// 平民
        /// </summary>
        Civilian,

        /// <summary>
        /// 箭塔
        /// </summary>
        Archer,

        /// <summary>
        /// 黑暗王子
        /// </summary>
        DarkPrince,

        /// <summary>
        /// 最大长度
        /// </summary>
        MaxLength
    }
}
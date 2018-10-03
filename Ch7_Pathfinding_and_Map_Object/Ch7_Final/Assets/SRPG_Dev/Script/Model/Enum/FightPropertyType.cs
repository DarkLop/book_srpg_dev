#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FightPropertyType.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 21:18:15 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    [Serializable]
    public enum FightPropertyType : int
    {
        /// <summary>
        /// 力量
        /// </summary>
        STR = 0,

        /// <summary>
        /// 魔力
        /// </summary>
        MAG = 1,

        /// <summary>
        /// 技巧
        /// </summary>
        SKL = 2,

        /// <summary>
        /// 速度
        /// </summary>
        SPD = 3,

        /// <summary>
        /// 防御
        /// </summary>
        DEF = 4,

        /// <summary>
        /// 魔防
        /// </summary>
        MDF = 5,

        /// <summary>
        /// 长度
        /// </summary>
        MaxLength
    }
}
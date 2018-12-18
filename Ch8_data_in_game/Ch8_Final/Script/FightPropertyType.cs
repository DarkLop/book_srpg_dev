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

namespace DR.Book.SRPG_Dev
{
    [Serializable]
    public enum FightPropertyType : int
    {
        /// <summary>
        /// 生命值
        /// </summary>
        HP = 0,

        /// <summary>
        /// 魔法值
        /// </summary>
        MP,

        /// <summary>
        /// 力量
        /// </summary>
        STR,

        /// <summary>
        /// 魔力
        /// </summary>
        MAG,

        /// <summary>
        /// 技巧
        /// </summary>
        SKL,

        /// <summary>
        /// 速度
        /// </summary>
        SPD,

        /// <summary>
        /// 防御
        /// </summary>
        DEF,

        /// <summary>
        /// 魔防
        /// </summary>
        MDF,

        /// <summary>
        /// 长度
        /// </summary>
        MaxLength
    }
}
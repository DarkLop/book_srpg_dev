#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				WeaponType.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 02 Feb 2018 00:00:41 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev
{
    [Serializable]
    public enum WeaponType : int
    {
        /// <summary>
        /// 剑
        /// </summary>
        Sword = 0,

        /// <summary>
        /// 枪
        /// </summary>
        [Obsolete("No animation", true)]
        Lance = 1,

        /// <summary>
        /// 斧
        /// </summary>
        [Obsolete("No animation", true)]
        Axe = 2,

        /// <summary>
        /// 弓
        /// </summary>
        [Obsolete("No animation", true)]
        Bow = 3,

        /// <summary>
        /// 杖
        /// </summary>
        [Obsolete("No animation", true)]
        Staff = 4,
    }
}
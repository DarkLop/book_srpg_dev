#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ItemType.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 04:04:36 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev
{
    [Serializable, XmlType(IncludeInSchema = false)]
    public enum ItemType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow = 0,

        /// <summary>
        /// 武器
        /// </summary>
        Weapon,

        /// <summary>
        /// 饰品
        /// </summary>
        Ornament,

        /// <summary>
        /// 消耗品
        /// </summary>
        Consumable
    }
}
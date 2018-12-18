#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ConsumableUniqueInfo.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 04:47:36 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev.Models
{
    [Serializable]
    public class ConsumableUniqueInfo : UniqueInfo
    {
        /// <summary>
        /// 最大堆叠次数
        /// </summary>
        [XmlAttribute]
        public int stackingNumber;

        /// <summary>
        /// 使用次数
        /// </summary>
        [XmlAttribute]
        public int amountUsed;

        /// <summary>
        /// 使用效果ID
        /// </summary>
        [XmlAttribute]
        public int usingEffectId;
    }
}
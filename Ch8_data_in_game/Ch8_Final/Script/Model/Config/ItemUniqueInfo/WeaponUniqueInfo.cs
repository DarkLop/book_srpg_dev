#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				WeaponUniqueInfo.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 04:48:48 GMT
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
    public class WeaponUniqueInfo : OrnamentUniqueInfo
    {
        /// <summary>
        /// 武器类型
        /// </summary>
        [XmlAttribute]
        public WeaponType weaponType;

        /// <summary>
        /// 武器等级
        /// </summary>
        [XmlAttribute]
        public int level;

        /// <summary>
        /// 攻击力
        /// </summary>
        [XmlAttribute]
        public int attack;

        /// <summary>
        /// 最小攻击范围
        /// </summary>
        [XmlAttribute]
        public int minRange;

        /// <summary>
        /// 最大攻击范围
        /// </summary>
        [XmlAttribute]
        public int maxRange;

        /// <summary>
        /// 命中率
        /// </summary>
        [XmlAttribute]
        public int hit;

        /// <summary>
        /// 重量
        /// </summary>
        [XmlAttribute]
        public int weight;

        /// <summary>
        /// 耐久度
        /// </summary>
        [XmlAttribute]
        public int durability;
    }
}
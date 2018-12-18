#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				OrnamentUniqueInfo.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 04:48:14 GMT
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
    public class OrnamentUniqueInfo : UniqueInfo
    {
        /// <summary>
        /// 生命值加成
        /// </summary>
        //[XmlElement]
        //public int hp;

        /// <summary>
        /// 魔法值加成
        /// </summary>
        //[XmlElement]
        //public int mp;

        /// <summary>
        /// 战斗属性加成
        /// </summary>
        [XmlElement]
        public FightProperties fightProperties;

        /// <summary>
        /// 幸运加成
        /// </summary>
        [XmlElement]
        public int luk;

        /// <summary>
        /// 移动力加成
        /// </summary>
        [XmlElement]
        public float movePoint;

        /// <summary>
        /// TODO 包含的技能IDs
        /// </summary>
        //[XmlElement]
        //public int[] skills;

        /// <summary>
        /// TODO 特殊效果IDs
        /// </summary>
        //[XmlElement]
        //public int[] specialSkills;

    }
}
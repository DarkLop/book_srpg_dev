#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CharacterInfoConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 02:01:01 GMT
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
    public class CharacterInfoConfig : BaseXmlConfig<int, CharacterInfo>
    {
    }

    [Serializable]
    public class CharacterInfo : IConfigData<int>
    {
        /// <summary>
        /// 人物ID
        /// </summary>
        [XmlAttribute]
        public int id;

        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string name;

        /// <summary>
        /// 头像
        /// </summary>
        [XmlAttribute]
        public string profile;

        /// <summary>
        /// 人物的职业ID
        /// </summary>
        [XmlAttribute]
        public int classId;

        /// <summary>
        /// 基本等级
        /// </summary>
        [XmlAttribute]
        public int level;

        /// <summary>
        /// 基本生命值
        /// </summary>
        //[XmlAttribute]
        //public int hp;

        /// <summary>
        /// 基本魔法值（如果不同职业有不同副能量，还需要能量类型参数，比如怒气，能量等）
        /// </summary>
        //[XmlAttribute]
        //public int mp;

        /// <summary>
        /// 战斗属性
        /// </summary>
        [XmlElement]
        public FightProperties fightProperties;

        /// <summary>
        /// 幸运值
        /// </summary>
        [XmlAttribute]
        public int luk;

        /// <summary>
        /// TODO 包含的技能IDs
        /// </summary>
        //[XmlElement]
        //public int[] skills;

        /// <summary>
        /// 起始包含的物品IDs
        /// </summary>
        [XmlArray, XmlArrayItem]
        public int[] items;

        public int GetKey()
        {
            return this.id;
        }
    }
}
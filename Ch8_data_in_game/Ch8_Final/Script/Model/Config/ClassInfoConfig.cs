#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ClassInfoConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 05 Oct 2018 13:49:57 GMT
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
    public class ClassInfoConfig : BaseXmlConfig<int, ClassInfo>
    {
    }

    [Serializable]
    public class ClassInfo : IConfigData<int>
    {
        /// <summary>
        /// 职业ID
        /// </summary>
        [XmlAttribute]
        public int id;

        /// <summary>
        /// 预制体名称
        /// </summary>
        [XmlAttribute]
        public string prefab;

        /// <summary>
        /// 动画名称，当使用同一个prefab时，可以设置不同的动画
        /// </summary>
        [XmlAttribute]
        public string animator;

        /// <summary>
        /// 职业名称
        /// </summary>
        [XmlAttribute]
        public string name;

        /// <summary>
        /// 职业类型
        /// </summary>
        [XmlAttribute]
        public ClassType classType;

        /// <summary>
        /// 移动点数
        /// </summary>
        [XmlAttribute]
        public float movePoint;

        /// <summary>
        /// 战斗属性
        /// </summary>
        [XmlElement]
        public FightProperties fightProperties;

        /// <summary>
        /// 最大战斗属性
        /// </summary>
        [XmlElement]
        public FightProperties maxFightProperties;

        /// <summary>
        /// 可用武器
        /// </summary>
        [XmlElement]
        public AvailableWeapons availableWeapons;

        /// <summary>
        /// 部下头像
        /// </summary>
        [XmlAttribute]
        public string followingProfile;

        /// <summary>
        /// 部下成长
        /// </summary>
        [XmlElement]
        public FightProperties followingGrouth;

        /// <summary>
        /// TODO 包含的技能IDs
        /// </summary>
        //[XmlElement]
        //public int[] skills;

        public int GetKey()
        {
            return this.id;
        }

    }
}
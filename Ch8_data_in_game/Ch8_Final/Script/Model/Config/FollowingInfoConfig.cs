#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FollowingInfoConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 16 Oct 2018 14:43:13 GMT
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
    public class FollowingInfoConfig : BaseXmlConfig<int, FollowingInfo>
    {

    }

    [Serializable]
    public class FollowingInfo : IConfigData<int>
    {
        /// <summary>
        /// id
        /// </summary>
        [XmlAttribute]
        public int id;

        /// <summary>
        /// 所属阵营
        /// </summary>
        [XmlAttribute]
        public AttitudeTowards attitudeTowards;

        /// <summary>
        /// 头像
        /// </summary>
        [XmlAttribute]
        public string profile;

        /// <summary>
        /// 经验值
        /// </summary>
        [XmlAttribute]
        public int exp;

        /// <summary>
        /// 金钱
        /// </summary>
        [XmlAttribute]
        public int money;

        /// <summary>
        /// 基本等级
        /// </summary>
        [XmlAttribute]
        public int level;

        /// <summary>
        /// 基本生命值
        /// </summary>
        [XmlAttribute]
        public int hp;

        /// <summary>
        /// 基本魔法值（如果不同职业有不同副能量，还需要能量类型参数，比如怒气，能量等）
        /// </summary>
        [XmlAttribute]
        public int mp;

        /// <summary>
        /// 战斗属性成长率
        /// </summary>
        [XmlElement]
        public FightProperties fightGrowth;

        /// <summary>
        /// 幸运值
        /// </summary>
        [XmlAttribute]
        public int luk;

        /// <summary>
        /// TODO 起始包含的物品IDs
        /// </summary>
        //[XmlElement]
        //public int[] items;

        public int GetKey()
        {
            return this.id;
        }
    }
}
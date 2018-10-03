#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CharacterConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 16:57:01 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev.Models.Old
{
    [Serializable]
    public class CharacterConfig : XmlBase<CharacterConfig, int, CharacterInfo>
    {

    }

    [Serializable]
    public class CharacterInfo : IConfigKey<int>
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string profile { get; set; }
        [XmlAttribute]
        public int classId { get; set; }

        [XmlElement("HP")]
        public int hp { get; set; }

        [XmlElement("FightProperty")]
        public FightProperty property { get; set; }

        [XmlElement("LUK")]
        public int luk { get; set; }

        [XmlElement("Skills")]
        public SkillProperty skills { get; set; }

        [XmlElement("Money")]
        public int money { get; set; }

        [XmlIgnore]
        public int key { get { return id; } }
    }
}
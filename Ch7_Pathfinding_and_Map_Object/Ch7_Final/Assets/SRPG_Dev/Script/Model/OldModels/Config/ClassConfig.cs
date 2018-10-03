#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ClassConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 16:21:16 GMT
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
    public class ClassConfig : XmlBase<ClassConfig, int, ClassInfo>
    {

    }

    [Serializable]
    public class ClassInfo : IConfigKey<int>
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string prefab { get; set; }
        [XmlAttribute]
        public string animator { get; set; }
        [XmlAttribute]
        public ClassType classType { get; set; }
        [XmlAttribute]
        public int rankUpLevel { get; set; }

        [XmlElement("FightProperty")]
        public FightProperty property { get; set; }

        [XmlElement("MaxFightProperty")]
        public FightProperty maxProperty { get; set; }

        [XmlElement("MovePoint")]
        public int movePoint { get; set; }

        [XmlElement("Skills")]
        public SkillProperty skills { get; set; }

        [XmlIgnore]
        public int key { get { return id; } }
    }
}
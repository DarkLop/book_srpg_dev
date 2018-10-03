#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MoveConsumeConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 16:44:36 GMT
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
    public class MoveConsumeConfig : XmlBase<MoveConsumeConfig, ClassType, MoveConsumeInfo>
    {

    }

    [Serializable]
    public class MoveConsumeInfo : IConfigKey<ClassType>
    {
        [XmlAttribute]
        public ClassType type { get; set; }
        [XmlAttribute]
        public int[] consumes { get; set; }

        [XmlIgnore]
        public ClassType key { get { return type; } }
    }
}
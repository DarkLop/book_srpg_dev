#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				SkillProperty.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 07 Apr 2018 01:06:08 GMT
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
    public struct SkillProperty
    {
        [XmlAttribute]
        public int[] skills;
    }
}
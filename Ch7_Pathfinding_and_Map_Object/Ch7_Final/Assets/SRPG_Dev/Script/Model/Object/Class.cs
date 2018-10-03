#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Class.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 08:32:14 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Maps;

    public class Class
    {
        private ClassInfo m_ClassInfo;

        private MoveConsumption m_MoveConsumption;

        public MoveConsumption moveConsumption
        {
            get { return m_MoveConsumption; }
        }

        public Class(int classId)
        {
            // TODO Load from config file
            m_ClassInfo = new ClassInfo()
            {
                movePoint = 9f
            };
            m_MoveConsumption = new MoveConsumption(ClassType.Knight1);
        }

        // TODO other
    }
}
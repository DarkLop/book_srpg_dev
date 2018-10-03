#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MoveConsume.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 23:59:32 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;

namespace DR.Book.SRPG_Dev.Models.Old
{
    using DR.Book.SRPG_Dev.Maps;

    public class MoveConsume : IEnumerable<int>
    {
        private MoveConsumeInfo m_Info;

        public ClassType classType
        {
            get { return m_Info.type; }
        }

        public int this[TerrainType terrainType]
        {
            get { return m_Info.consumes[(int)terrainType]; }
        }

        public MoveConsume(MoveConsumeInfo consume)
        {
            if (consume == null)
            {
                throw new ArgumentNullException("consume is null.");
            }
            m_Info = consume;
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (int consume in m_Info.consumes)
            {
                yield return consume;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Info.consumes.GetEnumerator();
        }
    }
}
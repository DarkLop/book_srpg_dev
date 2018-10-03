#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Class.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 07 Apr 2018 00:06:06 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models.Old
{
    public class Class
    {
        private ClassInfo m_Info;
        private MoveConsume m_Consume;

        public int id
        {
            get { return m_Info.id; }
        }

        public string prefab
        {
            get { return m_Info.prefab; }
        }

        public string animator
        {
            get { return m_Info.animator; }
        }

        public ClassType classType
        {
            get { return m_Info.classType; }
        }

        public int rankUpLevel
        {
            get { return m_Info.rankUpLevel; }
        }

        public MoveConsume consume
        {
            get { return m_Consume; }
        }

        public FightProperty fightProperty
        {
            get { return m_Info.property; }
        }

        public FightProperty maxFightProperty
        {
            get { return m_Info.maxProperty; }
        }

        public int movePoint
        {
            get { return m_Info.movePoint; }
        }

        public Class(ClassInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("class info is null.");
            }
            m_Info = info;
            RoleModel model = ModelManager.models.Get<RoleModel>();
            m_Consume = model.GetMoveConsume(m_Info.classType);
        }
    }
}
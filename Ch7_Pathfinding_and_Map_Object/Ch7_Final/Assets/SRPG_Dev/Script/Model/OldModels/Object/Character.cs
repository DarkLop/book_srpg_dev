#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Character.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 07 Apr 2018 00:39:12 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models.Old
{
    public class Character
    {
        private CharacterInfo m_Info;

        public int id
        {
            get { return m_Info.id; }
        }

        public string profile
        {
            get { return m_Info.profile; }
        }

        public int hp
        {
            get { return m_Info.hp; }
        }

        public FightProperty fightProperty
        {
            get { return m_Info.property; }
        }

        public int luk
        {
            get { return m_Info.luk; }
        }

        public int money
        {
            get { return m_Info.money; }
        }

        public int classId
        {
            get { return m_Info.classId; }
        }

        public Character(CharacterInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("Character Info is null.");
            }

            m_Info = info;
        }
    }
}
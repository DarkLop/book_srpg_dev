#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Role.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 09:47:05 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.Models
{
    public class Role
    {
        private Character m_Character;
        private Class m_CharacterClass;

        public Character character
        {
            get { return m_Character; }
        }

        public Class characterClass
        {
            get { return m_CharacterClass; }
        }

        public void Load(int characterId)
        {
            m_Character = new Character(characterId);
            m_CharacterClass = new Class(0);
        }
    }
}
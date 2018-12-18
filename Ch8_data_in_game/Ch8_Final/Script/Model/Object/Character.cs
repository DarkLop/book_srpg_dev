#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Character.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 09:45:05 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------


namespace DR.Book.SRPG_Dev.Models
{
    public class Character
    {
        public CharacterInfo info { get; private set; }

        public Character(CharacterInfo info)
        {
            this.info = info;
        }
    }
}
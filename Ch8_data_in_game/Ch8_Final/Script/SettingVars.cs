#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ConstVars.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 02 Nov 2018 15:46:17 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev
{
    public static class SettingVars
    {
        public const int k_LevelUpExp = 100;
        public const int k_RoleItemCount = 7;

        private static int s_MaxLevel = 30;
        private static int s_MaxHp = 80;
        private static int s_MaxMp = 80;
        private static int s_MaxLuk = 30;

        public static int maxLevel
        {
            get { return s_MaxLevel; }
            set { s_MaxLevel = Mathf.Max(1, value); }
        }

        public static int maxHp
        {
            get { return s_MaxHp; }
            set { s_MaxHp = Mathf.Max(1, value); }
        }

        public static int maxMp
        {
            get { return s_MaxMp; }
            set { s_MaxMp = Mathf.Max(0, value); }
        }

        public static int maxLuk
        {
            get { return s_MaxLuk; }
            set { s_MaxLuk = Mathf.Max(0, value); }
        }
    }
}
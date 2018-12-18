#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				AttitudeTowards.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 11 Oct 2018 18:33:10 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev
{
    [Serializable]
    public enum AttitudeTowards
    {
        /// <summary>
        /// 玩家
        /// </summary>
        Player,

        /// <summary>
        /// 敌人
        /// </summary>
        Enemy,

        /// <summary>
        /// 盟友
        /// </summary>
        Ally,

        /// <summary>
        /// 中立
        /// </summary>
        Neutral
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MoveConsumptionInfo.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 08:22:42 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    [Serializable]
    public class MoveConsumptionInfo
    {
        /// <summary>
        /// 职业类型
        /// </summary>
        public ClassType type;

        /// <summary>
        /// 移动消耗具体数值
        /// </summary>
        public float[] consumptions;
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Direction.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 01 Feb 2018 23:51:19 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Maps
{
    [Serializable]
    [Flags]
    public enum Direction : byte
    {
        Down = 0x01,
        Right = 0x02,
        Up = 0x04,
        Left = 0x08,
        Cross = Down | Right | Up | Left
    }
}
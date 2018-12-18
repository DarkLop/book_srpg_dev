#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IRuntimeData.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 14 Oct 2018 14:07:46 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    public interface IRuntimeData<T> : ICloneable where T : class, IRuntimeData<T>, new()
    {
        void CopyTo(T data);
        new T Clone();
    }
}
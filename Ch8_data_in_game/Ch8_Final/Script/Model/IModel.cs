#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IModel.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 16:08:53 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    public interface IModel : IDisposable
    {
        void Load();
    }
}
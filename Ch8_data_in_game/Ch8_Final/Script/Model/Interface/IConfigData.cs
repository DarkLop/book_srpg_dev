#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IConfigData.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 05 Oct 2018 13:57:55 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.Models
{
    public interface IConfigData<TKey>
    {
        TKey GetKey();
    }
}
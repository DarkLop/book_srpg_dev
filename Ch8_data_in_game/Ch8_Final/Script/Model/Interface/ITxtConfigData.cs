#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ITxtConfigData.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 08 Oct 2018 00:01:13 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.Models
{
    public interface ITxtConfigData<TKey> : IConfigData<TKey>
    {
        bool FormatText(string line);
    }
}
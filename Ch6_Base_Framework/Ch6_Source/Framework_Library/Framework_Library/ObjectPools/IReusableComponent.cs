#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IReusableComponent.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 18:57:40 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------


namespace DR.Book.SRPG_Dev.Framework
{
    public interface IReusableComponent
    {
        void OnSpawn();
        void OnDespawn();
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MessageControllerBase.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 16:16:27 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------


namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class MessageControllerBase
    {
        public abstract void ExecuteMessage(string message, object sender, MessageArgs messageArgs, params object[] messageParams);
    }
}
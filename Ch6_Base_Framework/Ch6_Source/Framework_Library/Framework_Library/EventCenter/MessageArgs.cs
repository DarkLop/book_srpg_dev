#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MessageArgs.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 15:59:53 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class MessageArgs : EventArgs, IDisposable
    {
        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {

        }
    }
}
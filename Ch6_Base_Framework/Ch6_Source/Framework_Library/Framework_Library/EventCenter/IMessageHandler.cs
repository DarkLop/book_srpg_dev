#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IMessageHandler.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 15:56:43 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    public interface IMessageHandler
    {
        void ExecuteMessage(string message, object sender, MessageArgs messageArgs, params object[] messageParams);
    }
}
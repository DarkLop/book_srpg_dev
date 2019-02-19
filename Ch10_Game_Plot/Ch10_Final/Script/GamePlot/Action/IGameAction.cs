#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				IGameAction.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 05 Jan 2019 21:54:39 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public interface IGameAction : IDisposable
    {
        /// <summary>
        /// 是否打印信息
        /// </summary>
        bool debugInfo { get; set; }

        /// <summary>
        /// 上一个Action
        /// </summary>
        IGameAction previous { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        string error { get; }

        #region Input Methods
        void OnMouseMove(Vector3 mousePOsition);
        void OnMouseLButtonDown(Vector3 mousePosition);
        void OnMouseLButtonUp(Vector3 mousePosition);
        void OnMouseRButtonDown(Vector3 mousePosition);
        void OnMouseRButtonUp(Vector3 mousePosition);
        void OnKeyDown(object key);
        #endregion

        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();

        /// <summary>
        /// 重启
        /// </summary>
        void Resume();

        /// <summary>
        /// 每帧运行：true继续运行，false终止运行。
        /// </summary>
        /// <returns></returns>
        bool Update();

        /// <summary>
        /// 计时器到时的操作
        /// </summary>
        /// <param name="id"></param>
        void TimerTimeout(int timerId);

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <param name="abortParams"></param>
        void Abort(params object[] abortParams);
    }

    public delegate void OnGameActionDelegate(IGameAction action, params object[] actionParams);
    public delegate void OnTimerTimeout(int timerId);
}
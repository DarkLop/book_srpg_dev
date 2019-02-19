#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				GameAction.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 05 Jan 2019 19:12:09 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    [Serializable]
    public class GameAction : IGameAction
    {
        #region Static
        public static List<Type> GetDefaultExecutorTypesForScenarioAction()
        {
            return new List<Type>()
            {
                typeof(DebugExecutor), typeof(LoadExecutor),
                typeof(VarExecutor), typeof(CalcExecutor),
                typeof(GotoExecutor), typeof(IfGotoExecutor),
                typeof(TextExecutor), typeof(MenuExecutor),
                typeof(ClearExecutor), typeof(EndExecutor),
                typeof(BackExecutor), typeof(BattleExecutor)
            };
        }

        public static List<Type> GetDefaultExecutorTypesForScenarioActionInMap()
        {
            return new List<Type>()
            {
                typeof(DebugExecutor), typeof(LoadExecutor),
                typeof(VarExecutor), typeof(CalcExecutor),
                typeof(GotoExecutor), typeof(IfGotoExecutor),
                typeof(TextExecutor), typeof(MenuExecutor),
                typeof(ClearExecutor), typeof(EndExecutor),
                typeof(BackExecutor)
            };
        }
        #endregion

        #region Events
        public event OnGameActionDelegate onAbort;
        #endregion

        #region Fields
        [SerializeField]
        private bool m_DebugInfo = false;

        private IGameAction m_Previous = null;
        protected string m_Error = null;
        #endregion

        #region Properties
        /// <summary>
        /// Debug.Log message
        /// </summary>
        public bool debugInfo
        {
            get { return m_DebugInfo; }
            set { m_DebugInfo = value; }
        }

        /// <summary>
        /// Parent action
        /// </summary>
        public IGameAction previous
        {
            get { return m_Previous; }
            protected set { m_Previous = value; }
        }

        /// <summary>
        /// Error
        /// </summary>
        public string error
        {
            get { return m_Error; }
            protected set { m_Error = value; }
        }
        #endregion

        #region Constructor
        public GameAction()
        {

        }

        public GameAction(IGameAction previous)
        {
            m_Previous = previous;
        }
        #endregion

        #region Input Event Methods
        public virtual void OnMouseMove(Vector3 mousePosition)
        {

        }

        public virtual void OnMouseLButtonDown(Vector3 mousePosition)
        {

        }

        public virtual void OnMouseLButtonUp(Vector3 mousePosition)
        {

        }

        public virtual void OnMouseRButtonDown(Vector3 mousePosition)
        {

        }

        public virtual void OnMouseRButtonUp(Vector3 mousePosition)
        {

        }

        public virtual void OnKeyDown(object key)
        {

        }
        #endregion

        #region Pause/Resume Methods
        public virtual void Pause()
        {

        }

        public virtual void Resume()
        {

        }
        #endregion

        #region Running Methods
        public virtual bool Update()
        {
            return true;
        }

        public virtual void TimerTimeout(int timerId)
        {

        }

        public virtual void Abort(params object[] abortParams)
        {
            InvokeOnAbort(abortParams);
        }

        protected void InvokeOnAbort(params object[] abortParams)
        {
            if (onAbort != null)
            {
                onAbort(this, abortParams);
            }
        }

        #endregion

        #region Helper Methods
        protected void DebugLog(LogType type, object message)
        {
            if (!m_DebugInfo)
            {
                return;
            }

            switch (type)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                case LogType.Exception:
                    if (message is Exception)
                    {
                        Debug.LogException(message as Exception);
                    }
                    break;
            }
        }

        protected void DebugLogFormat(LogType type, string format, params object[] args)
        {
            if (!m_DebugInfo)
            {
                return;
            }

            switch (type)
            {
                case LogType.Log:
                    Debug.LogFormat(format, args);
                    break;
                case LogType.Warning:
                    Debug.LogWarningFormat(format, args);
                    break;
                case LogType.Error:
                case LogType.Exception:
                    Debug.LogErrorFormat(format, args);
                    break;
                case LogType.Assert:
                    Debug.LogAssertionFormat(format, args);
                    break;
            }
        }
        #endregion

        #region IDisposable
        public virtual void Dispose()
        {
            m_DebugInfo = false;
            m_Previous = null;
            m_Error = null;
            onAbort = null;
        }
        #endregion

    }
}
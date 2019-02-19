#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				ScenarioAction.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 03 Jan 2019 23:24:20 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    using DR.Book.SRPG_Dev.Framework;
    using DR.Book.SRPG_Dev.UI;

    [Serializable]
    public class ScenarioAction : GameAction,
        IScenarioAction,
        IScenarioContentExecutorContainer
    {
        #region Fields
        private IScenario m_Scenario = null;
        private int m_Token = 0;
        private ActionStatus m_Status = ActionStatus.Error;

        private SetFlagExecutor m_SetFlagExecutor = new SetFlagExecutor();
        private readonly Dictionary<string, IScenarioContentExecutor> m_ExecutorDict
            = new Dictionary<string, IScenarioContentExecutor>();

        private readonly Dictionary<string, int> m_FlagDict = new Dictionary<string, int>();
        #endregion

        #region Properties
        /// <summary>
        /// 当前剧本
        /// </summary>
        public IScenario scenario
        {
            get { return m_Scenario; }
            protected set { m_Scenario = value; }
        }

        /// <summary>
        /// 当前运行标识
        /// </summary>
        public int token
        {
            get { return m_Token; }
            protected set { m_Token = value; }
        }

        /// <summary>
        /// 当前运行状态
        /// </summary>
        public ActionStatus status
        {
            get { return m_Status; }
            protected set { m_Status = value; }
        }

        /// <summary>
        /// 设置标识符
        /// </summary>
        public SetFlagExecutor setFlagExecutor
        {
            get { return m_SetFlagExecutor; }
            protected set { m_SetFlagExecutor = value; }
        }
        #endregion

        #region Constructor
        public ScenarioAction() : base()
        {

        }

        public ScenarioAction(IGameAction previous) : base(previous)
        {

        }
        #endregion

        #region IScenarioContentExecutorContainer Load Methods
        /// <summary>
        /// 初始化Executor
        /// </summary>
        /// <param name="executorTypes"></param>
        public void LoadExecutors(params Type[] executorTypes)
        {
            if (executorTypes == null && executorTypes.Length == 0)
            {
                return;
            }

            ScenarioContentExecutor.GetOrCreateInstances(
                executorTypes,
                (i, executor) =>
                {
                    if (m_ExecutorDict.ContainsKey(executor.code))
                    {
                        Debug.LogWarningFormat(
                            "{0} -> LoadExecutors: the code `{1}` of executor `{2}` in {3} is already exist. OVERRIDE!.",
                            GetType().Name,
                            executor.code,
                            executorTypes[i].Name,
                            i);
                    }
                    SetExecutor(executor, true);
                },
                triggerError: (i, error) =>
                {
                    Debug.LogWarningFormat("{0} - {1}", i, error);
                },
                fromCache: true,
                ignorEmpty: true);
        }
        #endregion

        #region IScenarioContentExecutorContainer Other Methods
        /// <summary>
        /// 添加或设置Executor
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="existOverride"></param>
        /// <returns></returns>
        public bool SetExecutor(IScenarioContentExecutor executor, bool existOverride = true)
        {
            if (executor == null)
            {
                return false;
            }

            if (executor.code == null)
            {
                return false;
            }

            if (!existOverride && m_ExecutorDict.ContainsKey(executor.code))
            {
                return false;
            }

            m_ExecutorDict[executor.code] = executor;
            return true;
        }

        /// <summary>
        /// 获取Executor
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IScenarioContentExecutor GetExecutor(string code)
        {
            if (code == null)
            {
                return null;
            }

            IScenarioContentExecutor executor;
            if (!m_ExecutorDict.TryGetValue(code, out executor))
            {
                return null;
            }
            return executor;
        }

        public IScenarioContentExecutor[] GetAllExecutors()
        {
            return m_ExecutorDict.Values.ToArray();
        }

        /// <summary>
        /// 移除Executor
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool RemoveExecutor(string code)
        {
            return m_ExecutorDict.Remove(code);
        }

        /// <summary>
        /// 清空Executor
        /// </summary>
        public void ClearExecutors()
        {
            m_ExecutorDict.Clear();
        }
        #endregion

        #region Load Scenario Methods
        public bool LoadScenario(IScenario scenario)
        {
            if (scenario == null || !scenario.isLoaded)
            {
                error = string.Format(
                    "{0} -> LoadScenario: `scenario` is null, or `scenario` is not loaded.",
                    GetType().Name);
                return false;
            }

            this.scenario = scenario;
            this.status = ActionStatus.Continue;
            this.token = 0;
            this.m_FlagDict.Clear();

            return true;
        }

        public void ClearFlags()
        {
            m_FlagDict.Clear();
        }
        #endregion

        #region Running Methods
        /// <summary>
        /// 每帧运行
        /// </summary>
        /// <returns></returns>
        public override bool Update()
        {
            if (status == ActionStatus.Continue)
            {
                // 执行每一条命令，直到状态不为Continue
                do
                {
                    status = Step();
                } while (status == ActionStatus.Continue);
            }

            // 如果出错了，就中断
            if (status == ActionStatus.Error)
            {
                Abort();
                return false;
            }
            // 等待下一帧
            else if (status == ActionStatus.NextFrame)
            {
                status = ActionStatus.Continue;
            }
            // 返回上一个Action
            else if (status == ActionStatus.BackAction)
            {
                BackAction();
                status = ActionStatus.NextFrame;
            }

            return true;
        }

        /// <summary>
        /// 执行每一条命令
        /// </summary>
        /// <returns></returns>
        private ActionStatus Step()
        {
            if (token >= scenario.contentCount)
            {
                error = string.Format(
                    "{0} -> Step: scenario running end.",
                    GetType().Name);
                return ActionStatus.Error;
            }

            // 获取命令
            IScenarioContent content = scenario.GetContent(token);
            DebugLogFormat(LogType.Log,
                "Step {0}: {1}",
                token,
                content.ToString());

            IScenarioContentExecutor executor;
            // 如果是标识符，设置标识符
            if (content.type == ScenarioContentType.Flag)
            {
                executor = setFlagExecutor;
            }
            // 如果是动作，执行动作
            else
            {
                executor = GetExecutor(content.code);
                if (executor == null)
                {
                    error = string.Format(
                        "{0} -> Step: executor `{1}` was not found.",
                        GetType().Name,
                        content.code);
                    return ActionStatus.Error;
                }
            }

            token++;
            ActionStatus result = executor.Execute(this, content, out m_Error);
            return result;
        }

        /// <summary>
        /// 计时器操作
        /// </summary>
        /// <param name="timerId"></param>
        public override void TimerTimeout(int timerId)
        {
            if (status == ActionStatus.WaitTimerTimeout)
            {
                status = ActionStatus.NextFrame;
            }
        }

        /// <summary>
        /// 中断并打印错误
        /// </summary>
        public override void Abort(params object[] abortParams)
        {
            DebugLogFormat(LogType.Error,
                "{0}: {1}",
                token.ToString(),
                error);

            InvokeOnAbort(abortParams);
        }

        protected virtual void BackAction()
        {
            GameDirector.instance.BackGameAction();
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// 检查并设置剧情标识符
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public ActionStatus SetFlagCommand(string flag, out string cmdError)
        {
            int index;
            if (m_FlagDict.TryGetValue(flag, out index))
            {
                // 如果已经存在的标识符重名，并且值不等，那么说明标识符不唯一
                if (index != token)
                {
                    cmdError = string.Format(
                        "{0} -> SetFlagCommand: flag `{1}` is already exist.",
                        GetType().Name,
                        flag);
                    return ActionStatus.Error;
                }
            }
            else
            {
                m_FlagDict.Add(flag, token);
            }

            cmdError = null;
            return ActionStatus.Continue;
        }

        /// <summary>
        /// 将剧本跳转到Flag
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public ActionStatus GotoCommand(string flag, out string cmdError)
        {
            int index;
            if (!m_FlagDict.TryGetValue(flag, out index))
            {
                // 向后查找flag
                while (token < scenario.contentCount)
                {
                    IScenarioContent content = scenario.GetContent(token);
                    token++;
                    if (content.type != ScenarioContentType.Flag)
                    {
                        continue;
                    }

                    // 向后查找时，将新的剧情标识符加入到字典中
                    if (setFlagExecutor.Execute(this, content, out cmdError) == ActionStatus.Error)
                    {
                        return ActionStatus.Error;
                    }

                    // 是我们需要的剧情标识符
                    if (flag == content.code)
                    {
                        return ActionStatus.Continue;
                    }
                }

                // 没有搜索到
                cmdError = string.Format(
                    "{0} GotoCommand error: flag `{1}` was not found.",
                    GetType().Name,
                    flag);
                return ActionStatus.Error;
            }

            token = index;
            cmdError = null;
            return ActionStatus.Continue;
        }

        /// <summary>
        /// 读取场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="cmdError"></param>
        /// <returns></returns>
        public ActionStatus LoadSceneCommand(string sceneName, out string cmdError)
        {
            MessageCenter.AddListener(GameMain.k_Event_OnSceneLoaded, LoadScene_OnSceneLoaded);
            GameMain.instance.LoadSceneAsync(sceneName);
            cmdError = null;
            return ActionStatus.WaitTimerTimeout;
        }

        private void LoadScene_OnSceneLoaded(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            MessageCenter.RemoveListener(GameMain.k_Event_OnSceneLoaded, LoadScene_OnSceneLoaded);

            OnSceneLoadedArgs args = messageArgs as OnSceneLoadedArgs;

            ScenarioBlackboard.lastScenarioScene = args.scene.name;
            TimerTimeout(-1);
        }
        #endregion

        #region Input Methods
        public override void OnMouseLButtonDown(Vector3 mousePosition)
        {
            if (status == ActionStatus.WaitWriteTextDone)
            {
                WriteTextDone();
                return;
            }
        }
        #endregion

        #region Status Methods
        public void WriteTextDone()
        {
            if (status == ActionStatus.WaitWriteTextDone)
            {
                UITextPanel panel = UIManager.views.GetView<UITextPanel>(UINames.k_UITextPanel);
                if (panel.isWriting)
                {
                    panel.WriteTextImmediately();
                }
                else
                {
                    panel.HideIcon();
                    status = ActionStatus.Continue;
                }
            }
        }

        public void MenuDone()
        {
            if (status == ActionStatus.WaitMenuOption)
            {
                status = ActionStatus.NextFrame;
            }
        }

        public void BattleMapDone(string error = null)
        {
            if (status == ActionStatus.WaitMapDone)
            {
                if (string.IsNullOrEmpty(error))
                {
                    status = ActionStatus.NextFrame;
                }
                else
                {
                    this.error = error;
                    status = ActionStatus.Error;
                }
            }
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            base.Dispose();

            m_Scenario = null;
            m_Status = ActionStatus.Error;
            m_Token = 0;
            m_SetFlagExecutor = null;
            m_ExecutorDict.Clear();
            m_FlagDict.Clear();
        }
        #endregion
    }
}
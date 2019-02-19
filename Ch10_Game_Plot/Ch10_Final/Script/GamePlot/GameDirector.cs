#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				GameDirector.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 06 Jan 2019 12:03:58 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    using DR.Book.SRPG_Dev.Framework;
    using DR.Book.SRPG_Dev.Models;

    public class GameDirector : UnitySingleton<GameDirector>
    {
        #region Fields
        [SerializeField]
        private bool m_DebugInfo = true;
        [SerializeField]
        private string m_FirstScenario = "main";
        [SerializeField]
        private bool m_FirstScenarioIsTxt = true;

        private IGameAction m_GameAction = null;
        private Coroutine m_Coroutine = null;
        #endregion

        #region Properties
        /// <summary>
        /// 首次运行游戏的剧本名称
        /// </summary>
        public string firstScenario
        {
            get { return m_FirstScenario; }
            set { m_FirstScenario = value; }
        }

        /// <summary>
        /// 是否打印信息
        /// </summary>
        public bool debugInfo
        {
            get { return m_DebugInfo; }
            set
            {
                m_DebugInfo = value;
                if (currentAction != null)
                {
                    currentAction.debugInfo = value;
                }
            }
        }

        /// <summary>
        /// 首次运行的剧本是否是txt文件，否则是xml文件
        /// </summary>
        public bool firstScenarioIsTxt
        {
            get { return m_FirstScenarioIsTxt; }
            set { m_FirstScenarioIsTxt = value; }
        }

        /// <summary>
        /// 当前Action
        /// </summary>
        public IGameAction currentAction
        {
            get { return m_GameAction; }
            protected set { m_GameAction = value; }
        }
        #endregion

        #region Unity Callback
        protected virtual void Update()
        {
            if (currentAction == null)
            {
                return;
            }

            currentAction.OnMouseMove(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                currentAction.OnMouseLButtonDown(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                currentAction.OnMouseLButtonUp(Input.mousePosition);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                currentAction.OnMouseRButtonDown(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                currentAction.OnMouseRButtonUp(Input.mousePosition);
            }
        }
        #endregion

        #region Load Scenario Methods
        /// <summary>
        /// 开始首个剧本
        /// </summary>
        /// <returns></returns>
        public bool LoadFirstScenario()
        {
            return LoadScenario(firstScenario, firstScenarioIsTxt);
        }

        /// <summary>
        /// 读取剧本
        /// </summary>
        /// <param name="scriptName"></param>
        /// <returns></returns>
        public bool LoadScenario(string scriptName, bool txt = true)
        {
            ScenarioModel model = ModelManager.models.Get<ScenarioModel>();
            IScenario scenario = model.Get(scriptName, txt);
            if (scenario == null)
            {
                return false;
            }

            ScenarioAction action = new ScenarioAction(currentAction)
            {
                debugInfo = debugInfo
            };

            Type[] executorTypes = GameAction.GetDefaultExecutorTypesForScenarioAction().ToArray();
            action.LoadExecutors(executorTypes);

            if (!action.LoadScenario(scenario))
            {
                Debug.LogError(action.error);
                action.Dispose();
                return false;
            }

            currentAction = action;

            if (debugInfo)
            {
                Debug.LogFormat("GameDirector Load Scenario: {0}", scenario.name);
            }

            return true;
        }

        /// <summary>
        /// 读取地图
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public bool LoadMap(string sceneName)
        {
            MessageCenter.AddListener(GameMain.k_Event_OnSceneLoaded, LoadMap_OnSceneLoaded);
            GameMain.instance.LoadSceneAsync(sceneName);
            return true;
        }

        public void LoadMap_OnSceneLoaded(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            MessageCenter.RemoveListener(GameMain.k_Event_OnSceneLoaded, LoadMap_OnSceneLoaded);

            StopGameAction();
            if (debugInfo)
            {
                Debug.LogFormat("GameDirector Load Map Script: {0}", ScenarioBlackboard.mapScript);
            }

            // TODO
            /*
            MapAction action = new MapAction(currentAction)
            {
                debugInfo = debugInfo
            };

            if (!action.Load(ScenarioBlackboard.mapScript))
            {
                Debug.LogError(action.error);
                action.Dispose();
                return;
            }
            */

            currentAction.Pause();
            //currentAction = action;
        }
        #endregion

        #region Running Scenario Methods
        /// <summary>
        /// 开始运行
        /// </summary>
        public void RunGameAction()
        {
            m_Coroutine = StartCoroutine(RunningGameAction());
        }

        /// <summary>
        /// 运行中
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunningGameAction()
        {
            yield return null;

            while (true)
            {
                if (currentAction == null)
                {
                    yield return null;
                    continue;
                }

                if (currentAction.Update())
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            m_Coroutine = null;
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        public void StopGameAction()
        {
            if (m_Coroutine == null)
            {
                return;
            }

            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }

        /// <summary>
        /// 运行到最后了
        /// </summary>
        public void GameActionRunOver()
        {
            StopGameAction();

            while (currentAction != null)
            {
                BackGameAction();
            }
        }

        /// <summary>
        /// 返回上一个action
        /// </summary>
        public void BackGameAction()
        {
            IGameAction old = currentAction;
            currentAction = currentAction.previous;
            old.Dispose();

            if (currentAction == null)
            {
                GameMain.instance.LoadScene(GameMain.instance.startScene);
            }
        }
        #endregion

        #region Timer Method
        public void StartTimer(int timerId, float wait, OnTimerTimeout onTimerTimeout)
        {
            if (wait < 0)
            {
                if (onTimerTimeout != null)
                {
                    onTimerTimeout(timerId);
                }
                return;
            }

            StartCoroutine(TimerRunning(timerId, wait, onTimerTimeout));
        }

        private IEnumerator TimerRunning(int timerId, float wait, OnTimerTimeout onTimerDone)
        {
            yield return new WaitForSeconds(wait);
            if (onTimerDone != null)
            {
                onTimerDone(timerId);
            }
        }
        #endregion
    }
}
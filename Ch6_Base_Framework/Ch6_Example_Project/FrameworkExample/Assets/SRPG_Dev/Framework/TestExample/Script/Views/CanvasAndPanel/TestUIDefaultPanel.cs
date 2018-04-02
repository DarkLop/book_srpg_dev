#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestUIDefaultPanel.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 22:21:31 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    /// <summary>
    /// 测试用全局面板
    /// </summary>
	public class TestUIDefaultPanel : UIBase 
	{
        #region UI Field
        private Button m_AddAddSceneBtn;
        private Button m_LoadStageSceneBtn;
        private Button m_DestroyTestUICanvasBtn;
        private Button m_DebugAllUIBtn;
        private Button m_DebugAllConfigBtn;
        private Text m_ActiveSceneTxt;
        #endregion

        #region Field
        private bool m_IsAddScene;
        #endregion

        #region Load/Destroy Callback
        protected override IEnumerator OnLoadingView()
        {
            m_AddAddSceneBtn = transform.Find("ButtonLayout/AddAddSceneBtn").GetComponent<Button>();
            m_LoadStageSceneBtn = transform.Find("ButtonLayout/LoadStageSceneBtn").GetComponent<Button>();
            m_DestroyTestUICanvasBtn = transform.Find("ButtonLayout/DestroyTestUICanvasBtn").GetComponent<Button>();
            m_DebugAllUIBtn = transform.Find("ButtonLayout/DebugAllUIBtn").GetComponent<Button>();
            m_DebugAllConfigBtn = transform.Find("ButtonLayout/DebugAllConfigBtn").GetComponent<Button>();
            m_ActiveSceneTxt = transform.Find("ActiveSceneTxt").GetComponent<Text>();

            m_AddAddSceneBtn.onClick.AddListener(AddAddSceneBtn_onClick);
            m_LoadStageSceneBtn.onClick.AddListener(LoadStageSceneBtn_onClick);
            m_DestroyTestUICanvasBtn.onClick.AddListener(DestroyTestUICanvasBtn_onClick);
            m_DebugAllUIBtn.onClick.AddListener(DebugAllUIBtn_onClick);
            m_DebugAllConfigBtn.onClick.AddListener(DebugAllConfigBtn_onClick);
            m_ActiveSceneTxt.text = SceneManager.GetActiveScene().name;

            m_IsAddScene = false;

            firstSelected = m_AddAddSceneBtn.gameObject;

            MessageCenter.AddListener(TestGameMain.k_Event_OnActiveSceneChanged, Listener_onActiveSceneChanged);
            MessageCenter.AddListener(TestGameMain.k_Event_OnLoadScene, Listener_onLoadScene);
            MessageCenter.AddListener(TestGameMain.k_Event_OnSceneUnloaded, Listener_onSceneUnloaded);

            yield return null;
            OnFocus();
        }

        protected override void OnDestroying()
        {
            MessageCenter.RemoveListener(TestGameMain.k_Event_OnActiveSceneChanged, Listener_onActiveSceneChanged);
            MessageCenter.RemoveListener(TestGameMain.k_Event_OnLoadScene, Listener_onLoadScene);
            MessageCenter.RemoveListener(TestGameMain.k_Event_OnSceneUnloaded, Listener_onSceneUnloaded);
        }
        #endregion

        #region Message Listener Callback Method
        /// <summary>
        /// 改变激活场景事件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="messageArgs"></param>
        /// <param name="messageParams"></param>
        private void Listener_onActiveSceneChanged(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            OnActiveSceneChangedArgs args = messageArgs as OnActiveSceneChangedArgs;
            m_ActiveSceneTxt.text = args.scene2.name;
        }

        /// <summary>
        /// 读取场景之前执行
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="messageArgs"></param>
        /// <param name="messageParams"></param>
        private void Listener_onLoadScene(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            OnLoadSceneArgs args = messageArgs as OnLoadSceneArgs;
            if (args.type == LoadSceneType.SceneName)
            {
                switch (args.sceneName)
                {
                    case TestGameMain.k_TestFrameworkMainSceneName:
                        m_LoadStageSceneBtn.transform.Find("Text").GetComponent<Text>().text = "Load\n 'StageScene'";
                        break;
                    case TestGameMain.k_TestFrameworkStageSceneName:
                        m_LoadStageSceneBtn.transform.Find("Text").GetComponent<Text>().text = "Load\n 'MainScene'";
                        break;
                    case TestGameMain.k_TestFrameworkAddSceneName:
                        m_IsAddScene = true;
                        m_AddAddSceneBtn.transform.Find("Text").GetComponent<Text>().text = "Unload\n 'AddScene'";
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 卸载场景之后执行
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="messageArgs"></param>
        /// <param name="messageParams"></param>
        private void Listener_onSceneUnloaded(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            OnSceneUnloadedArgs args = messageArgs as OnSceneUnloadedArgs;
            if (args.scene.name == TestGameMain.k_TestFrameworkAddSceneName)
            {
                // AddScene中有对象池，每次进入scene都会加入到PoolManager中，
                // 由于ShapePool不是DontDestroy，所以退出AddScene时会被自动从PoolManager删除。
                m_IsAddScene = false;
                m_AddAddSceneBtn.transform.Find("Text").GetComponent<Text>().text = "Add\n 'AddScene'";
                
                // 如果ShapePool是DontDestroy，屏蔽按钮，不能进入该场景。重复的pool会报错。
                // m_AddAddSceneBtn.transform.Find("Text").GetComponent<Text>().text = "Invalid\n Button";
                // m_AddAddSceneBtn.interactable = false;
            }
        }
        #endregion

        #region Button Callback
        private void AddAddSceneBtn_onClick()
        {
            if (!m_IsAddScene)
            {
                // 添加AddScene场景，场景中有一个对象池会自动加入到DontDestroy中
                TestGameMain.instance.LoadScene(TestGameMain.k_TestFrameworkAddSceneName, LoadSceneMode.Additive);
            }
            else
            {
                // 卸载AddScene场景
                TestGameMain.instance.UnloadSceneAsync(TestGameMain.k_TestFrameworkAddSceneName);
            }
        }

        private void LoadStageSceneBtn_onClick()
        {
            Scene active = SceneManager.GetActiveScene();
            if (active.name == TestGameMain.k_TestFrameworkStageSceneName)
            {
                TestGameMain.instance.LoadScene(TestGameMain.k_TestFrameworkMainSceneName);
            }
            else
            {
                // 读取StageScene场景
                TestGameMain.instance.LoadSceneAsync(TestGameMain.k_TestFrameworkStageSceneName);
            }
        }

        private void DestroyTestUICanvasBtn_onClick()
        {
            TestUIManager.views.DestroyView(TestUINames.Canvas_TestUICanvas);
        }

        private void DebugAllUIBtn_onClick()
        {
            Debug.Log(TestUIManager.views.ToString());
            Debug.Log(TestUIManager.views.ToString(true));
            Debug.Log(TestUIManager.views.ToString(false));
        }

        private void DebugAllConfigBtn_onClick()
        {
            TestLogData data = TestLogXmlConfig.GetData(0);
            Debug.LogFormat(
                "{0} -> Data: {1}",
                typeof(TestLogXmlConfig).Name,
                data.ToString()
                );

            data = TestLogXmlDocConfig.GetData(1);
            Debug.LogFormat(
                "{0} -> Data: {1}",
                typeof(TestLogXmlDocConfig).Name,
                data.ToString()
                );

            Debug.LogFormat(
                "{0} -> Data: {1}",
                typeof(TestLogJsonConfig).Name,
                TestLogJsonConfig.Get<TestLogJsonConfig>().data
                );

            Debug.LogFormat(
                "{0} -> Data: {1}",
                typeof(TestLogTxtConfig).Name,
                TestLogTxtConfig.Get<TestLogTxtConfig>().ToString()
                );
        }
        #endregion

    }
}
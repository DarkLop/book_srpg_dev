#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestMessageHandlerAndListener.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 23:38:44 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    public class TestMessageHandlerAndListener : MonoBehaviour, IMessageHandler
    {
        #region Field
        /// <summary>
        /// 监听的Scene名称，用于事件打印
        /// </summary>
        public string m_ListenerSceneName;
        #endregion

        #region Unity Callback
        private void Awake()
        {
            // 将Listener添加进MessageCenter
            MessageCenter.AddListener(TestGameMain.k_Event_OnSceneUnloaded, MessageListener_onSceneUnloaded);

            // 作为Handler添加进MessageCenter
            MessageCenter.RegisterHandler(this);
        }

        private void OnDestroy()
        {
            // 将Listener从MessageCenter删除
            MessageCenter.RemoveListener(TestGameMain.k_Event_OnSceneUnloaded, MessageListener_onSceneUnloaded);

            // 作为Handler从MessageCenter删除
            MessageCenter.UnregisterHandler(this);
        }
        #endregion

        #region Message Listener Method
        private void MessageListener_onSceneUnloaded(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            OnSceneUnloadedArgs args  = messageArgs as OnSceneUnloadedArgs;
            if (args.scene.name == m_ListenerSceneName)
            {
                Debug.LogFormat(
                    "{0}: Listener trigger '{1}' message. '{2}' is unloaded.", 
                    name, 
                    message,
                    m_ListenerSceneName
                    );
            }
        }
        #endregion

        #region Interface IMessageHandler
        void IMessageHandler.ExecuteMessage(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            switch (message)
            {
                case TestGameMain.k_Event_OnLoadScene:
                    {
                        OnLoadSceneArgs args = messageArgs as OnLoadSceneArgs;
                        if (args.type == LoadSceneType.SceneName && args.sceneName == m_ListenerSceneName)
                        {
                            Debug.LogFormat(
                                "{0}: Handler trigger '{1}' message. On load scene '{2}'", 
                                name, 
                                message,
                                args.sceneName
                                );
                        }
                        break;
                    }
                case TestGameMain.k_Event_OnSceneLoaded:
                    {
                        OnSceneLoadedArgs args = messageArgs as OnSceneLoadedArgs;
                        if (args.scene.name == m_ListenerSceneName)
                        {
                            Debug.LogFormat(
                                "{0}: Handler trigger '{1}' message. On scene loaded '{2}'",
                                name,
                                message,
                                args.scene.name
                                );
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

    }
}
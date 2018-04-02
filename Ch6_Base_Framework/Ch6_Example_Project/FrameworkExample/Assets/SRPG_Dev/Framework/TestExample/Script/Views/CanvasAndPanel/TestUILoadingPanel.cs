#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestUILoadingPanel.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 10 Mar 2018 18:22:01 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    /// <summary>
    /// 异步读取场景之前打开的Loading面板，例子只有StageScene用了。
    /// </summary>
    public class TestUILoadingPanel : UIBase
    {
        [SerializeField, Range(0.1f, 5f)]
        private float m_Interval = 0.5f;
        private float m_Time = 0f;

        private Text m_LoadingText;
        private Slider m_ProgressSlider;
        private int m_DotCount = 0;

        private object[] m_Args;

        protected override void OnUpdate()
        {
            m_Time += Time.deltaTime;
            if (m_Time >= m_Interval)
            {
                m_Time -= m_Interval;
                if (m_DotCount >= 3)
                {
                    m_LoadingText.text = m_LoadingText.text.Remove(m_LoadingText.text.Length - 3, 3);
                    m_DotCount = 0;
                }
                else
                {
                    m_LoadingText.text += ".";
                    m_DotCount += 1;
                }
            }
        }

        private void ResetSlider()
        {
            m_LoadingText.text = "Loading";
            m_ProgressSlider.normalizedValue = 0f;
            m_DotCount = 0;
            m_Time = 0f;
        }

        protected override IEnumerator OnLoadingView()
        {
            m_LoadingText = transform.Find("Text").GetComponent<Text>();
            m_ProgressSlider = transform.Find("ProgressSlider").GetComponent<Slider>();

            yield break;
        }

        private void Listener_onSceneLoaded(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            if ((messageArgs as OnSceneLoadedArgs).scene.name == TestGameMain.k_TestFrameworkStageSceneName)
            {
                StartCoroutine(LoadingSceneAsset(m_Args));
            }
        }

        protected override void OnOpen(params object[] args)
        {
            MessageCenter.AddListener(TestGameMain.k_Event_OnSceneLoaded, Listener_onSceneLoaded);
            m_Args = args;
            ResetSlider();
            m_LoadingText.text = "Loading " + (string)m_Args[0];
        }

        protected override void OnClose()
        {
            MessageCenter.RemoveListener(TestGameMain.k_Event_OnSceneLoaded, Listener_onSceneLoaded);
            m_Args = null;
        }

        private IEnumerator LoadingSceneAsset(params object[] args)
        {
            // 等待一帧 'TestSceneLoadedController'
            // 你也可以不用Controller，直接用这个
            yield return null;

            string sceneName = (string)args[0];

            Debug.LogFormat("LoadingPanel: Simulate loading scene '{0}' async.", sceneName);

            m_LoadingText.text = "Loading Map of Stage";
            m_DotCount = 0;
            yield return new WaitForSeconds(3f);
            m_ProgressSlider.normalizedValue = 0.3f;

            m_LoadingText.text = "Loading Objects of Stage";
            m_DotCount = 0;
            for (int i = 0; i < 100; i++)
            {
                m_ProgressSlider.normalizedValue += 0.006f;
                yield return new WaitForSeconds(0.05f);
            }

            m_LoadingText.text = "Wait For Ready";
            m_DotCount = 0;
            yield return new WaitForSeconds(2f);
            m_ProgressSlider.normalizedValue = 1f;

            yield return new WaitForSeconds(1f);
            // 关闭Loading面板
            TestUIManager.views.CloseView(viewName);

            Debug.LogFormat("LoadingPanel: Simulate scene '{0}' end.", sceneName);
        }

    }
}
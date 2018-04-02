#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestFrameworkSceneLoadedController.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 22:29:28 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    /// <summary>
    /// 初始化事件
    /// </summary>
    public class TestGameInitController : MessageControllerBase
    {
        public override void ExecuteMessage(string message, object sender, MessageArgs args, params object[] messageParams)
        {
            InitConfig();
            InitMessageController();
            InitUI();

            // 进入MainScene
            TestGameMain.instance.LoadScene(TestGameMain.k_TestFrameworkMainSceneName);
        }

        /// <summary>
        /// 初始化Config文件
        /// </summary>
        private void InitConfig()
        {
            // 设置Config根目录
            ConfigLoader.rootDirectory = Application.streamingAssetsPath + "/" + TestGameMain.instance.m_ConfigPath;

            // 自动读取Config，获取config中的测试数据
            TestLogData logData0 = TestLogXmlConfig.GetData(0);
            TestLogData logData1 = TestLogXmlConfig.GetData(1);

            // 打印Config中的测试数据
            Debug.Log(logData0);
            Debug.Log(logData1);

            // 这里可以自定义预读取的Config。
            ConfigLoader.LoadConfig(
               typeof(TestLogTxtConfig),
               typeof(TestLogXmlDocConfig),
               typeof(TestLogJsonConfig)
               );
        }

        /// <summary>
        /// 初始化消息事件Controller
        /// </summary>
        private void InitMessageController()
        {
            MessageCenter.RegisterController<TestSceneLoadedController>(TestGameMain.k_Event_OnSceneLoaded);
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitUI()
        {
            // 设置UI的Prefab的根目录
            TestUIManager.prefabDirectory = TestGameMain.instance.m_UIPrefabPath;

            // 寻找外部没有组件的普通Canvas，并设置给UIManager。
            // 你也可以不用动态寻找，而是建立一个GameObject，加上TestUIManager组件，拖入。
            Canvas defaultCanvas = GameObject.FindObjectOfType<Canvas>();
            if (defaultCanvas == null)
            {
                throw new Exception("Default Canvas is not found.");
            }
            defaultCanvas.transform.SetParent(TestUIManager.instance.transform, false);
            TestUIManager.defaultCanvas = defaultCanvas;

            // 将EventSystem也设置成Manager的子物体，就不用每次都建立了
            FrameworkUIUtility.FindOrCreateEventSystem().transform.SetParent(TestUIManager.instance.transform, false);

            // 预实例化UI，你也可以从Config中读取预实例化的UI名字列表
            TestUIManager.views.PreInstantiateView(
                TestUINames.Canvas_TestUICanvas,
                TestUINames.Panel_TestUIDefaultPanel
                );
        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestGameMain.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 22:32:31 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    /// <summary>
    /// 测试Framework主类，继承应用程序入口类
    /// </summary>
    public class TestGameMain : ApplicationEntry<TestGameMain>
    {
        #region Const 场景名称也可以使用Enum枚举
        /// <summary>
        /// 测试初始化场景名称
        /// </summary>
        public const string k_TestFrameworkSceneName = "TestFrameworkScene";

        /// <summary>
        /// 测试主场景名称
        /// </summary>
        public const string k_TestFrameworkMainSceneName = "TestFrameworkMainScene";

        /// <summary>
        /// 测试附加场景名称
        /// </summary>
        public const string k_TestFrameworkAddSceneName = "TestFrameworkAddScene";

        /// <summary>
        /// 测试Stage场景名称
        /// </summary>
        public const string k_TestFrameworkStageSceneName = "TestFrameworkStageScene";
        #endregion

        #region Field
        /// <summary>
        /// Config相对StreamingAssets的根目录
        /// </summary>
        public string m_ConfigPath = "FrameworkTest/Config/";

        /// <summary>
        /// Prefab的Resources路径
        /// </summary>
        public string m_PrefabPath = "TestPrefab/";

        /// <summary>
        /// UIPrefab的Resources路径
        /// </summary>
        public string m_UIPrefabPath = "TestPrefab/UI/";
        #endregion

        #region Unity Callback
        private void Start()
        {
            // 实例化一个测试Handler与Listener事件的对象
            GameObject prefab = Resources.Load<GameObject>(m_PrefabPath + "TestHandlerAndListener");
            GameObject handler = GameObject.Instantiate<GameObject>(prefab);
            handler.transform.SetParent(transform);

            // 注册初始化事件Controller
            MessageCenter.RegisterController<TestGameInitController>(TestEventNames.TestGameInit);
            
            // 发送初始化事件
            this.SendByMessageCenter(TestEventNames.TestGameInit);
        }
        #endregion

        #region Scene Callback
        /// <summary>
        /// 全局场景读取完成事件
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected override IEnumerator OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 等待父类运行（父类里是Resources.UnloadUnusedAssets与GC.Collect）
            yield return base.OnSceneLoaded(scene, mode);

            // 如果是Add Scene，改变激活场景为Add Scene
            if (scene.name == k_TestFrameworkAddSceneName && mode == LoadSceneMode.Additive)
            {
                SetActiveScene(k_TestFrameworkAddSceneName);
            }
        }

        /// <summary>
        /// 全局场景卸载完成事件
        /// </summary>
        /// <param name="scene"></param>
        protected override void OnSceneUnloaded(Scene scene)
        {
            // 当卸载的是初始化场景时
            if (scene.name == k_TestFrameworkSceneName)
            {
                // 注销初始化事件，初始化事件只有游戏打开时执行。
                MessageCenter.UnregisterController(TestEventNames.TestGameInit);
            }
        }

        /// <summary>
        /// 全局读取场景事件，在读取场景之前运行
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <param name="sceneName"></param>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        /// <param name="async"></param>
        protected override void OnLoadScene(int buildIndex, string sceneName, LoadSceneType type, LoadSceneMode mode, bool async)
        {
            // 如果是异步的，打开Loading面板
            if (async)
            {
                TestUIManager.views.OpenViewNotStack(TestUINames.Panel_TestUILoadingPanel, false, sceneName);
            }
        }
        #endregion

    }
}
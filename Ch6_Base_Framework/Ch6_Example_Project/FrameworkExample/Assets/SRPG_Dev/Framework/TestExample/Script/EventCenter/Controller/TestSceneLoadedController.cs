#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestSceneLoadedController.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 23:38:53 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    /// <summary>
    /// 场景读取完成Controller，
    /// 你可以在这里读取每个场景的资源，也可以读取每个场景的AssetBundle，
    /// 如果是AssetBundle，最好写一个AssetBundleManager。
    /// 或者你也可以直接在全局的场景读取完成事件中完成场景资源加载。
    /// 或者在Loading面板执行，请查看LoadingPanel代码。
    /// </summary>
	public class TestSceneLoadedController : MessageControllerBase
    {
        public override void ExecuteMessage(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            OnSceneLoadedArgs args = messageArgs as OnSceneLoadedArgs;
            Debug.Log("TestSceneLoadedController: " + args.scene.name + " Load Scene Assets.");

            // Example 1:
            /*
            SceneData data = SceneDataConfig.GetData(args.scene.name);
            AssetBundleManager.instance.LoadAssetBundle(data.assetBundleInfo, (bundle) =>
            {
                // TODO your code
            });
            */

            // Example 2: 在SceneState的Load方法中完成场景读取
            /*
            SceneState state = SceneState.CreateState(args.scene.name);
            state.Load();
            */

            // 如果是Main Scene，打开默认面板
            if (args.scene.name == TestGameMain.k_TestFrameworkMainSceneName)
            {
                TestUIManager.views.OpenView(TestUINames.Panel_TestUIDefaultPanel, true);
            }

            if (args.scene.name == TestGameMain.k_TestFrameworkAddSceneName)
            {
                TestUIManager.views.OpenView(TestUINames.Panel_TestUIPoolPanel, false);
            }

        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ApplicationEntry.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 15:03:01 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class ApplicationEntry<T> : UnitySingleton<T> where T : ApplicationEntry<T>
    {
        #region Const/Static Field
        public const string k_Event_OnActiveSceneChanged = "Event_OnActiveSceneChanged";
        public const string k_Event_OnSceneLoaded = "Event_OnSceneLoaded";
        public const string k_Event_OnSceneUnloaded = "Event_OnSceneUnloaded";
        public const string k_Event_OnLoadScene = "Event_OnLoadScene";

        private static readonly OnSceneUnloadedArgs s_OnSceneUnloadedArgs = new OnSceneUnloadedArgs();
        private static readonly OnSceneLoadedArgs s_OnSceneLoadedArgs = new OnSceneLoadedArgs();
        private static readonly OnActiveSceneChangedArgs s_OnActiveSceneChangedArgs = new OnActiveSceneChangedArgs();
        private static readonly OnLoadSceneArgs s_OnLoadSceneArgs = new OnLoadSceneArgs();
        #endregion

        #region Field
        [SerializeField, Tooltip("If editor log.")]
        public bool m_EditorLog;
        #endregion

        #region Unity Callback
        protected override void Awake()
        {
            base.Awake();

            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
        }
        #endregion

        #region Scene Callback
        /// <summary>
        /// Unity 激活的场景改变事件.
        /// </summary>
        /// <param name="scene1"></param>
        /// <param name="scene2"></param>
        private void SceneManager_activeSceneChanged(Scene scene1, Scene scene2)
        {
            if (m_EditorLog)
            {
                Debug.LogFormat("Globle -> Active Scene({0} - {1}) is changed to Scene({2} - {3})", scene1.buildIndex, scene1.name, scene2.buildIndex, scene2.name);
            }

            OnActiveSceneChanged(scene1, scene2);

            // 发送场景改变事件
            s_OnActiveSceneChangedArgs.scene1 = scene1;
            s_OnActiveSceneChangedArgs.scene2 = scene2;
            this.SendByMessageCenter(k_Event_OnActiveSceneChanged, s_OnActiveSceneChangedArgs);
        }

        /// <summary>
        /// 用于override的全局激活场景改变事件
        /// </summary>
        /// <param name="scene1"></param>
        /// <param name="scene2"></param>
        protected virtual void OnActiveSceneChanged(Scene scene1, Scene scene2)
        {

        }


        /// <summary>
        /// Unity 场景卸载事件
        /// </summary>
        /// <param name="scene"></param>
        private void SceneManager_sceneUnloaded(Scene scene)
        {
            // -1是UnityEditor的Preview Scene
            if (scene.buildIndex < 0 /*scene.buildIndex == -1*/)
            {
                return;
            }

            if (m_EditorLog)
            {
                Debug.LogFormat("Globle -> Scene({0} - {1}) is unloaded.", scene.buildIndex, scene.name);
            }

            OnSceneUnloaded(scene);

            // 发送场景卸载事件
            s_OnSceneUnloadedArgs.scene = scene;
            this.SendByMessageCenter(k_Event_OnSceneUnloaded, s_OnSceneUnloadedArgs);
        }

        /// <summary>
        /// 用于override的全局场景卸载事件
        /// </summary>
        /// <param name="scene"></param>
        protected virtual void OnSceneUnloaded(Scene scene)
        {

        }

        /// <summary>
        /// Unity 场景读取完成事件，开始异步读取场景.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StartCoroutine(SceneManager_sceneLoadedAsync(scene, mode));
        }

        /// <summary>
        /// 异步场景读取完成事件.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private IEnumerator SceneManager_sceneLoadedAsync(Scene scene, LoadSceneMode mode)
        {
            if (m_EditorLog)
            {
                Debug.LogFormat("Globle -> Scene({0} - {1}) is loaded by mode '{2}'.", scene.buildIndex, scene.name, mode.ToString());
            }

            yield return OnSceneLoaded(scene, mode);

            // 发送场景读取完成事件
            s_OnSceneLoadedArgs.scene = scene;
            s_OnSceneLoadedArgs.mode = mode;
            this.SendByMessageCenter(k_Event_OnSceneLoaded, s_OnSceneLoadedArgs);
        }

        /// <summary>
        /// 用于override的全局异步读取场景完成事件
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        protected virtual IEnumerator OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Additive)
            {
                // 卸载并释放未使用的Unity Object的资源
                yield return Resources.UnloadUnusedAssets();

                // 卸载并释放垃圾，使用GC有时会造成卡顿，慎用
                GC.Collect();
            }
        }
        #endregion

        #region Scene Method
        /// <summary>
        /// 用BuildIndex同步读取场景
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <param name="mode"></param>
        public void LoadScene(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadScene(buildIndex, "Null", LoadSceneType.BuildIndex, mode, false, null);
        }

        /// <summary>
        /// 用SceneName同步读取场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadScene(-2, sceneName, LoadSceneType.SceneName, mode, false, null);
        }

        /// <summary>
        /// 用BuildIndex异步读取场景
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <param name="progress"></param>
        /// <param name="mode"></param>
        public void LoadSceneAsync(int buildIndex, Action<float> progress = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadScene(buildIndex, "Null", LoadSceneType.BuildIndex, mode, true, progress);
        }

        /// <summary>
        /// 用SceneName异步读取场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="progress"></param>
        /// <param name="mode"></param>
        public void LoadSceneAsync(string sceneName, Action<float> progress = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            LoadScene(-2, sceneName, LoadSceneType.SceneName, mode, true, progress);
        }

        /// <summary>
        /// 开始读取场景
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        /// <param name="async"></param>
        /// <param name="progress"></param>
        private void LoadScene(int buildIndex, string sceneName, LoadSceneType type, LoadSceneMode mode, bool async, Action<float> progress)
        {
            if (m_EditorLog)
            {
                Debug.LogFormat(
                    "Globle -> Before Load Scene({0}). Type({1}) Mode({2}) Async({3})",
                    type == LoadSceneType.BuildIndex ? buildIndex.ToString() : sceneName,
                    type.ToString(),
                    mode.ToString(),
                    async.ToString()
                    );
            }

            OnLoadScene(buildIndex, sceneName, type, mode, async);

            // 发送开始读取场景事件
            s_OnLoadSceneArgs.activeScene = SceneManager.GetActiveScene();
            s_OnLoadSceneArgs.buildIndex = buildIndex;
            s_OnLoadSceneArgs.sceneName = sceneName;
            s_OnLoadSceneArgs.type = type;
            s_OnLoadSceneArgs.mode = mode;
            s_OnLoadSceneArgs.async = async;
            this.SendByMessageCenter(k_Event_OnLoadScene, s_OnLoadSceneArgs);

            if (type ==  LoadSceneType.BuildIndex)
            {
                if (async)
                {
                    AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex, mode);
                    StartCoroutine(LoadingOrUnloadingScene(operation, progress));
                }
                else
                {
                    SceneManager.LoadScene(buildIndex, mode);
                }
            }
            else
            {
                if (async)
                {
                    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, mode);
                    StartCoroutine(LoadingOrUnloadingScene(operation, progress));
                }
                else
                {
                    SceneManager.LoadScene(sceneName, mode);
                }
            }
        }

        /// <summary>
        /// 用于override的全局开始读取场景事件，在读取场景之前执行
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <param name="sceneName"></param>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        /// <param name="async"></param>
        protected virtual void OnLoadScene(int buildIndex, string sceneName, LoadSceneType type, LoadSceneMode mode, bool async)
        {

        }

        /// <summary>
        /// 异步读取/卸载场景
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="progress"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private IEnumerator LoadingOrUnloadingScene(AsyncOperation operation, Action<float> progress)
        {
            if (operation == null)
            {
                Debug.LogError("[ApplicationEntry] UNEXPECTED ERROR! Argument named 'operation' is null.");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
                if (progress != null)
                {
                    progress(operation.progress);
                }
            }
        }

        /// <summary>
        /// 如果有多个场景，用BuildIndex改变当前激活的场景
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <returns></returns>
        public bool SetActiveScene(int buildIndex)
        {
            if (SceneManager.sceneCount == 1)
            {
                Debug.LogWarning("You tryed to change the active scene, but there is only one scene.");
                return false;
            }

            Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            return SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 如果有多个场景，用SceneName改变当前激活的场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public bool SetActiveScene(string sceneName)
        {
            if (SceneManager.sceneCount == 1)
            {
                Debug.LogWarning("You tryed to change the active scene, but there is only one scene.");
                return false;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);
            return SceneManager.SetActiveScene(scene);
        }

        /// <summary>
        /// 如果有多个场景，用BuildIndex卸载场景
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <param name="progress"></param>
        public void UnloadSceneAsync(int buildIndex, Action<float> progress = null)
        {
            if (SceneManager.sceneCount == 1)
            {
                Debug.LogErrorFormat("You tryed to unload scene(BuildIndex {0}), but there is only one scene.", buildIndex);
                return;
            }

            Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
            StartCoroutine(LoadingOrUnloadingScene(operation, progress));
        }

        /// <summary>
        /// 如果有多个场景，用SceneName卸载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="progress"></param>
        public void UnloadSceneAsync(string sceneName, Action<float> progress = null)
        {
            if (SceneManager.sceneCount == 1)
            {
                Debug.LogErrorFormat("You tryed to unload scene(SceneName {0}), but there is only one scene.", sceneName);
                return;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);
            AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
            StartCoroutine(LoadingOrUnloadingScene(operation, progress));
        }
        #endregion
    }
}
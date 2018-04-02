#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ViewBase.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 07 Mar 2018 23:23:35 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    [DisallowMultipleComponent]
    public abstract class ViewBase : MonoBehaviour
    {
        #region Field
        private string m_ViewName = "";
        private bool m_IsLoaded = false;
        private string m_PreviousViewName;
        private string m_NextViewName;
        #endregion

        #region Property
        /// <summary>
        /// View的名字
        /// </summary>
        public string viewName
        {
            get { return m_ViewName; }
            private set { m_ViewName = value; }
        }

        /// <summary>
        /// 是否已经LoadViewInternal
        /// </summary>
        public bool isLoaded
        {
            get { return m_IsLoaded; }
            private set { m_IsLoaded = value; }
        }

        /// <summary>
        /// Stack中上一个View的名字
        /// </summary>
        public string previousViewNameInternal
        {
            get { return m_PreviousViewName; }
            internal set { m_PreviousViewName = null; }
        }

        /// <summary>
        /// Stack中下一个View的名字
        /// </summary>
        public string nextViewNameInternal
        {
            get { return m_NextViewName; }
            internal set { m_NextViewName = value; }
        }
        #endregion

        #region Unity Callback
        /// <summary>
        /// 默认为关闭状态
        /// </summary>
        protected void Awake()
        {
            OnAwake();
            Display(false, false);
        }

        protected virtual void OnAwake()
        {

        }

        /// <summary>
        /// 如果Update中，只有在LoadView初始化的物体，请使用OnUpdate()代替。
        /// 如果继承Update：在使用LoadView初始化的物体时，请放入OnUpdate()中，并调用base.Update()，或者自行写代码控制。
        /// </summary>
        protected virtual void Update()
        {
            if (m_IsLoaded)
            {
                OnUpdate();
            }
        }

        protected virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 如果Update中，只有在LoadView初始化的物体，请使用OnUpdate()代替。
        /// 如果继承Update：在使用LoadView初始化的物体时，请放入OnUpdate()中，并调用base.Update()，或者自行写代码控制。
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (m_IsLoaded)
            {
                OnLateUpdate();
            }
        }

        protected virtual void OnLateUpdate()
        {

        }

        /// <summary>
        /// 如果Update中，只有在LoadView初始化的物体，请使用OnUpdate()代替。
        /// 如果继承Update：在使用LoadView初始化的物体时，请放入OnUpdate()中，并调用base.Update()，或者自行写代码控制。
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (m_IsLoaded)
            {
                OnFixedUpdate();
            }
        }

        protected virtual void OnFixedUpdate()
        {

        }

        /// <summary>
        /// 如果在OnDestroy中，只有在LoadView初始化的物体，请使用OnDestroying()代替。
        /// 如果需要与Awake和Start配对，请继承它；
        /// 如果还要使用LoadView中初始化的物体，请放入OnDestroying()中，并调用base.OnDestroy()，或者自行写代码控制。
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (m_IsLoaded)
            {
                OnDestroying();
                m_IsLoaded = false;
            }
        }

        protected virtual void OnDestroying()
        {

        }
        #endregion

        #region Load Method
        /// <summary>
        /// Internal初始化View，由Manager调用。
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="args"></param>
        internal void LoadViewInternal(string viewName)
        {
            if (m_IsLoaded)
            {
                return;
            }

            this.viewName = viewName;
            // 使用外部Coroutine，就算物体在Active false下，也可以进行读取
            CoroutineInstance.instance.StartCoroutine(LoadingView());
        }

        private IEnumerator LoadingView()
        {
            yield return OnLoadingView();
            this.isLoaded = true;
        }

        /// <summary>
        /// 异步初始化View
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual IEnumerator OnLoadingView()
        {
            yield break;
        }
        #endregion

        #region Open/Close
        /// <summary>
        /// 显示/隐藏，
        /// trigger：是否触发OnOpen与OnClose。
        /// </summary>
        /// <param name="open"></param>
        /// <param name="trigger"></param>
        /// <param name="args"></param>
        protected void Display(bool open, bool trigger, params object[] args)
        {
            if (gameObject.activeSelf == open)
            {
                return;
            }

            gameObject.SetActive(open);

            if (trigger)
            {
                if (open)
                {
                    OnOpen(args);
                    StartCoroutine(OnOpenAsync(args));
                }
                else
                {
                    OnClose();
                }
            }
        }

        /// <summary>
        /// 同步设置显示
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnOpen(params object[] args)
        {

        }

        /// <summary>
        /// 异步设置显示
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual IEnumerator OnOpenAsync(params object[] args)
        {
            yield break;
        }

        /// <summary>
        /// 设置关闭
        /// </summary>
        protected virtual void OnClose()
        {

        }

        /// <summary>
        /// 显示，Manager调用
        /// </summary>
        /// <param name="args"></param>
        internal void OpenInternal(params object[] args)
        {
            Display(true, true, args);
        }

        /// <summary>
        /// 隐藏，Manager调用
        /// </summary>
        internal void CloseInternal()
        {
            StopAllCoroutines();
            Display(false, true);
        }
        #endregion
    }
}
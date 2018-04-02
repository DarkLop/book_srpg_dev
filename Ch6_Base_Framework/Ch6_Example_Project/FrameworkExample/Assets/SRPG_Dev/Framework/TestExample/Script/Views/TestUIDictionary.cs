#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestUIDictonary.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 00:12:31 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Reflection;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    public class TestUIDictionary : ViewDictionary
    {
        /// <summary>
        /// panel的父对象，演示自定义外部Canvas
        /// </summary>
        public Canvas m_DefaultCanvas;

        /// <summary>
        /// panel的父对象，内部Canvas，由manager打开的Canvas
        /// </summary>
        private TestUICanvas m_TestUICanvas;

        /// <summary>
        /// 获取View组件类型，
        /// 如果预制体没有组件，那么会调用此方法。
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        protected override Type GetViewComponentType(string viewName)
        {
            switch (viewName)
            {
                case TestUINames.Panel_TestUIDefaultPanel:
                    return typeof(TestUIDefaultPanel);
                case TestUINames.Panel_TestUILoadingPanel:
                    return typeof(TestUILoadingPanel);
                case TestUINames.Panel_TestUIPoolPanel:
                    return typeof(TestUIPoolPanel);
                default:
                    // 默认是反射，需要命名空间。
                    // 为了测试，只有TestUICanavs，用了反射。
                    return Assembly.GetAssembly(typeof(TestUIDictionary)).GetType("DR.Book.SRPG_Dev.Framework.Test." + viewName);
            }
        }

        /// <summary>
        /// 实例化View时调用
        /// </summary>
        /// <param name="view"></param>
        protected override void OnInstantiateView(ViewBase view)
        {
            // 如果是Canvas，父对象设为Manager。
            if (view is TestUICanvas)
            {
                m_TestUICanvas = view as TestUICanvas;
                view.transform.SetParent(gameObject.transform, false);
                //// 这里添加设置Canvas的设置

                //m_TestUICanvas.canvas.renderMode = RenderMode.ScreenSpaceCamera;
                //m_TestUICanvas.canvas.worldCamera = Camera.main; 
                //// 其它设置
            }
            // 如果不是Canvas而是Panel，设置父对象为选择的Canvas。
            else
            {
                // 如果有多个父对象Canvas，
                // 这里可以自定义添加判断设置哪个Canvas为父对象。
                if (view.viewName == TestUINames.Panel_TestUIDefaultPanel)
                {
                    if (m_DefaultCanvas == null)
                    {
                        throw new Exception("Default Canvas is null.");
                    }
                    view.transform.SetParent(m_DefaultCanvas.transform, false);


                    //// 也可以直接使用GetComponent获取组件

                    //TestUIManager manager = gameObject.GetComponent<TestUIManager>();
                    //if (manager.parentCanvas == null)
                    //{
                    //    throw new Exception("Parent Canvas in manager is null.");
                    //}
                    //view.transform.SetParent(manager.parentCanvas.transform);
                }
                else
                {
                    // 如果TestUICanvas没有被打开，就打开它。
                    // 需要注意，因为Canvas只是Panel的一个容器，不能让Canvas加入Stack。
                    if (m_TestUICanvas == null || !m_TestUICanvas.gameObject.activeSelf)
                    {
                        m_TestUICanvas = TestUIManager.views.OpenViewNotStack(TestUINames.Canvas_TestUICanvas, false) as TestUICanvas;
                        if (m_TestUICanvas == null)
                        {
                            throw new Exception("Open Test UI Canvas error.");
                        }
                    }

                    view.transform.SetParent(m_TestUICanvas.transform, false);
                    m_TestUICanvas.childPanelNames.Add(view.viewName);
                }
            }
        }

        /// <summary>
        /// View打开或关闭时调用
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="current"></param>
        /// <param name="open"></param>
        protected override void OnStackViewOpenOrClose(ViewBase previous, ViewBase current, bool open, params object[] openArgs)
        {
            if ((previous != null && !(previous is UIBase)) || (current != null && !(current is UIBase)))
            {
                throw new Exception("You tryed to open/close a view not a ui.");
            }

            // 一定要调用Base
            base.OnStackViewOpenOrClose(previous, current, open);
            // 如果是打开
            if (open)
            {
                if (previous != null)
                {
                    // 将上一个View设置成不可操作
                    (previous as UIBase).canvasGroup.interactable = false;
                }

                // 将打开的View设置成可操作
                (current as UIBase).canvasGroup.interactable = true;
            }
            else
            {
                if (previous != null)
                {
                    // 将上一个View设置成可操作
                    (previous as UIBase).canvasGroup.interactable = true;
                }

                // 将已经关闭的view设置成可操作
                (current as UIBase).canvasGroup.interactable = true;
            }
        }

        protected override void OnDispose()
        {
            m_DefaultCanvas = null;
            m_TestUICanvas = null;
        }
    }
}
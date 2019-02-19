#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				UIManager.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 04 Oct 2018 05:10:05 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev.UI
{
    using DR.Book.SRPG_Dev.Framework;

    public class UIManager : ViewManagerBase<UIManager, UIManager.UIDictionary>
    {
        #region Fields/Properties
        [SerializeField]
        private Canvas m_PreDefaultCanvas;

        [SerializeField]
        private Canvas m_PreObjectCanvas;

        /// <summary>
        /// 默认Canvas
        /// </summary>
        public static Canvas defaultCanvas
        {
            get { return views.defaultCanvas; }
            set { views.defaultCanvas = value; }
        }

        /// <summary>
        /// 可以显示ObjectCanvas
        /// </summary>
        public static Canvas objectCanvas
        {
            get { return views.objectCanvas; }
            set { views.objectCanvas = value; }
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();

            gameObject.layer = LayerMask.NameToLayer("UI");
        }

        protected override UIDictionary CreateViewDictionary()
        {
            return new UIDictionary(m_PreDefaultCanvas, m_PreObjectCanvas);
        }

        #region class UIDictionary
        public class UIDictionary : ViewDictionary
        {
            /// <summary>
            /// 默认Canvas
            /// </summary>
            public Canvas defaultCanvas { get; set; }

            /// <summary>
            /// 可以显示ObjectCanvas
            /// </summary>
            public Canvas objectCanvas { get; set; }

            #region Constructor
            public UIDictionary() : base()
            {
            }

            public UIDictionary(Canvas defaultCanvas, Canvas objectCanvas) : base()
            {
                this.defaultCanvas = defaultCanvas;
                this.objectCanvas = objectCanvas;
            }
            #endregion

            /// <summary>
            /// 获取view组件类型
            /// </summary>
            /// <param name="viewName"></param>
            /// <returns></returns>
            protected override Type GetViewComponentType(string viewName)
            {
                Type viewType;
                switch (viewName)
                {
                    case UINames.k_UITextPanel:
                        viewType = typeof(UITextPanel);
                        break;
                    case UINames.k_UIMenuPanel:
                        viewType = typeof(UIMenuPanel);
                        break;
                    default:
                        viewType = typeof(UIManager).Assembly.GetType("DR.Book.SRPG_Dev.UI." + viewName);
                        break;
                }

                return viewType;
            }

            /// <summary>
            /// 在实例化时设置父对象
            /// </summary>
            /// <param name="view"></param>
            protected override void OnInstantiateView(ViewBase view)
            {
                view.transform.SetParent(defaultCanvas.transform, false);
            }

            /// <summary>
            /// 在打开关闭时，设置是否可点击
            /// </summary>
            /// <param name="previous"></param>
            /// <param name="current"></param>
            /// <param name="open"></param>
            /// <param name="openArgs"></param>
            protected override void OnStackViewOpenOrClose(ViewBase previous, ViewBase current, bool open, params object[] openArgs)
            {
                base.OnStackViewOpenOrClose(previous, current, open, openArgs);

                if (previous != null)
                {
                    (previous as UIBase).canvasGroup.blocksRaycasts = !open;
                }

                if (current != null)
                {
                    (current as UIBase).canvasGroup.blocksRaycasts = true;
                }

                if (open)
                {
                    current.transform.SetAsLastSibling();
                }
            }

            protected override void OnDispose()
            {
                base.OnDispose();

                this.defaultCanvas = null;
                this.objectCanvas = null;
            }
        }
        #endregion
    }
}
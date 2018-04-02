#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestUICanvas.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 20:10:01 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    [RequireComponent(typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster))]
    public class TestUICanvas : UIBase
    {
        #region Unity Component Field/Property
        private Canvas m_Canvas;
        private CanvasScaler m_CanvasScaler;
        private GraphicRaycaster m_GraphicRaycaster;

        public Canvas canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = GetComponent<Canvas>();
                }
                return m_Canvas;
            }
        }

        public CanvasScaler canvasScaler
        {
            get
            {
                if (m_CanvasScaler == null)
                {
                    m_CanvasScaler = GetComponent<CanvasScaler>();
                }
                return m_CanvasScaler;
            }
        }

        public GraphicRaycaster graphicRaycaster
        {
            get
            {
                if (m_GraphicRaycaster == null)
                {
                    m_GraphicRaycaster = GetComponent<GraphicRaycaster>();
                }
                return m_GraphicRaycaster;
            }
        }
        #endregion

        #region Field/Property
        private readonly List<string> m_ChildPanelNames = new List<string>();

        /// <summary>
        /// 子面板的名字们
        /// </summary>
        public List<string> childPanelNames
        {
            get { return m_ChildPanelNames; }
        }
        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // 销毁所有子view
            for (int i = 0; i < m_ChildPanelNames.Count; i++)
            {
                // 如果ApplicationQuit或在关闭场景中手动Destroy(TestUIManager)时，
                // TestUIManager有时会提前被释放。
                // 所以不能直接调用TestUIManager.views。否则会调用Instantiate。
                // Unity在关闭场景时中有规定，不能在OnDestroy创建物体。
                if (TestUIManager.needCreated)
                {
                    break;
                }
                TestUIManager.views.DestroyView(m_ChildPanelNames[i]);
            }
        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				UIBase.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 08 Mar 2018 00:19:04 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIBase : ViewBase
    {
        #region Field
        [SerializeField, Tooltip("When OnFocus(), first selected game object.")]
        private GameObject m_FirstSelected;

        private CanvasGroup m_CanvasGroup;
        #endregion

        #region Property
        /// <summary>
        /// 当UI打开时，第一个选择的物体。
        /// </summary>
        public GameObject firstSelected
        {
            get { return m_FirstSelected; }
            set { m_FirstSelected = value; }
        }

        /// <summary>
        /// 获取或添加CanvasGroup组件
        /// </summary>
        public CanvasGroup canvasGroup
        {
            get
            {
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = GetComponent<CanvasGroup>();
                    if (m_CanvasGroup == null)
                    {
                        m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
                    }
                }
                return m_CanvasGroup;
            }
        }

        /// <summary>
        /// RectTransform组件
        /// </summary>
        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// 设置UI焦点为'firstSelect'的物体
        /// </summary>
        public virtual void OnFocus()
        {
            if (firstSelected != null)
            {
                FrameworkUIUtility.FindOrCreateEventSystem().SetSelectedGameObject(firstSelected);
            }
        }
        #endregion
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestUIManager.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 00:07:58 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    public class TestUIManager : ViewManagerBase<TestUIManager, TestUIDictionary>
    {
        /// <summary>
        /// 演示用，外部Canvas
        /// </summary>
        [SerializeField]
        public Canvas m_DefaultCanvas;

        /// <summary>
        /// 演示用，外部Canvas
        /// </summary>
        public static Canvas defaultCanvas
        {
            get { return instance.m_DefaultCanvas; }
            set
            {
                if (instance.m_DefaultCanvas != value)
                {
                    instance.m_DefaultCanvas = value;
                    views.m_DefaultCanvas = value;
                }
            }
        }

        protected override TestUIDictionary CreateViewDictionary()
        {
            /// 你也可以不在这里添加Canvas，
            /// 在Dictionary中可以GetComponent获取本组件。
            TestUIDictionary dict = new TestUIDictionary
            {
                m_DefaultCanvas = m_DefaultCanvas
            };
            return dict;
        }

        protected override void Awake()
        {
            base.Awake();

            gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }
}
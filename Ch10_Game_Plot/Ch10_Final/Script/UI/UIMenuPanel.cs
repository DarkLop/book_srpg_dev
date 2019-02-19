#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				UIMenuPanel.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 11 Jan 2019 22:49:14 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using UnityEngine;

namespace DR.Book.SRPG_Dev.UI
{
    using DR.Book.SRPG_Dev.Framework;

    public class UIMenuPanel : UIBase
    {
        #region UI Fields
        private SubUIButtonLayoutGroup m_ButtonLayoutGroup;
        #endregion

        #region Fields
        private Action<int> m_OnMenuDone;
        #endregion

        #region Load/Open/Close
        protected override IEnumerator OnLoadingView()
        {
            m_ButtonLayoutGroup = this.FindChildComponent<SubUIButtonLayoutGroup>("ButtonLayoutGroup");
            m_ButtonLayoutGroup.Display(false);
            m_ButtonLayoutGroup.onItemClick.AddListener(ButtonLayoutGroup_OnItemClick);

            yield break;
        }

        protected override IEnumerator OnOpenAsync(params object[] args)
        {
            yield return null;
            m_ButtonLayoutGroup.Display(true);
            firstSelected = m_ButtonLayoutGroup.firstButtonGameObject;
            OnFocus();
        }

        protected override void OnClose()
        {
            firstSelected = null;
            m_ButtonLayoutGroup.Display(false);
        }
        #endregion

        #region Event
        private void ButtonLayoutGroup_OnItemClick(GameObject itemGameObject, int index, string message)
        {
            if (m_OnMenuDone != null)
            {
                m_OnMenuDone(index);
                m_OnMenuDone = null;
            }
        }
        #endregion

        /// <summary>
        /// 设置需要的按钮和点击后的事件
        /// </summary>
        /// <param name="options"></param>
        /// <param name="onMenuDone"></param>
        /// <returns></returns>
        public bool SetOptions(string[] options, Action<int> onMenuDone)
        {
            if (options == null || options.Length == 0 || onMenuDone == null)
            {
                return false;
            }

            m_ButtonLayoutGroup.itemOptions.Clear();
            m_OnMenuDone = onMenuDone;

            for (int i = 0; i < options.Length; i++)
            {
                SubUIButtonLayoutGroup.ItemOption option = 
                    new SubUIButtonLayoutGroup.ItemOption(options[i], i.ToString());
                m_ButtonLayoutGroup.itemOptions.Add(option);
            }

            return true;
        }
    }
}
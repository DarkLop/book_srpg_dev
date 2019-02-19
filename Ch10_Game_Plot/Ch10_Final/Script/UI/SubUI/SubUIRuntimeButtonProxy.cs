#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				SubUILayoutGroupItem.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 28 Dec 2018 04:55:09 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DR.Book.SRPG_Dev.UI
{
    [RequireComponent(typeof(Image), typeof(Button)), DisallowMultipleComponent]
    public sealed class SubUIRuntimeButtonProxy : UIBehaviour, 
        IPointerEnterHandler
    {
        private SubUIButtonLayoutGroup m_ParentLayoutGroup;
        private int m_Index;
        private string m_Message;

        private Button m_Button;

        public SubUIButtonLayoutGroup parentLayoutGroup
        {
            get { return m_ParentLayoutGroup; }
            internal set { m_ParentLayoutGroup = value; }
        }

        public int index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        public string message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

        public Button button
        {
            get
            {
                if (m_Button == null)
                {
                    m_Button = GetComponent<Button>();
                    m_Button.onClick.AddListener(Button_onClick);
                }
                return m_Button;
            }
        }

        protected override void Awake()
        {
            if (m_Button == null)
            {
                m_Button = GetComponent<Button>();
                m_Button.onClick.AddListener(Button_onClick);
            }
        }

        protected override void OnDestroy()
        {
            if (m_Button != null)
            {
                m_Button.onClick.RemoveListener(Button_onClick);
                m_Button = null;
            }
        }

        private void Button_onClick()
        {
            if (parentLayoutGroup != null)
            {
                parentLayoutGroup.Internal_OnItemClick(gameObject, index, message);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem eventSystem = UIManager.instance.GetComponentInChildren<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(gameObject);
            }
        }

    }
}
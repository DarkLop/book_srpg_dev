#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				SubUIMenuLayout.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 28 Dec 2018 03:36:06 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DR.Book.SRPG_Dev.UI
{
    [DisallowMultipleComponent]
    [AddComponentMenu("SRPG/UI/Button Layout Group")]
    public class SubUIButtonLayoutGroup : UIBehaviour
    {
#if UNITY_EDITOR
        #region Editor Create Menu
        [UnityEditor.MenuItem("GameObject/SRPG/UI/ButtonLayoutGroup(Vertical)", false, -1)]
        public static SubUIButtonLayoutGroup EditorCreateButtonVerticalLayoutGroup(UnityEditor.MenuCommand menuCommand)
        {
            return EditorCreateButtonLayoutGroup<VerticalLayoutGroup>(menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/SRPG/UI/ButtonLayoutGroup(Horizontal)", false, -1)]
        public static SubUIButtonLayoutGroup EditorCreateButtonHorizontalLayoutGroup(UnityEditor.MenuCommand menuCommand)
        {
            return EditorCreateButtonLayoutGroup<HorizontalLayoutGroup>(menuCommand);
        }

        [UnityEditor.MenuItem("GameObject/SRPG/UI/ButtonLayoutGroup(Grid)", false, -1)]
        public static SubUIButtonLayoutGroup EditorCreateButtonGridLayoutGroup(UnityEditor.MenuCommand menuCommand)
        {
            return EditorCreateButtonLayoutGroup<GridLayoutGroup>(menuCommand);
        }

        private static DefaultControls.Resources s_EditorStandardResources;

        private static DefaultControls.Resources EditorGetStandardResources()
        {
            if (s_EditorStandardResources.standard == null)
            {
                s_EditorStandardResources.standard = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                s_EditorStandardResources.background = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                s_EditorStandardResources.inputField = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
                s_EditorStandardResources.knob = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                s_EditorStandardResources.checkmark = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
                s_EditorStandardResources.dropdown = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
                s_EditorStandardResources.mask = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
            }
            return s_EditorStandardResources;
        }

        private static SubUIButtonLayoutGroup EditorCreateButtonLayoutGroup<T>(UnityEditor.MenuCommand menuCommand) where T : LayoutGroup
        {
            GameObject gameObject = new GameObject("ButtonLayoutGroup", typeof(RectTransform), typeof(T), typeof(ContentSizeFitter));
            SubUIButtonLayoutGroup btnLayout = gameObject.AddComponent<SubUIButtonLayoutGroup>();

            LayoutGroup layoutGroup = gameObject.GetComponent<T>();
            btnLayout.layoutGroup = layoutGroup;

            GameObject itemTemplate = DefaultControls.CreateButton(EditorGetStandardResources());
            itemTemplate.name = "ItemTemplate";
            Text itemTemplateText = itemTemplate.GetComponentInChildren<Text>();
            itemTemplateText.text = "Template Button";
            itemTemplateText.fontSize = 20;
            //itemTemplate.SetActive(false);
            itemTemplate.transform.SetParent(gameObject.transform, false);
            btnLayout.itemTemplate = itemTemplate;

            ContentSizeFitter fitter = gameObject.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject parent = menuCommand.context as GameObject;
            if (parent != null)
            {
                gameObject.name = UnityEditor.GameObjectUtility.GetUniqueNameForSibling(parent.transform, gameObject.name);
            }
            UnityEditor.Undo.RegisterCreatedObjectUndo(gameObject, "Create" + gameObject.name);
            if (parent != null)
            {
                UnityEditor.Undo.SetTransformParent(gameObject.transform, parent.transform, "Parent" + gameObject.name);
                UnityEditor.GameObjectUtility.SetParentAndAlign(gameObject, parent);
            }

            UnityEditor.Selection.activeGameObject = gameObject;
            return btnLayout;
        }
        #endregion
#endif

        #region struct ItemOption
        /// <summary>
        /// item参数
        /// </summary>
        [Serializable]
        public struct ItemOption
        {
            /// <summary>
            /// 如果模版存在Text组件或模版子物体存在Text组件，Text组件的text属性
            /// </summary>
            [SerializeField, Tooltip("Text")]
            public string text;

            /// <summary>
            /// 点击事件时发送的消息
            /// </summary>
            [SerializeField, Tooltip("Send this message on click.")]
            public string message;

            /// <summary>
            /// 是否显示
            /// </summary>
            [SerializeField, Tooltip("Show or not show")]
            public bool active;

            /// <summary>
            /// 是否可点击
            /// </summary>
            [SerializeField, Tooltip("Button can click.")]
            public bool interactable;

            public ItemOption(string text, string message)
            {
                this.text = text;
                this.message = message;
                this.active = true;
                this.interactable = true;
            }
        }
        #endregion

        #region UI Fields
        [SerializeField]
        private LayoutGroup m_LayoutGroup;
        [SerializeField]
        private GameObject m_ItemTemplate;
        [SerializeField]
        private List<ItemOption> m_ItemOptions = new List<ItemOption>();

        private readonly List<SubUIRuntimeButtonProxy> m_Items = new List<SubUIRuntimeButtonProxy>();
        #endregion

        #region UI Properties
        /// <summary>
        /// 布局组件
        /// </summary>
        public LayoutGroup layoutGroup
        {
            get { return m_LayoutGroup; }
            set { m_LayoutGroup = value; }
        }

        /// <summary>
        /// 模版物体
        /// </summary>
        public GameObject itemTemplate
        {
            get { return m_ItemTemplate; }
            set { m_ItemTemplate = value; }
        }

        /// <summary>
        /// item参数
        /// </summary>
        public List<ItemOption> itemOptions
        {
            get
            {
                if (m_ItemOptions == null)
                {
                    m_ItemOptions = new List<ItemOption>();
                }
                return m_ItemOptions;
            }
            set { m_ItemOptions = value; }
        }

        public GameObject firstButtonGameObject
        {
            get
            {
                if (m_Items.Count == 0)
                {
                    return null;
                }
                return m_Items[0].gameObject;
            }
        }
        #endregion

        #region Unity Event
        /// <summary>
        /// 当item被点击时
        /// Args: 
        ///     GameObject buttonGameObject, // 被点击的按钮
        ///     int index, // 在此group中的下标，
        ///     string message, // 点击事件时发送的消息。
        /// </summary>
        [Serializable]
        public class OnItemClickEvent : UnityEvent<GameObject, int, string> { }

        [Space, SerializeField]
        private OnItemClickEvent m_OnItemClickEvent = new OnItemClickEvent();

        /// <summary>
        /// 当item被点击时
        /// Args: 
        ///     GameObject buttonGameObject, // 被点击的按钮
        ///     int index, // 在此group中的下标，
        ///     string message, // 点击事件时发送的消息。
        /// </summary>
        public OnItemClickEvent onItemClick
        {
            get
            {
                if (m_OnItemClickEvent == null)
                {
                    m_OnItemClickEvent = new OnItemClickEvent();
                }
                return m_OnItemClickEvent;
            }
            set { m_OnItemClickEvent = value; }
        }

        internal void Internal_OnItemClick(GameObject itemGameObject, int index, string message)
        {
            if (m_OnItemClickEvent != null)
            {
                m_OnItemClickEvent.Invoke(itemGameObject, index, message);
            }
        }
        #endregion

        #region Unity Callback
        protected override void Awake()
        {
            if (itemTemplate != null && itemTemplate.gameObject.activeSelf)
            {
                itemTemplate.gameObject.SetActive(false);
            }
        }

        protected override void OnEnable()
        {
            CreateItems();
        }

        protected override void OnDisable()
        {
            DestroyAllItems();
        }
        #endregion

        #region Method
        /// <summary>
        /// 重新生成items
        /// </summary>
        public void RefreshItems()
        {
            DestroyAllItems();
            CreateItems();
        }

        /// <summary>
        /// 创建item
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected SubUIRuntimeButtonProxy CreateLayoutItem(string text, int index, string message, bool interactable)
        {
            if (itemTemplate == null)
            {
                Debug.LogError("SubUIButtonLayoutGroup -> please set a template.");
                return null;
            }

            Transform parentTransform = layoutGroup == null ? transform : layoutGroup.transform;
            GameObject itemGameObject = GameObject.Instantiate(itemTemplate, parentTransform, false);

            SubUIRuntimeButtonProxy item = itemGameObject.GetComponent<SubUIRuntimeButtonProxy>();
            if (item == null)
            {
                item = itemGameObject.AddComponent<SubUIRuntimeButtonProxy>();
            }

            item.parentLayoutGroup = this;
            item.index = index;
            item.message = message;
            item.button.interactable = interactable;
            itemGameObject.SetActive(true);

            Text txt = itemGameObject.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = text;
            }

            OnCreateLayoutItem(item);
            return item;
        }

        /// <summary>
        /// 当创建item时
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnCreateLayoutItem(SubUIRuntimeButtonProxy item)
        {

        }

        /// <summary>
        /// 创建所有items
        /// </summary>
        protected void CreateItems()
        {
            for (int i = 0; i < itemOptions.Count; i++)
            {
                if (!itemOptions[i].active)
                {
                    continue;
                }
                SubUIRuntimeButtonProxy item = CreateLayoutItem(itemOptions[i].text, m_Items.Count, itemOptions[i].message, itemOptions[i].interactable);
                if (item != null)
                {
                    m_Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 销毁所有items
        /// </summary>
        protected void DestroyAllItems()
        {
            if (m_Items.Count > 0)
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    GameObject.Destroy(m_Items[i].gameObject);
                }
                m_Items.Clear();
            }
        }

        public void Display(bool show)
        {
            gameObject.SetActive(show);
        }
        #endregion
    }
}
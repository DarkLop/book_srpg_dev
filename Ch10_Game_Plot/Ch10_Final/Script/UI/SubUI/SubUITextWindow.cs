#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				SubUITextWindow.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 07 Jan 2019 05:35:20 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DR.Book.SRPG_Dev.UI
{
    [AddComponentMenu("SRPG/UI/Text Window")]
    public class SubUITextWindow : UIBehaviour
    {
        #region UI Fields
        [SerializeField]
        private Image m_ImgBackground;
        [SerializeField]
        private Image m_ImgProfile;
        [SerializeField]
        private Image m_ImgBackgroundInset;
        [SerializeField]
        private Text m_TxtText;
        [SerializeField]
        private Image m_ImgIcon;
        #endregion

        #region UI Properties
        public Image imgBackground
        {
            get { return m_ImgBackground; }
            set { m_ImgBackground = value; }
        }

        public Image imgProfile
        {
            get { return m_ImgProfile; }
            set { m_ImgProfile = value; }
        }

        public Image imgBackgroundInset
        {
            get { return m_ImgBackgroundInset; }
            set { m_ImgBackgroundInset = value; }
        }

        public Text txtText
        {
            get { return m_TxtText; }
            set { m_TxtText = value; }
        }

        public Image imgIcon
        {
            get { return m_ImgIcon; }
            set { m_ImgIcon = value; }
        }
        #endregion

        #region Set Property Methods
        public void SetBackground(Sprite background)
        {
            if (imgBackground != null)
            {
                imgBackground.overrideSprite = background;
            }
        }

        public void SetBackgroundInset(Sprite backgroundInset)
        {
            if (imgBackgroundInset != null)
            {
                imgBackgroundInset.overrideSprite = backgroundInset;
            }
        }

        public void SetProfile(Sprite profile)
        {
            if (imgProfile != null)
            {
                imgProfile.overrideSprite = profile;
            }
        }

        public void SetText(string text)
        {
            m_Text = text;
            if (txtText != null)
            {
                txtText.text = text;
            }
        }

        public void SetIcon(Sprite icon)
        {
            if (imgIcon != null)
            {
                imgIcon.overrideSprite = icon;
            }
        }
        #endregion

        #region Fields
        [SerializeField]
        private float m_WordInterval = 0.05f;

        private string m_Text = string.Empty;
        private Coroutine m_WritingCoroutine = null;
        #endregion

        #region Properties
        /// <summary>
        /// 写入时每个字符的时间间隔
        /// </summary>
        public float wordInterval
        {
            get { return m_WordInterval; }
            set { m_WordInterval = Mathf.Max(0f, value); }
        }

        /// <summary>
        /// 是否在写入状态
        /// </summary>
        public bool isWriting
        {
            get { return m_WritingCoroutine != null; }
        }

        /// <summary>
        /// 获取应写入的完整文本
        /// </summary>
        public string text
        {
            get { return m_Text; }
        }
        #endregion

        #region Unity Event
        /// <summary>
        /// 当文本写入完成时：
        /// Args: 
        ///     SubUITextWindow textWindow; // 写入的窗口
        /// </summary>
        [Serializable]
        public class TextWriteDoneEvent : UnityEvent<SubUITextWindow> { }

        [Space, SerializeField]
        private TextWriteDoneEvent m_TextWriteDoneEvent = new TextWriteDoneEvent();

        /// <summary>
        /// 当文本写入完成时：
        /// Args: 
        ///     SubUITextWindow textWindow; // 写入的窗口
        /// </summary>
        public TextWriteDoneEvent textWriteDone
        {
            get
            {
                if (m_TextWriteDoneEvent == null)
                {
                    m_TextWriteDoneEvent = new TextWriteDoneEvent();
                }
                return m_TextWriteDoneEvent;
            }
            set { m_TextWriteDoneEvent = value; }
        }
        #endregion

        #region Unity Callback
        protected override void Awake()
        {
            if (txtText != null)
            {
                m_Text = txtText.text;
            }
        }

        protected override void OnEnable()
        {
            DisplayIcon(false);
        }

        protected override void Reset()
        {
            textWriteDone.RemoveAllListeners();
            SetBackground(null);
            SetBackgroundInset(null);
            SetProfile(null);
            SetText(string.Empty);
        }
        #endregion

        #region Write Text Methods
        /// <summary>
        /// 如果正在写入，立即写入文本
        /// </summary>
        public void WriteText()
        {
            if (isWriting)
            {
                StopCoroutine(m_WritingCoroutine);
                m_WritingCoroutine = null;

                txtText.text = m_Text;
                OnTextWriteDone();
            }
        }

        /// <summary>
        /// 立即写入文本
        /// </summary>
        /// <param name="text"></param>
        public void WriteText(string text)
        {
            if (isWriting)
            {
                StopCoroutine(m_WritingCoroutine);
                m_WritingCoroutine = null;
            }

            SetText(text);
            OnTextWriteDone();
        }

        /// <summary>
        /// 逐字写入文本
        /// </summary>
        /// <param name="text"></param>
        public void WriteTextAsync(string text)
        {
            if (isWriting)
            {
                StopCoroutine(m_WritingCoroutine);
                m_WritingCoroutine = null;
            }

            m_Text = text;
            DisplayIcon(false);

            // Text组件不存在
            if (txtText == null)
            {
                OnTextWriteDone();
                return;
            }

            // 没有要写入的内容
            txtText.text = string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                OnTextWriteDone();
                return;
            }

            m_WritingCoroutine = StartCoroutine(WritingText());
        }

        private IEnumerator WritingText()
        {
            // 富文本变量
            Dictionary<int, int> richLengthDict;
            Dictionary<int, KeyValuePair<string, string>> richTextDict;
            int richCount = RegexUtility.GetRichTextFormatString(m_Text, out richLengthDict, out richTextDict);

            int index = 0; // 正在写入的下标
            int richIndex = 0; // 正在写入的富文本下标
            string curText = string.Empty; // 当前Text组件文本
            string richTextInset = string.Empty; // 富文本内的普通文本
            string richText = string.Empty; // 富文本
            while (txtText.text != m_Text)
            {
                if (wordInterval <= 0f)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(wordInterval);
                }

                // 如果存在富文本
                if (richCount > 0 && richLengthDict.ContainsKey(index))
                {
                    // 如果没有内容：例如 "<color=red></color>"
                    if (string.IsNullOrEmpty(richTextDict[index].Value))
                    {
                        richText = richTextDict[index].Key;
                    }
                    else
                    {
                        richTextInset += richTextDict[index].Value[richIndex++];
                        richText = string.Format(richTextDict[index].Key, richTextInset);
                        txtText.text = curText + richText;
                    }

                    // 富文本写入完成
                    if (richText.Length == richLengthDict[index])
                    {
                        curText += richText;
                        richCount--;
                        richIndex = 0;
                        richTextInset = string.Empty;
                        richText = string.Empty;
                        index += richLengthDict[index];
                    }
                }
                else
                {
                    curText += m_Text[index++];
                    txtText.text = curText;
                }
            }

            OnTextWriteDone();
            m_WritingCoroutine = null;
        }
        #endregion

        #region Display Methods
        public void Display(bool open)
        {
            if (gameObject.activeSelf != open)
            {
                gameObject.SetActive(open);
            }
        }

        public void DisplayIcon(bool show)
        {
            if (imgIcon != null && imgIcon.enabled != show)
            {
                imgIcon.enabled = show;
            }
        }
        #endregion

        #region Run Event Methods
        protected void OnTextWriteDone()
        {
            textWriteDone.Invoke(this);
            DisplayIcon(true);
        }
        #endregion
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				UITextPanel.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 07 Jan 2019 05:35:01 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using UnityEngine;

namespace DR.Book.SRPG_Dev.UI
{
    using DR.Book.SRPG_Dev.Framework;
    using DR.Book.SRPG_Dev.Models;

    public class UITextPanel : UIBase
    {
        private static readonly UI_TextWriteDoneArgs s_TextWriteDoneArgs = new UI_TextWriteDoneArgs();

        private SubUITextWindow m_TopTextWindow;
        private SubUITextWindow m_BottomTextWindow;
        private SubUITextWindow m_GlobalTextWindow;

        public bool isWriting
        {
            get
            {
                return m_TopTextWindow.isWriting 
                    || m_BottomTextWindow.isWriting
                    || m_GlobalTextWindow.isWriting;
            }
        }

        public bool hasWindowDisplay
        {
            get
            {
                return m_TopTextWindow.gameObject.activeSelf
                  || m_BottomTextWindow.gameObject.activeSelf
                  || m_GlobalTextWindow.gameObject.activeSelf;
            }
        }

        #region Get Window
        public SubUITextWindow GetWindow(string position)
        {
            if (position == "top")
            {
                return m_TopTextWindow;
            }
            else if (position == "bottom")
            {
                return m_BottomTextWindow;
            }
            else
            {
                return m_GlobalTextWindow;
            }
        }

        public SubUITextWindow GetWritingWindow()
        {
            if (m_TopTextWindow.isWriting)
            {
                return m_TopTextWindow;
            }

            if (m_BottomTextWindow.isWriting)
            {
                return m_BottomTextWindow;
            }

            if (m_GlobalTextWindow.isWriting)
            {
                return m_GlobalTextWindow;
            }

            return null;
        }
        #endregion

        #region Load / Destroy
        protected override IEnumerator OnLoadingView()
        {
            m_TopTextWindow = this.FindChildComponent<SubUITextWindow>("TopTextWindow");
            m_BottomTextWindow = this.FindChildComponent<SubUITextWindow>("BottomTextWindow");
            m_GlobalTextWindow = this.FindChildComponent<SubUITextWindow>("GlobalTextWindow");

            m_TopTextWindow.textWriteDone.AddListener(TopTextWindow_TextWriteDone);
            m_BottomTextWindow.textWriteDone.AddListener(BottomTextWindow_TextWriteDone);
            m_GlobalTextWindow.textWriteDone.AddListener(GlobalTextWindow_TextWriteDone);

            MessageCenter.AddListener(MessageNames.k_Map_Talk, Map_Talk);

            yield break;
        }

        protected override void OnDestroying()
        {
            MessageCenter.RemoveListener(MessageNames.k_Map_Talk, Map_Talk);
        }
        #endregion

        #region Open/Close
        protected override void OnOpen(params object[] args)
        {
            m_TopTextWindow.SetText(string.Empty);
            m_TopTextWindow.Display(false);
            m_BottomTextWindow.SetText(string.Empty);
            m_BottomTextWindow.Display(false);
            m_GlobalTextWindow.SetText(string.Empty);
            m_GlobalTextWindow.Display(false);
        }

        protected override void OnClose()
        {
            m_TopTextWindow.SetProfile(null);
            m_BottomTextWindow.SetProfile(null);
        }
        #endregion

        #region Event
        private void TopTextWindow_TextWriteDone(SubUITextWindow textWindow)
        {
            TextWindow_TextWriteDone("top", textWindow.text);
        }

        private void BottomTextWindow_TextWriteDone(SubUITextWindow textWindow)
        {
            TextWindow_TextWriteDone("bottom", textWindow.text);
        }

        private void GlobalTextWindow_TextWriteDone(SubUITextWindow textWindow)
        {
            TextWindow_TextWriteDone("global", textWindow.text);
        }

        private void TextWindow_TextWriteDone(string position, string text)
        {
            s_TextWriteDoneArgs.position = position;
            s_TextWriteDoneArgs.message = text;
            this.SendByMessageCenter(MessageNames.k_UI_TextWriteDone, s_TextWriteDoneArgs);
        }

        private void Map_Talk(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            Map_TalkArgs args = messageArgs as Map_TalkArgs;

            RoleModel model = ModelManager.models.Get<RoleModel>();
            Character character = model.GetOrCreateCharacter(args.topId);

            // TODO
            Sprite profile = ResourcesManager.LoadSprite(character.info.profile);
            m_TopTextWindow.SetProfile(profile);

            if (args.all)
            {
                character = model.GetOrCreateCharacter(args.bottomId);
                profile = ResourcesManager.LoadSprite(character.info.profile);
                m_BottomTextWindow.SetProfile(profile);

                m_TopTextWindow.Display(true);
                m_BottomTextWindow.Display(true);
            }
            else
            {
                m_TopTextWindow.Display(true);
            }
        }
        #endregion

        #region Write Text/Icon Methods
        /// <summary>
        /// 立即写入文本
        /// </summary>
        public void WriteTextImmediately()
        {
            SubUITextWindow window = GetWritingWindow();
            if (window == null)
            {
                return;
            }

            window.WriteText();
        }

        /// <summary>
        /// 写入文本
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="async"></param>
        public void WriteText(string position, string text, bool async)
        {
            SubUITextWindow textWindow;
            // 我这里用的字符串，你可以用Enum，这取决于你的文本执行器
            switch (position)
            {
                case "top":
                    m_GlobalTextWindow.Display(false);
                    textWindow = m_TopTextWindow;
                    break;
                case "bottom":
                    m_GlobalTextWindow.Display(false);
                    textWindow = m_BottomTextWindow;
                    break;
                default:
                    m_TopTextWindow.Display(false);
                    m_BottomTextWindow.Display(false);
                    textWindow = m_GlobalTextWindow;
                    break;
            }

            textWindow.Display(true);
            if (async)
            {
                textWindow.WriteTextAsync(text);
            }
            else
            {
                textWindow.WriteText(text);
            }
        }

        /// <summary>
        /// 隐藏按键提示符
        /// </summary>
        public void HideIcon()
        {
            m_TopTextWindow.DisplayIcon(false);
            m_BottomTextWindow.DisplayIcon(false);
            m_GlobalTextWindow.DisplayIcon(false);
        }
        #endregion

        #region Display
        public void CloseWindow(string position)
        {
            if (position == "top")
            {
                m_TopTextWindow.Display(false);
            }
            else if (position == "bottom")
            {
                m_BottomTextWindow.Display(false);
            }
            else
            {
                m_GlobalTextWindow.Display(false);
            }

            ClearProfile(position);
        }

        public void ClearProfile(string position)
        {
            if (position == "top")
            {
                m_TopTextWindow.SetProfile(null);
            }
            else if (position == "bottom")
            {
                m_BottomTextWindow.SetProfile(null);
            }
        }
        #endregion
    }
}
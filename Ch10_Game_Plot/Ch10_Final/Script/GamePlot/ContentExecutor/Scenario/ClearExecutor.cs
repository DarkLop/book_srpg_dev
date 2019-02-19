#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				ClearExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 13 Jan 2019 02:57:22 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    using DR.Book.SRPG_Dev.UI;

    public class ClearExecutor : ScenarioContentExecutor<ClearExecutor.ClearArgs>
    {
        public struct ClearArgs
        {
            public string type;
            public string arg0;
            public string arg1;
            public string arg2;
            public string arg3;
        }

        public override string code
        {
            get { return "clear"; }
        }

        protected static readonly HashSet<string> s_DefaultSupportedTypes = new HashSet<string>()
        {
            "text"
        };

        protected virtual HashSet<string> GetSupportedTypes()
        {
            return s_DefaultSupportedTypes;
        }

        #region Parse Args
        public override bool ParseArgs(IScenarioContent content, ref ClearArgs args, out string error)
        {
            if (content.length < 2)
            {
                error = GetLengthErrorString();
                return false;
            }

            args.type = content[1].ToLower();
            if (!GetSupportedTypes().Contains(args.type))
            {
                error = string.Format(
                    "{0} ParseArgs -> the type `{1}` is not supported.",
                    typeName,
                    args.type);
                return false;
            }

            int length = content.length;
            int index = 2;

            if (index < length)
            {
                args.arg0 = content[index++];
            }

            if (index < length)
            {
                args.arg1 = content[index++];
            }

            if (index < length)
            {
                args.arg2 = content[index++];
            }

            if (index < length)
            {
                args.arg3 = content[index++];
            }

            return CheckArgsCorrect(content, ref args, out error);
        }

        protected virtual bool CheckArgsCorrect(IScenarioContent content, ref ClearArgs args, out string error)
        {
            switch (args.type)
            {
                case "text":
                    if (!string.IsNullOrEmpty(args.arg0))
                    {
                        args.arg0 = args.arg0.ToLower();
                        if (args.arg0 != "top" && args.arg0 != "bottom" && args.arg0 != "global")
                        {
                            error = string.Format(
                                "{0} ParseArgs error: position must be empty or one of [top, bottom, global].",
                                typeName,
                                args.arg0);
                            return false;
                        }
                    }
                    break;
                // TODO other
                default:
                    break;
            }

            error = null;
            return true;
        }
        #endregion

        #region Run
        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, ClearArgs args, out string error)
        {
            switch (args.type)
            {
                case "text":
                    return ClearTextCmd(gameAction, args, out error);
                // TODO other
                default:
                    error = string.Format(
                        "{0} Run -> UNESPECTED ERROR! the type `{1}` is not supported.",
                        typeName,
                        args.type);
                    return ActionStatus.Error;
            }
        }

        protected ActionStatus ClearTextCmd(IGameAction gameAction, ClearArgs args, out string error)
        {
            error = null;
            if (!UIManager.views.ContainsKey(UINames.k_UITextPanel))
            {
                return ActionStatus.NextFrame;
            }

            string position = args.arg0;
            if (string.IsNullOrEmpty(position))
            {
                UIManager.views.CloseView(UINames.k_UITextPanel);
                return ActionStatus.NextFrame;
            }

            UITextPanel panel = UIManager.views.GetView<UITextPanel>(UINames.k_UITextPanel);
            panel.CloseWindow(position);

            if (!panel.hasWindowDisplay)
            {
                UIManager.views.CloseView(UINames.k_UITextPanel);
            }

            return ActionStatus.NextFrame;
        }
        #endregion
    }
}
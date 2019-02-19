#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				LoadExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 14 Jan 2019 11:45:12 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    using DR.Book.SRPG_Dev.UI;

    public class LoadExecutor : ScenarioContentExecutor<LoadExecutor.LoadArgs>
    {
        public struct LoadArgs
        {
            public string type;
            public string arg0;
            public string arg1;
            public string arg2;
            public string arg3;
        }

        public override string code
        {
            get { return "load"; }
        }

        protected static readonly HashSet<string> s_DefaultSupportedTypes = new HashSet<string>()
        {
            "scene", "profile"
        };

        protected virtual HashSet<string> GetSupportedTypes()
        {
            return s_DefaultSupportedTypes;
        }

        public override bool ParseArgs(IScenarioContent content, ref LoadArgs args, out string error)
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

        protected virtual bool CheckArgsCorrect(IScenarioContent content, ref LoadArgs args, out string error)
        {
            switch (args.type)
            {
                case "scene":
                    if (string.IsNullOrEmpty(args.arg0))
                    {
                        error = string.Format(
                            "{0} ParseArgs error: scene name can not be null or empty.",
                            typeName);
                        return false;
                    }

                    break;
                case "profile":
                    if (string.IsNullOrEmpty(args.arg0))
                    {
                        error = string.Format(
                            "{0} ParseArgs error: profile position is null or empty.",
                            typeName);
                        return false;
                    }

                    args.arg0 = args.arg0.ToLower();
                    if (args.arg0 != "top" || args.arg0 != "bottom")
                    {
                        error = string.Format(
                            "{0} ParseArgs error: profile position can only be one of [top, bottom].",
                            typeName);
                        return false;
                    }

                    if (string.IsNullOrEmpty(args.arg1))
                    {
                        error = string.Format(
                            "{0} ParseArgs error: profile name is null or empty.",
                            typeName);
                        return false;
                    }

                    break;
                // TODO other
                default:
                    break;
            }

            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, LoadArgs args, out string error)
        {
            switch (args.type)
            {
                case "scene":
                    ScenarioAction action;
                    if (!ParseAction<ScenarioAction>(gameAction, out action, out error))
                    {
                        return ActionStatus.Error;
                    }
                    return action.LoadSceneCommand(args.arg0, out error);
                case "profile":
                    UITextPanel panel;
                    if (!UIManager.views.ContainsKey(UINames.k_UITextPanel))
                    {
                        panel = UIManager.views.OpenView<UITextPanel>(UINames.k_UITextPanel, false);
                        UIManager.views.CloseView();
                    }
                    else
                    {
                        panel = UIManager.views.GetView<UITextPanel>(UINames.k_UITextPanel);
                    }
                    SubUITextWindow window = panel.GetWindow(args.arg0);
                    Sprite profile = ResourcesManager.LoadProfileSprite(args.arg1);
                    window.SetProfile(profile);
                    break;
                default:
                    break;
            }

            error = null;
            return ActionStatus.NextFrame;
        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				MenuExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 11 Jan 2019 23:31:05 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    using DR.Book.SRPG_Dev.Models;
    using DR.Book.SRPG_Dev.UI;

    public class MenuExecutor : ScenarioContentExecutor<MenuExecutor.MenuArgs>
    {
        public struct MenuArgs
        {
            public string menuName;
            public string[] options;
        }

        public override string code
        {
            get { return "menu"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref MenuArgs args, out string error)
        {
            /// menu option
            ///     option0
            ///     option1
            ///     ...;
            if (content.length < 3)
            {
                error = GetLengthErrorString();
                return false;
            }

            // 获取使用的变量
            if (!RegexUtility.IsMatchVariable(content[1]))
            {
                error = GetMatchVariableErrorString(content[1]);
                return false;
            }
            args.menuName = content[1];

            TextInfoConfig config = TextInfoConfig.Get<TextInfoConfig>();
            List<string> options = new List<string>();
            int index = 2;
            while (index < content.length)
            {
                string line;
                if (content[index].StartsWith("\""))
                {
                    if (!ScenarioUtility.ParseContentString(content, ref index, out line, out error))
                    {
                        return false;
                    }
                }
                else
                {
                    // 可能是个变量
                    int id = -1;
                    if (!ParseOrGetVarValue(content[index], ref id, out error))
                    {
                        return false;
                    }

                    TextInfo info = config[id];
                    if (info == null)
                    {
                        error = string.Format(
                            "{0} ParseArgs error: text id `{1}` was not found.",
                            typeName,
                            content[index]);
                        return false;
                    }

                    line = info.text;
                    index++;
                }
                options.Add(line);
            }
            args.options = options.ToArray();

            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, MenuArgs args, out string error)
        {
            ScenarioAction action;
            if (!ParseAction<ScenarioAction>(gameAction, out action, out error))
            {
                return ActionStatus.Error;
            }

            // 重置变量为-1
            ScenarioBlackboard.Set(args.menuName, -1);

            // 打开UI
            UIMenuPanel panel = UIManager.views.OpenView<UIMenuPanel>(UINames.k_UIMenuPanel, false);

            // 设置选项并传入选择后的处理方法
            if (!panel.SetOptions(args.options, selected =>
             {
                 // 选择完成后的处理

                 // 关闭菜单UI
                 UIManager.views.CloseView(UINames.k_UIMenuPanel);
                 // 设置选择的按钮
                 ScenarioBlackboard.Set(args.menuName, selected);
                 // 继续剧本
                 action.MenuDone();
             }))
            {
                error = string.Format(
                    "{0} ParseArgs error: set menu options error.",
                    typeName);
                return ActionStatus.Error;
            }

            error = null;
            return ActionStatus.WaitMenuOption;
        }
    }
}
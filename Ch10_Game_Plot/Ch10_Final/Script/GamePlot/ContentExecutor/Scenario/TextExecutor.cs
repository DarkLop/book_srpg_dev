#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				TextExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 07 Jan 2019 09:38:41 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Text;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    using DR.Book.SRPG_Dev.Models;
    using DR.Book.SRPG_Dev.UI;

    public class TextExecutor : ScenarioContentExecutor<TextExecutor.TextArgs>
    {
        public struct TextArgs
        {
            public string position;
            public string text;
            public bool async;
        }

        public override string code
        {
            get { return "text"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref TextArgs args, out string error)
        {
            // text position text0 text1 ...;
            if (content.length < 3)
            {
                error = GetLengthErrorString();
                return false;
            }

            string position = content[1].ToLower();
            if (position != "top" && position != "bottom" && position != "global")
            {
                error = string.Format(
                    "{0} ParseArgs error: position must be one of [top, bottom, global].",
                    typeName,
                    content[1]);
                return false;
            }
            args.position = position;

            TextInfoConfig config = TextInfoConfig.Get<TextInfoConfig>();
            StringBuilder builder = new StringBuilder();

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
                builder.AppendLine(line);
            }

            args.text = builder.ToString();

            /// 从游戏设置中读取
            /// 最常见的形式是类似J-AVG快进的形式
            args.async = true;

            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, TextArgs args, out string error)
        {
            UITextPanel panel = UIManager.views.OpenView<UITextPanel>(UINames.k_UITextPanel, false);
            panel.WriteText(args.position, args.text, args.async);

            error = null;

            // 如果是快进模式，要等待一帧，否则有可能看不到界面，连闪屏都没有。
            return args.async ? ActionStatus.WaitWriteTextDone : ActionStatus.NextFrame;
        }
    }
}
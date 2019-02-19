#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TxtScript.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 30 Dec 2018 00:38:59 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // 可能需要正则表达式，如果你熟悉它的话

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    /// <summary>
    /// 剧本（脚本）
    /// </summary>
    public class TxtScript : IScenario, IList<TxtScript.Command>
    {
        #region Const/Static
        /// <summary>
        /// 用于命令的分割符
        /// </summary>
        public const string k_CommandSeparator = ";";

        /// <summary>
        /// 空格
        /// </summary>
        public const string k_Space = " ";

        /// <summary>
        /// 分隔符
        /// </summary>
        public const string k_Separator = "\t";

        /// <summary>
        /// 换行符
        /// </summary>
        public const string k_NewLine = "\n";

        /// <summary>
        /// 注释前缀
        /// </summary>
        public const string k_CommentingPrefix = "//";

        /// <summary>
        /// 默认剧本标识前缀
        /// </summary>
        public const string k_DefaultFlagMark = "#";
        #endregion

        #region Enum FormatType
        /// <summary>
        /// 格式化类型
        /// </summary>
        public enum FormatType
        {
            /// <summary>
            /// 按行格式化
            /// </summary>
            Line,

            /// <summary>
            /// 按命令格式化
            /// </summary>
            Command
        }
        #endregion

        #region Class Command
        /// <summary>
        /// 剧本的一条命令
        /// </summary>
        public class Command : IScenarioContent
        {
            #region Fields
            private readonly int m_LineNo;
            private readonly string m_Commenting;
            private readonly ScenarioContentType m_Type;
            private readonly string[] m_Arguments;
            #endregion

            #region Properties
            /// <summary>
            /// 行号，若命令在脚本中是多行，这里指最后一行的行号
            /// </summary>
            public int lineNo
            {
                get { return m_LineNo; }
            }

            /// <summary>
            /// 注释
            /// </summary>
            public string commenting
            {
                get { return m_Commenting; }
            }

            /// <summary>
            /// 类型
            /// </summary>
            public ScenarioContentType type
            {
                get { return m_Type; }
            }

            /// <summary>
            /// 参数数量
            /// </summary>
            public int length
            {
                get { return m_Arguments.Length; }
            }

            /// <summary>
            /// 索引器
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public string this[int index]
            {
                get { return m_Arguments[index]; }
            }

            /// <summary>
            /// 关键字或剧情标识
            /// </summary>
            public string code
            {
                get { return m_Arguments[0]; }
            }
            #endregion

            #region Constructor
            public Command(ScenarioContentType type, string[] arguments)
            {
                m_Type = type;
                m_Arguments = arguments;
            }

            public Command(int lineNo, ScenarioContentType type, string[] arguments) : this(type, arguments)
            {
                m_LineNo = lineNo;
            }
            public Command(string commenting, ScenarioContentType type, string[] arguments) : this(type, arguments)
            {
                m_LineNo = -1;
                m_Commenting = commenting;
            }

            public Command(int lineNo, string commenting, ScenarioContentType type, string[] arguments) : this(type, arguments)
            {
                m_LineNo = lineNo;
                m_Commenting = commenting;
            }
            #endregion

            #region ToString
            public override string ToString()
            {
                return string.Join(k_Space, m_Arguments) + k_CommandSeparator;
            }

            public string ToString(bool withCommenting)
            {
                if (!withCommenting)
                {
                    return ToString();
                }

                string thisCommenting = commenting;
                if (!thisCommenting.EndsWith(k_NewLine))
                {
                    thisCommenting += k_NewLine;
                }

                return string.Format("{0}{1}{2}", thisCommenting, string.Join(k_Space, m_Arguments), k_CommandSeparator);
            }
            #endregion
        }
        #endregion

        #region Fields
        private string m_Name;
        private string m_Buffer;
        private string m_FlagMark = k_DefaultFlagMark;
        private string m_CommentingPrefix = k_CommentingPrefix;
        private string m_Error = string.Empty;
        private readonly List<Command> m_Commands = new List<Command>();
        #endregion

        #region Properties
        /// <summary>
        /// 剧本名（可能为null）
        /// </summary>
        public string name
        {
            get { return m_Name; }
            private set { m_Name = value; }
        }

        /// <summary>
        /// 剧本的原始副本
        /// </summary>
        public string buffer
        {
            get { return m_Buffer; }
            private set { m_Buffer = value; }
        }

        /// <summary>
        /// 用作剧本标识的符号
        /// </summary>
        public string flagMark
        {
            get { return m_FlagMark; }
            set { m_FlagMark = value; }
        }

        /// <summary>
        /// 注释
        /// </summary>
        public string commentingPrefix
        {
            get { return m_CommentingPrefix; }
            set { m_CommentingPrefix = value; }
        }

        /// <summary>
        /// 错误
        /// </summary>
        public string formatError
        {
            get { return m_Error; }
            protected set { m_Error = value; }
        }

        /// <summary>
        /// 是否读取过剧本文本
        /// </summary>
        public bool isLoaded
        {
            get { return !string.IsNullOrEmpty(m_Buffer); }
        }

        /// <summary>
        /// 内容（动作）
        /// </summary>
        protected List<Command> commands
        {
            get { return m_Commands; }
        }

        /// <summary>
        /// 命令数量
        /// </summary>
        public int contentCount
        {
            get { return m_Commands.Count; }
        }

        public IScenarioContent GetContent(int index)
        {
            return m_Commands[index];
        }
        #endregion

        #region IList<TxtScript.Content> Properties
        int ICollection<TxtScript.Command>.Count
        {
            get { return m_Commands.Count; }
        }

        bool ICollection<TxtScript.Command>.IsReadOnly
        {
            get { return ((IList<Command>)m_Commands).IsReadOnly; }
        }

        public TxtScript.Command this[int index]
        {
            get { return m_Commands[index]; }
            set { throw new NotImplementedException("Readonly"); }
        }
        #endregion

        #region Constructor
        public TxtScript()
        {

        }

        public TxtScript(string flagMark)
        {
            if (flagMark != null)
            {
                flagMark = flagMark.Replace(" ", "");
            }

            if (!string.IsNullOrEmpty(flagMark))
            {
                m_FlagMark = flagMark;
            }
        }

        public TxtScript(string flagMark, string commentingPrefix)
        {
            /// 防止 flagMark 有空格，
            /// 它可以有特殊字符，但不推荐含有特殊符号，
            /// 你也可以使用 Trim() 去除两边的特殊字符，
            /// 或使用 Regex.Replace(flagMark, @"\s", "") 去除所有特殊字符。
            /// \s 是正则表达式的匹配符，包含任何空白的字符（[" ", "\f", "\n", "\t", "\v"...]）
            /// 这些限定不是必须的，也许你就喜欢有空格也说不定，
            /// 你在创建语言规则之前，应该考虑这些。
            if (flagMark != null)
            {
                flagMark = flagMark.Replace(" ", "");
            }

            // 防止 commentingPrefix 有空格。
            if (commentingPrefix != null)
            {
                commentingPrefix = commentingPrefix.Replace(" ", "");
            }

            if (!string.IsNullOrEmpty(flagMark))
            {
                m_FlagMark = flagMark;
            }

            if (!string.IsNullOrEmpty(commentingPrefix))
            {
                m_CommentingPrefix = commentingPrefix;
            }
        }
        #endregion

        #region Load/Reset Methods
        /// <summary>
        /// 读取剧本
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scriptText"></param>
        /// <returns></returns>
        public bool Load(string fileName, string scriptText)
        {
            return Load(fileName, scriptText, FormatType.Line);
        }

        /// <summary>
        /// 读取剧本，按FormatType.Command格式化没有行号和注释
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scriptText"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public bool Load(string fileName, string scriptText, FormatType formatType)
        {
            string script = Regex.Unescape(scriptText).Trim();

            if (string.IsNullOrEmpty(script))
            {
                formatError = "TxtScript Load -> `scriptText` is null or empty";
                return false;
            }

            Reset();

            bool loaded;
            if (formatType == FormatType.Line)
            {
                loaded = FormatScriptLines(script);
            }
            else
            {
                loaded = FormatScriptCommands(script);
            }

            if (loaded)
            {
                name = fileName;
                buffer = script;
            }

            return loaded;
        }

        public void Reset()
        {
            name = string.Empty;
            buffer = string.Empty;
            formatError = null;
            commands.Clear();
        }
        #endregion

        #region Format Line Methods
        /// <summary>
        /// 按每行格式化剧本
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        protected virtual bool FormatScriptLines(string script)
        {
            string[] separators = new string[] { k_Space, k_Separator };
            string[] newLineSeparator = new string[] { k_NewLine };

            // 不要删除空行，因为要记录行号
            string[] lineTexts = script.Split(newLineSeparator, StringSplitOptions.None);

            ScenarioContentType type = ScenarioContentType.Action;
            string commenting = string.Empty;
            List<string> arguments = new List<string>();

            int li = 0;
            while (li < lineTexts.Length)
            {
                // 删除左右空格和左右各种特殊转义符
                string line = lineTexts[li].Trim();

                // 删除每行注释
                int commentingIndex = line.IndexOf(commentingPrefix);
                if (commentingIndex != -1)
                {
                    commenting += line.Substring(commentingIndex) + Environment.NewLine;
                    line = line.Substring(0, commentingIndex).TrimEnd();
                }

                // 格式化行
                if (FormatLine(line, li, separators, ref commenting, ref type, ref arguments))
                {
                    li++;
                }
                else
                {
                    return false;
                }
            }

            // 如果还有参数，文本最后缺少 ";"
            if (arguments.Count > 0)
            {
                formatError = string.Format("TxtScript FormatError {0} -> syntactic error: missing `;`.", li);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 格式化行，
        ///     true: 下一行，
        ///     false: 有错误。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNo"></param>
        /// <param name="separators"></param>
        /// <param name="commenting"></param>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected virtual bool FormatLine(
            string line,
            int lineNo,
            string[] separators,
            ref string commenting,
            ref ScenarioContentType type,
            ref List<string> arguments)
        {
            // 如果为空或注释行，下一行
            if (string.IsNullOrEmpty(line))
            {
                return true;
            }

            // 搜索第一个 ";" 的下标
            // 以防止这种写法：
            // var aaa = 1; var bbb = 2;
            // 或者这种：
            // var aaa = 1; var
            // bbb = 2;
            int separatorIndex = line.IndexOf(k_CommandSeparator);

            // 如果没有找到 ";"
            if (separatorIndex == -1)
            {
                // 直接格式化内容
                FormatLineContent(line, separators, ref type, ref arguments);
                line = string.Empty;
            }

            // 如果找到了 ";"
            else
            {
                // 取 ";" 之前的内容，并格式化内容
                string contentText = line.Substring(0, separatorIndex).TrimEnd();

                // 取 ";" 之后的内容
                line = line.Remove(0, separatorIndex + 1);

                // 如果只有 ";"，后半截
                if (string.IsNullOrEmpty(contentText))
                {
                    return FormatLine(line, lineNo, separators, ref commenting, ref type, ref arguments);
                }
                else
                {
                    FormatLineContent(contentText, separators, ref type, ref arguments);

                    // 如果是标识，且内容超过1个，则语法错误
                    if (type == ScenarioContentType.Flag && arguments.Count > 1)
                    {
                        formatError = string.Format("TxtScript FormatCode {0} -> syntactic error: flag has no param.", lineNo);
                        return false;
                    }

                    Command command = new Command(lineNo, commenting, type, arguments.ToArray());
                    commands.Add(command);

                    // 重置参数，继续下一条内容
                    commenting = string.Empty;
                    type = ScenarioContentType.Action;
                    arguments.Clear();
                }
            }

            return FormatLine(line, lineNo, separators, ref commenting, ref type, ref arguments);
        }

        /// <summary>
        /// 格式化内容
        /// </summary>
        /// <param name="content"></param>
        /// <param name="separators"></param>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        private void FormatLineContent(
            string content,
            string[] separators,
            ref ScenarioContentType type,
            ref List<string> arguments)
        {
            // 按 [" "，"\t"] 分割每行
            string[] lineValues = content.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // 如果起始以标识开头，并且还没有内容
            if (lineValues[0].StartsWith(flagMark) && arguments.Count == 0)
            {
                type = ScenarioContentType.Flag;
            }

            // 添加内容
            for (int vi = 0; vi < lineValues.Length; vi++)
            {
                string value = lineValues[vi].Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    arguments.Add(value);
                }
            }
        }
        #endregion

        #region Format Command Methods
        /// <summary>
        /// 按内容格式化剧本，不含行号与注释
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        protected virtual bool FormatScriptCommands(string script)
        {
            // 以 [";"] 分割文本，并删除空白。
            string[] commandTexts = script.Split(
                new string[] { k_CommandSeparator },
                StringSplitOptions.RemoveEmptyEntries);

            // 分割剧本每个动作的分隔符： [" "，"\t"，"\n"] 
            string[] separators = new string[] { k_Space, k_Separator };
            string[] newLineSeparator = new string[] { k_NewLine };

            for (int i = 0; i < commandTexts.Length; i++)
            {
                // 删除左右空格和左右各种特殊转义符
                string commandText = commandTexts[i].Trim();

                // 如果为空，下一个动作
                if (string.IsNullOrEmpty(commandText))
                {
                    continue;
                }

                // 格式化每一次动作，生成命令
                Command command;
                FormatContentResult formatResult = FormatCommand(
                    i, // 不是行号，是下标
                    commandText,
                    separators,
                    newLineSeparator,
                    out command);

                // 成功添加
                if (formatResult == FormatContentResult.Succeed)
                {
                    commands.Add(command);
                }
                // 失败返回
                else if (formatResult == FormatContentResult.Failure)
                {
                    return false;
                }
                // 只有注释，下一动作
                else
                {
                    continue;
                }
            }

            return true;
        }

        /// <summary>
        /// 格式化每一条内容
        /// </summary>
        /// <param name="index">下标，不是行号</param>
        /// <param name="commandText"></param>
        /// <param name="separators"></param>
        /// <param name="newLineSeparator"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual FormatContentResult FormatCommand(
            int index,
            string commandText,
            string[] separators,
            string[] newLineSeparator,
            out Command command)
        {
            ScenarioContentType type = ScenarioContentType.Action;
            List<string> arguments = new List<string>();

            // 按 ["\n"] 分割每一条内容
            string[] lines = commandText.Split(newLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            for (int li = 0; li < lines.Length; li++)
            {
                string line = lines[li].Trim();

                // 删除每行注释
                int commentingIndex = line.IndexOf(commentingPrefix);
                if (commentingIndex != -1)
                {
                    line = line.Substring(0, commentingIndex).TrimEnd();
                }

                // 如果每行为空，则下一行
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                // 按 [" "，"\t"] 分割每行
                string[] lineValues = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                // 如果是标识，如果arguments.Count不是0，则不是第一个参数
                if (lineValues[0].StartsWith(flagMark) && arguments.Count == 0)
                {
                    type = ScenarioContentType.Flag;
                }

                // 添加内容
                for (int vi = 0; vi < lineValues.Length; vi++)
                {
                    string value = lineValues[vi].Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        arguments.Add(value);
                    }
                }
            }

            // 只有注释
            if (arguments.Count == 0)
            {
                command = null;
                return FormatContentResult.Commenting;
            }

            //// 如果标识参数大于1，则语法错误。
            //// 也可以不在这里判断，而在具体运行代码时判断。
            //if (type == ScenarioContentType.Flag && arguments.Count > 1)
            //{
            //    command = null;
            //    formatError = string.Format("TxtScript FormatError -> syntactic error: {0}", commandText);
            //    return FormatContentResult.Failure;
            //}

            command = new Command(index, type, arguments.ToArray());
            return FormatContentResult.Succeed;
        }
        #endregion

        #region IList<TxtScript.Content> Methods
        public int IndexOf(TxtScript.Command item)
        {
            return m_Commands.IndexOf(item);
        }

        void IList<TxtScript.Command>.Insert(int index, TxtScript.Command item)
        {
            throw new NotImplementedException("Readonly");
        }

        void IList<TxtScript.Command>.RemoveAt(int index)
        {
            throw new NotImplementedException("Readonly");
        }

        void ICollection<TxtScript.Command>.Add(TxtScript.Command item)
        {
            throw new NotImplementedException("Readonly");
        }

        void ICollection<TxtScript.Command>.Clear()
        {
            throw new NotImplementedException("Readonly");
        }

        public bool Contains(TxtScript.Command item)
        {
            return m_Commands.Contains(item as Command);
        }

        public void CopyTo(TxtScript.Command[] array, int arrayIndex)
        {
            m_Commands.CopyTo(array, arrayIndex);
        }

        bool ICollection<TxtScript.Command>.Remove(TxtScript.Command item)
        {
            throw new NotImplementedException("Readonly");
        }

        public IEnumerator<TxtScript.Command> GetEnumerator()
        {
            return m_Commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Commands.GetEnumerator();
        }
        #endregion

        #region Helper
        public override string ToString()
        {
            if (!isLoaded)
            {
                return base.ToString();
            }
            return buffer;
        }

        /// <summary>
        /// 重新建立文本，每个内容默认值为换行分割
        /// </summary>
        /// <param name="commandSeparator"></param>
        /// <param name="withCommenting"></param>
        /// <returns></returns>
        public string RecreateText(string commandSeparator = null, bool withCommenting = false)
        {
            if (commandSeparator == null)
            {
                commandSeparator = Environment.NewLine;
            }
            string[] texts = commands.Select(cmd => cmd.ToString(withCommenting)).ToArray();
            return string.Join(commandSeparator, texts);
        }
        #endregion

    }
}
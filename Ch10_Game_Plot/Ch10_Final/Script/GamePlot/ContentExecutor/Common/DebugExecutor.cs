#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				DebugExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 06 Jan 2019 17:34:12 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class DebugExecutor : ScenarioContentExecutor<DebugExecutor.DebugArgs>
    {
        public struct DebugArgs
        {
            public LogType logType;
            public string message;
        }

#if UNITY_EDITOR
        protected static readonly HashSet<string> s_EditorLowerLogTypeStrings = new HashSet<string>(
            Enum.GetNames(typeof(LogType)).Select(name => name.ToLower()));
#endif

        public override string code
        {
            get { return "debug"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref DebugArgs args, out string error)
        {
#if UNITY_EDITOR
            // debug log "message";
            // debug warning "var = " var;
            // debug error "str0" var0 "str1" var1 var2 var3 "str2"...;
            if (content.length < 3)
            {
                error = GetLengthErrorString();
                return false;
            }

            if (!s_EditorLowerLogTypeStrings.Contains(content[1].ToLower()))
            {
                error = string.Format(
                    "{0} ParseArgs error: LogType `{1}` is not defined.",
                    typeName,
                    content[1]);
                return false;
            }
            args.logType = (LogType)Enum.Parse(typeof(LogType), content[1], true);

            List<string> messages = new List<string>();
            int startIndex = 2;
            int endIndex = 2;
            string str;
            while (startIndex < content.length)
            {
                if (GetNextString(content, startIndex, ref endIndex, out str, out error))
                {
                    messages.Add(str);
                    startIndex = endIndex + 1;
                }
                else
                {
                    return false;
                }
            }
            args.message = string.Join(" ", messages.ToArray());
#endif
            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, DebugArgs args, out string error)
        {
#if UNITY_EDITOR
            string message = args.message;

            switch (args.logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Exception:
                    DebugException e = new DebugException(message);
                    error = e.ToString();
                    Debug.LogException(e);
                    return ActionStatus.Error;
                default:
                    break;
            }
#endif
            error = null;
            return ActionStatus.Continue;
        }

        protected bool GetNextString(IScenarioContent content, int startIndex, ref int endIndex, out string result, out string error)
        {
            string str = content[startIndex];
            if (str.StartsWith("\""))
            {
                str = str.Remove(0, 1);
                if (!string.IsNullOrEmpty(str) && str.EndsWith("\""))
                {
                    endIndex = startIndex;
                    result = str.Remove(str.Length - 1, 1);
                    error = null;
                    return true;
                }

                for (int i = startIndex + 1; i < content.length; i++)
                {
                    string word = content[i];
                    if (string.IsNullOrEmpty(word))
                    {
                        continue;
                    }

                    str += " ";

                    if (word.EndsWith("\""))
                    {
                        str += word.Remove(word.Length - 1, 1);
                        endIndex = i;
                        result = str;
                        error = null;
                        return true;
                    }
                    else
                    {
                        str += word;
                    }
                }

                result = null;
                error = string.Format(
                    "{0} ParseArgs error: missing `\"`.",
                    typeName);
                return false;
            }
            else
            {
                int value = 0;
                if (!ParseOrGetVarValue(str, ref value, out error))
                {
                    result = null;
                    return false;
                }

                endIndex = startIndex;
                result = value.ToString();
                error = null;
                return true;
            }
        }

        public class DebugException : Exception
        {
            public DebugException(string message) : base(message)
            {

            }
        }
    }
}
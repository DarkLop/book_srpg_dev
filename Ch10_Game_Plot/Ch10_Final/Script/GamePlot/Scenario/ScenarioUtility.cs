#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				ScriptUtility.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 01 Jan 2019 22:18:55 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public static class ScenarioUtility
    {
        #region Const
        public const string k_DefaultFlagMark = "#";
        public const string k_DefaultCommandSeparator = ";";
        public const string k_DefaultCommentingPrefix = "//";
        public const string k_Space = " ";
        public const string k_Separator = "\t";
        public const string k_NewLine = "\n";
        #endregion

        #region Format Scenario Content
        /// <summary>
        /// Format the text of content.
        /// </summary>
        /// <param name="contentText"></param>
        /// <param name="separators"></param>
        /// <param name="arguments"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static FormatContentResult FormatContentWithoutType(
            string contentText, 
            string[] separators, 
            out string[] arguments, 
            out string error,
            bool multiline = true,
            bool withCommenting = true,
            string commentingPrefix = k_DefaultCommentingPrefix)
        {
            ScenarioContentType type;

            return FormatContent(
                contentText, 
                separators, 
                out type, 
                out arguments, 
                out error,
                multiline: multiline,
                useFlagMark: false,
                withCommenting: withCommenting,
                commentingPrefix: commentingPrefix);
        }

        /// <summary>
        /// Format the text of content.
        /// </summary>
        /// <param name="contentText"></param>
        /// <param name="flagMark"></param>
        /// <param name="separators"></param>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <param name="error"></param>
        /// <param name="multiline"></param>
        /// <param name="withCommenting"></param>
        /// <param name="commentingPrefix"></param>
        /// <returns></returns>
        public static FormatContentResult FormatContent(
            string contentText,
            string[] separators,
            out ScenarioContentType type,
            out string[] arguments,
            out string error,
            bool multiline = true,
            bool useFlagMark = true,
            string flagMark = k_DefaultFlagMark,
            bool withCommenting = true,
            string commentingPrefix = k_DefaultCommentingPrefix)
        {
            type = ScenarioContentType.Action;
            arguments = null;
            error = null;

            if (string.IsNullOrEmpty(contentText))
            {
                error = "FormatContent -> `argumentsStr` is null or empty";
                return FormatContentResult.Failure;
            }

            contentText = contentText.Trim();
            if (string.IsNullOrEmpty(contentText))
            {
                error = "FormatContent -> `argumentsStr` is null or empty";
                return FormatContentResult.Failure;
            }

            List<string> args = new List<string>();

            if (multiline)
            {
                string[] lineSeparator = new string[] { k_NewLine };
                string[] lines = contentText.Split(lineSeparator, StringSplitOptions.RemoveEmptyEntries);
                for (int li = 0; li < lines.Length; li++)
                {
                    FormatLineInContent(
                        lines[li], separators, 
                        ref type, ref args, 
                        useFlagMark, flagMark,
                        withCommenting, commentingPrefix);
                }
            }
            else
            {
                FormatLineInContent(
                    contentText, separators,
                    ref type, ref args,
                    useFlagMark, flagMark,
                    withCommenting, commentingPrefix);
            }

            if (args.Count == 0)
            {
                return FormatContentResult.Commenting;
            }

            //if (type == ScenarioContentType.Flag && args.Count > 1)
            //{
            //    error = "FormatContent -> syntactic error: flag has some params.";
            //    return FormatContentResult.Failure;
            //}

            arguments = args.ToArray();
            return FormatContentResult.Succeed;
        }

        /// <summary>
        /// Format one line in content.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="flagMark"></param>
        /// <param name="separators"></param>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <param name="withCommenting"></param>
        /// <param name="commentingPrefix"></param>
        /// <returns></returns>
        public static bool FormatLineInContent(
            string line,
            string[] separators,
            ref ScenarioContentType type,
            ref List<string> args,
            bool useFlagMark = true,
            string flagMark = k_DefaultFlagMark,
            bool withCommenting = true,
            string commentingPrefix = k_DefaultCommentingPrefix)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            line = line.Trim();

            if (withCommenting)
            {
                int commentingIndex = line.IndexOf(commentingPrefix);
                if (commentingIndex != -1)
                {
                    line = line.Substring(0, commentingIndex).TrimEnd();
                }

                if (string.IsNullOrEmpty(line))
                {
                    return false;
                }
            }

            string[] lineValues = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (args == null)
            {
                args = new List<string>();
            }

            if (lineValues[0].StartsWith(flagMark) && args.Count == 0)
            {
                type = ScenarioContentType.Flag;
            }

            for (int vi = 0; vi < lineValues.Length; vi++)
            {
                string value = lineValues[vi].Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    args.Add(value);
                }
            }

            return true;
        }
        #endregion

        #region Parse Content String
        /// <summary>
        /// Get string with `"`.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startIndex"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool ParseContentString(IScenarioContent content, ref int startIndex, out string result, out string error)
        {
            if (content == null)
            {
                result = null;
                error = "ParseArgs error: `content` is null.";
                return false;
            }

            if (startIndex >= content.length)
            {
                result = null;
                error = "ParseArgs error: index is out of range.";
                return false;
            }

            string str = content[startIndex];
            if (str.StartsWith("\""))
            {
                str = str.Remove(0, 1);
                if (!string.IsNullOrEmpty(str) && str.EndsWith("\""))
                {
                    result = str.Remove(str.Length - 1, 1);
                    startIndex++;
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
                        startIndex = i + 1;
                        result = str;
                        error = null;
                        return true;
                    }
                    else
                    {
                        str += word;
                    }
                }
            }

            result = null;
            error = "ParseArgs error: missing `\"`.";
            return false;
        }

        /// <summary>
        /// Get strings with `"`.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startIndex"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool ParseContenStrings(IScenarioContent content, ref int startIndex, out List<string> result, out string error)
        {
            if (content == null)
            {
                result = null;
                error = "ParseArgs error: `content` is null.";
                return false;
            }

            if (startIndex >= content.length)
            {
                result = null;
                error = "ParseArgs error: index is out of range.";
                return false;
            }

            result = new List<string>();
            string str;
            while (startIndex < content.length)
            {
                if (content[startIndex].StartsWith("\""))
                {
                    if (!ParseContentString(content, ref startIndex, out str, out error))
                    {
                        result = null;
                        return false;
                    }

                    result.Add(str);
                }
                else
                {
                    break;
                }
            }

            error = null;
            return true;
        }
        #endregion
    }
}
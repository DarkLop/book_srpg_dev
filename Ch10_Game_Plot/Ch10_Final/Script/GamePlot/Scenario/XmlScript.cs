#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				XmlScript.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 31 Dec 2018 03:44:16 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class XmlScript : Scenario<XmlScript.Command>
    {
        #region Class Command
        [Serializable]
        public class Command : IScenarioContent, ICloneable
        {
            #region Fields
            [XmlIgnore, SerializeField]
            private ScenarioContentType m_Type;
            [XmlIgnore, SerializeField, Multiline]
            private string m_ContentText;

            [XmlIgnore, NonSerialized]
            private string[] m_Arguments;
            #endregion

            #region Properties
            /// <summary>
            /// 类型
            /// </summary>
            [XmlAttribute]
            public ScenarioContentType type
            {
                get { return m_Type; }
                set { m_Type = value; }
            }

            /// <summary>
            /// 字符串语句
            /// </summary>
            [XmlElement]
            public string contentText
            {
                get { return m_ContentText; }
                set { m_ContentText = value; }
            }

            /// <summary>
            /// 参数数量
            /// </summary>
            [XmlIgnore]
            public int length
            {
                get { return arguments.Length; }
            }

            /// <summary>
            /// 索引器
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            [XmlIgnore]
            public string this[int index]
            {
                get { return arguments[index]; }
            }

            /// <summary>
            /// 关键字或剧情标识
            /// </summary>
            [XmlIgnore]
            public string code
            {
                get { return arguments[0]; }
            }

            [XmlIgnore]
            public string[] arguments
            {
                get
                {
                    if (m_Arguments != null)
                    {
                        return m_Arguments;
                    }

                    string error;
                    if (ScenarioUtility.FormatContentWithoutType(
                        m_ContentText,
                        new string[] { ScenarioUtility.k_Space, ScenarioUtility.k_Separator },
                        out m_Arguments,
                        out error) == FormatContentResult.Failure)
                    {
                        Debug.LogError("XmlScript.Command -> " + error);
                        return null;
                    }

                    return m_Arguments;
                }
            }
            #endregion

            #region Constructor
            public Command()
            {

            }

            public Command(ScenarioContentType type, string arguments)
            {
                m_Type = type;
                m_ContentText = arguments;
            }
            #endregion

            #region Helper

            public override string ToString()
            {
                return ToString(true);
            }

            public string ToString(bool omitXmlDeclaration, bool indent = true, string indentChars = "  ")
            {
                StringBuilder builder = new StringBuilder();
                if (omitXmlDeclaration)
                {
                    builder.AppendLine("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                }

                XmlWriterSettings setting = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    Indent = indent,
                    IndentChars = indentChars,
                    NewLineChars = Environment.NewLine,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter xw = XmlWriter.Create(builder, setting))
                {
                    WriteXmlString(xw);
                    xw.Flush();
                }

                return builder.ToString().Trim();
            }

            public void WriteXmlString(XmlWriter xw)
            {
                if (xw == null)
                {
                    return;
                }

                xw.WriteStartElement(GetType().Name);
                {
                    xw.WriteStartAttribute("type");
                    xw.WriteValue(type.ToString());
                    xw.WriteEndAttribute();

                    xw.WriteElementString("contentText", contentText);
                }
                xw.WriteEndElement();
            }

            public Command Clone()
            {
                return new Command(type, contentText);
            }

            object ICloneable.Clone()
            {
                return this.Clone();
            }

            public TxtScript.Command ToTxtCommand()
            {
                string[] args = new string[length];
                Array.Copy(arguments, args, length);
                return new TxtScript.Command(type, args);
            }
            #endregion
        }
        #endregion

        #region Class SerializedXmlScript
        [Serializable]
        public class SerializedXmlScript
        {
            #region Fields
            [XmlIgnore, SerializeField]
            private string m_Name;
            [XmlIgnore, SerializeField]
            private string m_FlagMark = ScenarioUtility.k_DefaultFlagMark;
            [XmlIgnore, SerializeField]
            private List<XmlScript.Command> m_Commands = new List<XmlScript.Command>();
            #endregion

            #region Properties
            [XmlAttribute]
            public string name
            {
                get { return m_Name; }
                set { m_Name = value; }
            }

            [XmlAttribute]
            public string flagMark
            {
                get { return m_FlagMark; }
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = ScenarioUtility.k_DefaultFlagMark;
                    }
                    m_FlagMark = value;
                }
            }

            [XmlArray(IsNullable = false), XmlArrayItem(IsNullable = false)]
            public List<XmlScript.Command> commands
            {
                get { return m_Commands; }
                set { m_Commands = value; }
            }
            #endregion

            #region Helper
            public SerializedXmlScript Clone()
            {
                SerializedXmlScript clone = new SerializedXmlScript()
                {
                    name = name,
                    flagMark = flagMark
                };

                if (commands != null)
                {
                    clone.commands = new List<XmlScript.Command>();
                    for (int i = 0; i < commands.Count; i++)
                    {
                        clone.commands.Add(commands[i].Clone());
                    }
                }

                return clone;
            }

            public override string ToString()
            {
                return ToString(true);
            }

            public string ToString(bool omitXmlDeclaration, bool indent = true, string indentChars = "  ")
            {
                StringBuilder builder = new StringBuilder();
                if (omitXmlDeclaration)
                {
                    builder.AppendLine("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                }

                XmlWriterSettings setting = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8,
                    Indent = indent,
                    IndentChars = indentChars,
                    NewLineChars = Environment.NewLine,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter xw = XmlWriter.Create(builder, setting))
                {
                    WriteXmlString(xw);
                    xw.Flush();
                }

                return builder.ToString().Trim();
            }

            public void WriteXmlString(XmlWriter xw)
            {
                if (xw == null)
                {
                    return;
                }

                xw.WriteStartElement(GetType().Name);
                {
                    xw.WriteStartAttribute("name");
                    xw.WriteValue(name);
                    xw.WriteEndAttribute();

                    xw.WriteStartAttribute("flagMark");
                    xw.WriteValue(flagMark);
                    xw.WriteEndAttribute();

                    xw.WriteStartElement("commands");
                    if (commands != null)
                    {
                        for (int i = 0; i < commands.Count; i++)
                        {
                            commands[i].WriteXmlString(xw);
                        }
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }

            public TxtScript ToTxtScript()
            {
                TxtScript script = new TxtScript(flagMark);
                string[] texts = commands.Select(cmd => cmd.contentText).ToArray();
                string scriptText = string.Join(ScenarioUtility.k_NewLine, texts);
                script.Load(name, scriptText);
                return script;
            }

            public bool FromTxtScript(TxtScript script)
            {
                if (script == null)
                {
                    return false;
                }

                name = script.name;
                flagMark = script.flagMark;

                if (script.contentCount > 0)
                {
                    commands = new List<Command>();

                    for (int i = 0; i < script.contentCount; i++)
                    {
                        string argStr = script[i].ToString(true);
                        argStr = argStr.Remove(argStr.Length - 1);
                        Command command = new Command(script[i].type, argStr);
                        commands.Add(command);
                    }
                }

                return true;
            }

            #endregion
        }
        #endregion

        #region Fields
        private SerializedXmlScript m_SerializedXmlScript;
        #endregion

        #region Properties
        /// <summary>
        /// 剧本名（可能为null）
        /// </summary>
        public new string name
        {
            get { return base.name; }
            private set
            {
                base.name = value;
                if (isLoaded)
                {
                    m_SerializedXmlScript.name = value;
                }
            }
        }

        /// <summary>
        /// 用作剧本标识的符号
        /// </summary>
        public string flagMark
        {
            get
            {
                if (!isLoaded)
                {
                    return ScenarioUtility.k_DefaultFlagMark;
                }
                return m_SerializedXmlScript.flagMark;
            }
        }

        protected override IList<XmlScript.Command> contents
        {
            get
            {
                if (!isLoaded)
                {
                    return null;
                }
                return m_SerializedXmlScript.commands;
            }
        }

        public override bool isLoaded
        {
            get { return m_SerializedXmlScript != null; }
        }
        #endregion

        #region Format Methods
        public override void Reset()
        {
            base.Reset();

            m_SerializedXmlScript = null;
        }

        protected override bool FormatScript(string script, out string error)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(script)))
                {
                    using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(SerializedXmlScript));
                        m_SerializedXmlScript = xs.Deserialize(sr) as SerializedXmlScript;
                    }
                }

                error = null;
                return true;
            }
            catch (Exception e)
            {
                error = "XmlScript Load -> " + e.ToString();
                return false;
            }
        }
        #endregion

        #region Helper

        public string ToXmlString(bool indent = true, string indentChars = "  ")
        {
            if (!isLoaded)
            {
                SerializedXmlScript xml = new SerializedXmlScript();
                return xml.ToString(true, indent, indentChars);
            }

            return m_SerializedXmlScript.ToString(true, indent, indentChars);
        }

        public TxtScript ToTxtScript()
        {
            if (!isLoaded)
            {
                return new TxtScript();
            }

            return m_SerializedXmlScript.ToTxtScript();
        }

        public bool FromTxtScript(TxtScript script)
        {
            bool loaded = isLoaded;
            if (!loaded)
            {
                m_SerializedXmlScript = new SerializedXmlScript();
            }

            if (m_SerializedXmlScript.FromTxtScript(script))
            {
                name = m_SerializedXmlScript.name;
                buffer = m_SerializedXmlScript.ToString(true);
            }
            else if (!loaded)
            {
                m_SerializedXmlScript = null;
                return false;
            }

            return true;
        }
        #endregion
    }
}
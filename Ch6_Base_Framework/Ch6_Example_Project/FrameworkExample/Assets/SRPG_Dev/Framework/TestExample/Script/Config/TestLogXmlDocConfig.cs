#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestLogXmlDocConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 11 Mar 2018 00:53:53 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    public class TestLogXmlDocConfig : XmlDocConfigFile
    {
        /// <summary>
        /// 存储的数据
        /// </summary>
        private Dictionary<int, TestLogData> m_DataDict;

        /// <summary>
        /// 数据数量
        /// </summary>
        public static int dataCount
        {
            get
            {
                TestLogXmlDocConfig config = Get<TestLogXmlDocConfig>();
                if (config == null || config.m_DataDict == null)
                {
                    throw new FormatException("Config is not format. Check the console panel.");
                }

                return config.m_DataDict.Count;
            }
        }

        /// <summary>
        /// 文件Info构造器
        /// </summary>
        /// <param name="info"></param>
        protected override void ConstructInfo(ref Info info)
        {
            info.relative = "Xml";
            info.name = "TestLogXmlDocConfig.xml"; //如果是使用Resources读取不要加后缀
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = string.Empty;
        }

        protected override void FormatBuffer(XmlDocument buffer)
        {
            m_DataDict = new Dictionary<int, TestLogData>();
            XmlNodeList datas = buffer.SelectNodes("//TestLog");
            foreach (XmlElement element in datas)
            {
                TestLogData data = new TestLogData
                {
                    id = int.Parse(element.GetAttribute("ID")),
                    log = element.GetAttribute("Log")
                };
                m_DataDict.Add(data.id, data);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TestLogData GetData(int id)
        {
            TestLogXmlDocConfig config = Get<TestLogXmlDocConfig>();

            if (config == null || config.m_DataDict == null)
            {
                throw new FormatException("Config is not formated. Check the console panel.");
            }

            TestLogData data;
            if (!config.m_DataDict.TryGetValue(id, out data))
            {
                Debug.LogErrorFormat(
                    "Data in config '{0}' is not found. ID: {1}.",
                    typeof(TestLogXmlDocConfig).Name,
                    id.ToString()
                    );
                return null;
            }
            return data;
        }
    }
}
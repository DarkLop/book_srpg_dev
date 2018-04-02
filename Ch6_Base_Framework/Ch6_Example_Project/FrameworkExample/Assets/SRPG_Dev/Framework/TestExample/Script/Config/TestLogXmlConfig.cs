#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestLogXmlConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 07 Mar 2018 21:25:34 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{

    /// <summary>
    /// TestLog数据类
    /// </summary>
    [Serializable, XmlRoot("TestLog")]
    public class TestLogData
    {
        [XmlAttribute("ID")]
        public int id { get; set; }
        [XmlAttribute("Log")]
        public string log { get; set; }

        public override string ToString()
        {
            return string.Format("Test Log [ID({0}), Text({1})]", id, log);
        }
    }

    [Serializable, XmlRoot("TestLogXmlConfig")]
	public class TestLogXmlConfig : XmlConfigFile
	{
        /// <summary>
        /// Buffer中反序列化后得到的数据
        /// </summary>
        [XmlArray("Datas"), XmlArrayItem]
        public TestLogData[] m_Datas;

        /// <summary>
        /// 存储的数据
        /// </summary>
        [NonSerialized, XmlIgnore]
        private Dictionary<int, TestLogData> m_DataDict;

        /// <summary>
        /// 数据数量
        /// </summary>
        public static int dataCount
        {
            get
            {
                TestLogXmlConfig config = Get<TestLogXmlConfig>();
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
            info.name = "TestLogXmlConfig.xml"; //如果是使用Resources读取不要加后缀
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = string.Empty;
        }

        /// <summary>
        /// 反序列化完成时执行。
        /// </summary>
        /// <param name="buffer"></param>
        protected override XmlConfigFile FormatBuffer(XmlConfigFile buffer)
        {
            //// 如果你要直接使用m_Datas。请直接返回Buffer
            //return buffer;

            // 将反序列化数据从buffer填充到本config的Dict
            TestLogXmlConfig logBuffer = buffer as TestLogXmlConfig;
            m_DataDict = new Dictionary<int, TestLogData>();
            for (int i = 0; i < logBuffer.m_Datas.Length; i++)
            {
                TestLogData data = logBuffer.m_Datas[i];
                if (!m_DataDict.ContainsKey(data.id))
                {
                    m_DataDict.Add(data.id, data);
                }
            }

            // 这里将buffer的数据填充到dict，buffer将丢弃。
            // 需要注意，buffer的m_Datas属性有数据，而Dict没有数据。
            // 本config，m_Datas没有数据，而Dict有数据。
            // 你也可以把buffer填充，然后直接返回buffer，将本config丢弃。
            return this;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TestLogData GetData(int id)
        {
            TestLogXmlConfig config = Get<TestLogXmlConfig>();

            if (config == null || config.m_DataDict == null)
            {
                throw new FormatException("Config is not formated. Check the console panel.");
            }

            TestLogData data;
            if (!config.m_DataDict.TryGetValue(id, out data))
            {
                Debug.LogErrorFormat(
                    "Data in config '{0}' is not found. ID: {1}.",
                    typeof(TestLogXmlConfig).Name,
                    id.ToString()
                    );
                return null;
            }
            return data;
        }
    }

}
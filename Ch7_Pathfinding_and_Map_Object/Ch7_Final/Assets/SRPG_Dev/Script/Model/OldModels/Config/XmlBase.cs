#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				XmlBase.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 16:33:06 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models.Old
{
    using DR.Book.SRPG_Dev.Framework;

    public interface IConfigKey<TKey>
    {
        TKey key { get; }
    }

    public abstract class XmlBase<T, TKey, TData> : XmlConfigFile
        where T : XmlBase<T, TKey, TData>, new()
        where TData : class, IConfigKey<TKey>, new()
    {
        [XmlArray("Datas"), XmlArrayItem("Data")]
        public TData[] m_Datas;

        protected Dictionary<TKey, TData> m_DataDict;

        public static TData GetData(TKey key)
        {
            T config = Get<T>();
            if (config == null)
            {
                Debug.LogErrorFormat("{0} Format Error.", typeof(T).Name);
                return null;
            }
            return config.GetDataSelf(key);
        }

        public TData GetDataSelf(TKey key)
        {
            TData data;
            if (!m_DataDict.TryGetValue(key, out data))
            {
                Debug.LogErrorFormat("{0} Data is not found. Key: {1}", GetType().Name, key.ToString());
                return null;
            }
            return data;
        }

        protected override void ConstructInfo(ref Info info)
        {
            base.ConstructInfo(ref info);
            info.name = GetType().Name + ".xml";
        }

        protected override XmlConfigFile FormatBuffer(XmlConfigFile buffer)
        {
            T config = buffer as T;
            m_DataDict = new Dictionary<TKey, TData>();
            for (int i = 0; i < config.m_Datas.Length; i++)
            {
                TData info = config.m_Datas[i];
                if (m_DataDict.ContainsKey(info.key))
                {
                    Debug.LogWarningFormat("{0} Exist info. Key: {1}", GetType().Name, info.key.ToString());
                    continue;
                }
                m_DataDict[info.key] = info;
            }
            return this;
        }

        public IEnumerator<KeyValuePair<TKey, TData>> GetEnumerator()
        {
            return m_DataDict.GetEnumerator();
        }
    }
}
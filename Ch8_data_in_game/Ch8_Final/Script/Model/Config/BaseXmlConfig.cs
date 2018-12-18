#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				BaseXmlConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 05 Oct 2018 13:51:38 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Framework;

    [Serializable]
    public abstract class BaseXmlConfig<TKey, TData> : XmlConfigFile, IEditorConfigSerializer
        where TData : class, IConfigData<TKey>
    {
        /// <summary>
        /// 能够序列化的数据
        /// </summary>
        [XmlArray, XmlArrayItem]
        public TData[] datas;

        /// <summary>
        /// 读取后存储的数据
        /// </summary>
        private Dictionary<TKey, TData> m_DataDict = new Dictionary<TKey, TData>();

        /// <summary>
        /// 数据数量
        /// </summary>
        [XmlIgnore]
        public int count
        {
            get { return m_DataDict.Count; }
        }
        
        [XmlIgnore]
        public Dictionary<TKey, TData>.KeyCollection keys
        {
            get { return m_DataDict.Keys; }
        }

        /// <summary>
        /// 所有数据
        /// </summary>
        [XmlIgnore]
        public Dictionary<TKey, TData>.ValueCollection values
        {
            get { return m_DataDict.Values; }
        }

        /// <summary>
        /// 键值读取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [XmlIgnore]
        public TData this[TKey key]
        {
            get
            {
                //TData data = Array.Find(datas, d => d.GetKey().Equals(key));
                //if (data == null)
                //{
                //    Debug.LogErrorFormat("{0} -> Key `{1}` was not found.", GetType().Name, key);
                //}
                //return data;
                TData data;
                if (!m_DataDict.TryGetValue(key, out data))
                {
                    Debug.LogErrorFormat("{0} -> Key `{1}` was not found.", GetType().Name, key);
                    return null;
                }
                return data;
            }
        }

        /// <summary>
        /// 文件信息构造
        /// </summary>
        /// <param name="info"></param>
        protected override void ConstructInfo(ref Info info)
        {
            base.ConstructInfo(ref info);
            info.name = GetType().Name + ".xml";
            info.loadType = LoadType.WWW;
        }

        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected override XmlConfigFile FormatBuffer(XmlConfigFile buffer)
        {
            // 如果直接使用数组或字典序列化，可以直接返回buffer
            // return buffer;

            BaseXmlConfig<TKey, TData> config = buffer as BaseXmlConfig<TKey, TData>;
            foreach (TData data in config.datas)
            {
                if (m_DataDict.ContainsKey(data.GetKey()))
                {
                    Debug.LogWarningFormat("{0} -> Key `{1}` is exist. PASS.", GetType().Name, data.GetKey());
                    continue;
                }
                m_DataDict.Add(data.GetKey(), data);
            }

            return this;
        }

        public IEnumerator<KeyValuePair<TKey, TData>> GetEnumerator()
        {
            return m_DataDict.GetEnumerator();
        }


        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <returns></returns>
        Array IEditorConfigSerializer.EditorGetKeys()
        {
            if (datas == null)
            {
                return default(Array);
            }
            else
            {
                return datas.Select(data => data.GetKey()).ToArray();
            }
        }

        void IEditorConfigSerializer.EditorSortDatas()
        {
            if (datas != null)
            {
                Array.Sort(datas, (data1, data2) =>
                {
                    return data1.GetKey().GetHashCode().CompareTo(data2.GetKey().GetHashCode());
                });
            }
        }

        byte[] IEditorConfigSerializer.EditorSerializeToBytes()
        {
            return XmlUtility.ObjectToXmlBytes(this);
        }

        void IEditorConfigSerializer.EditorDeserializeToObject(byte[] bytes)
        {
            XmlConfigFile config = XmlUtility.ObjectFromXmlBytes(bytes, GetType());
            datas = (config as BaseXmlConfig<TKey, TData>).datas;
        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				BaseTxtConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 07 Oct 2018 23:40:57 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Framework;

    [Serializable]
    public class BaseTxtConfig<TKey, TData> : TxtConfigFile, IEditorConfigSerializer
        where TData : class, ITxtConfigData<TKey>, new()
    {
        /// <summary>
        /// 我们规定，以//开头的为注释
        /// </summary>
        public const string k_CommentingPrefix = "//";

        /// <summary>
        /// 能够序列化的数据，这里只在Editor中有作用
        /// </summary>
        public TData[] datas;

        /// <summary>
        /// 读取后存储的数据
        /// </summary>
        private Dictionary<TKey, TData> m_DataDict = new Dictionary<TKey, TData>();

        /// <summary>
        /// 数据数量
        /// </summary>
        public int count
        {
            get { return m_DataDict.Count; }
        }

        /// <summary>
        /// 所有数据
        /// </summary>
        public Dictionary<TKey, TData>.ValueCollection values
        {
            get { return m_DataDict.Values; }
        }

        /// <summary>
        /// 键值读取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TData this[TKey key]
        {
            get
            {
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
            info.name = GetType().Name + ".txt";
            info.loadType = LoadType.WWW;
        }

        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected override void FormatBuffer(string buffer)
        {
            // 分割行，并删除空行
            string[] lines = buffer.Split(
                new string[] { Environment.NewLine }, 
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                
                // 如果是注释，直接下一条
                if (line.StartsWith(k_CommentingPrefix))
                {
                    continue;
                }

                // 创建并格式化行数据
                TData data = new TData();
                if (!data.FormatText(line))
                {
                    continue;
                }

                if (m_DataDict.ContainsKey(data.GetKey()))
                {
                    Debug.LogWarningFormat("{0} -> Key `{1}` is exist. PASS.", GetType().Name, data.GetKey());
                    continue;
                }
                m_DataDict.Add(data.GetKey(), data);
            }
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

        public virtual byte[] EditorSerializeToBytes()
        {
            if (datas == null)
            {
                datas = new TData[0];
            }

            StringBuilder builder = new StringBuilder();

            // 反射获取所有public字段
            Type dataType = typeof(TData);
            FieldInfo[] fields = dataType.GetFields(BindingFlags.Instance 
                | BindingFlags.Public 
                | BindingFlags.GetField 
                | BindingFlags.SetField);

            if (fields.Length != 0)
            {
                // 每一列的名字
                string[] line = fields.Select(field => field.Name).ToArray();
                builder.AppendLine(k_CommentingPrefix + string.Join("\t", line));

                // 每一行数据
                for (int i = 0; i < datas.Length; i++)
                {
                    line = fields.Select(field => field.GetValue(datas[i]).ToString()).ToArray();
                    builder.AppendLine(string.Join("\t", line));
                }
            }
            return Encoding.UTF8.GetBytes(builder.ToString().Trim());
        }

        /// <summary>
        /// editor only
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void EditorDeserializeToObject(byte[] bytes)
        {
            string buffer = Encoding.UTF8.GetString(bytes).Trim();
            // 分割行
            string[] lines = buffer.Split(
                new string[] { Environment.NewLine }, 
                StringSplitOptions.RemoveEmptyEntries);

            List<TData> loadedDatas = new List<TData>();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                // 如果是注释，直接下一条
                if (line.StartsWith(k_CommentingPrefix))
                {
                    continue;
                }

                TData data = new TData();
                if (data.FormatText(line))
                {
                    loadedDatas.Add(data);
                }
            }

            datas = loadedDatas.ToArray();
        }
    }
}
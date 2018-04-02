#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ConfigFile.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 01:19:21 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.IO;

namespace DR.Book.SRPG_Dev.Framework
{
    /// <summary>
    /// 所有Config的基类。
    /// 参考其它继承它的类，可以添加任何类型文件。
    /// 比如protobuf，csv，excel等文件。
    /// </summary>
    public abstract class ConfigFile
    {
        #region Enum LoadType
        /// <summary>
        /// 文件读取方式
        /// </summary>
        [Flags]
        public enum LoadType
        {
            /// <summary>
            /// Unity Resources文件夹
            /// </summary>
            Resources = 0x01,

            /// <summary>
            /// 本地WWW
            /// </summary>
            WWW = 0x02,

            /// <summary>
            /// 需要与其它方式配合，不能单独使用
            /// </summary>
            AssetBundle = 0x04,

            /// <summary>
            /// 网络WWW
            /// </summary>
            WWWInternet = 0x08,
        }
        #endregion

        #region Struct Info
        protected struct Info
        {
            /// <summary>
            /// 相对路径，不要以'/'或'\'开始。
            /// 如果LoadType包含AssetBundle，这里指AssetBundle相对路径。
            /// </summary>
            public string relative { get; set; }

            /// <summary>
            /// 名称与扩展名，Resources与AssetBundle不需要扩展名。
            /// 如果LoadType包含AssetBundle，这里指AssetBundle名字。
            /// Resources支持的文本类型，请查看官方文档。
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 读取方式
            /// </summary>
            public LoadType loadType { get; set; }

            /// <summary>
            /// 如果LoadType包含AssetBundle，AssetBundle内的路径
            /// </summary>
            public string pathInAssetBundle { get; set; }

            /// <summary>
            /// 获取相对路径+文件名称+扩展名
            /// </summary>
            public string relativePath
            {
                get { return Path.Combine(relative, name); }
            }
        }
        #endregion

        #region Field/Property
        private Info m_Info;

        public string relativePath
        {
            get { return m_Info.relativePath; }
        }

        public LoadType loadType
        {
            get { return m_Info.loadType; }
        }

        public string pathInAssetBundle
        {
            get { return m_Info.pathInAssetBundle; }
        }
        #endregion

        #region Constructor
        protected ConfigFile()
        {
            ConstructInfo(ref m_Info);
        }

        /// <summary>
        /// 初始化Info构造器
        /// </summary>
        /// <param name="info"></param>
        protected abstract void ConstructInfo(ref Info info);
        #endregion

        #region Method
        /// <summary>
        /// Format数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bytes"></param>
        /// <param name="config"></param>
        protected abstract void Format(Type type, byte[] bytes, ref ConfigFile config);
        #endregion

        #region Static Field/Property/Method
        /// <summary>
        /// 保存所有Config文件
        /// </summary>
        private static readonly Dictionary<Type, ConfigFile> s_ConfigDict = new Dictionary<Type, ConfigFile>();

        /// <summary>
        /// 获取Config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : ConfigFile
        {
            Type type = typeof(T);
            ConfigFile config;
            GetInternal(type, out config);
            return config as T;
        }

        /// <summary>
        /// 读取Config，并加入到Dict
        /// </summary>
        /// <param name="type"></param>
        /// <param name="config"></param>
        internal static void GetInternal(Type type, out ConfigFile config)
        {
            if (type.IsAbstract)
            {
                throw new ArgumentException("[LoadConfig] throw exception. Type of config is abstract.");
            }

            if (!s_ConfigDict.TryGetValue(type, out config))
            {
                config = Activator.CreateInstance(type) as ConfigFile;
                byte[] configBytes = ConfigLoader.LoadConfigBytesInternal(config);
                if (configBytes == null)
                {
                    throw new Exception("Load config bytes error. Check the console panel.");
                }
                config.Format(type, configBytes, ref config);
                if (config == null)
                {
                    throw new Exception("Format config error. Check the console panel.");
                }
                s_ConfigDict.Add(type, config);
            }
        }

        /// <summary>
        /// 丢弃Config, 如果要再次使用，需要重新读取文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Release<T>() where T : ConfigFile
        {
            s_ConfigDict.Remove(typeof(T));
        }

        /// <summary>
        /// 丢弃所有Config，再次使用，需要重新读取文件
        /// </summary>
        public static void ReleaseAll()
        {
            s_ConfigDict.Clear();
        }
        #endregion
    }
}
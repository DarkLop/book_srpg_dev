#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ConfigLoader.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 01:34:04 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.IO;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    /// <summary>
    /// 读取Config的方法。
    /// 需要注意，这里网络加载没有设置超时属性。
    /// 如果要使用网络，需要添加超时参数，和异步处理。
    /// </summary>
    public static class ConfigLoader
    {
        private static string s_RootDirectory = "Config";

        /// <summary>
        /// Config所在的根目录
        /// </summary>
        public static string rootDirectory
        {
            get { return s_RootDirectory; }
            set { s_RootDirectory = value; }
        }

        /// <summary>
        /// 一次性读取多个Config
        /// </summary>
        /// <param name="types"></param>
        public static void LoadConfig(params Type[] types)
        {
            if (types != null)
            {
                ConfigFile config;
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] != null)
                    {
                        if (!types[i].IsSubclassOf(typeof(ConfigFile)))
                        {
                            Debug.LogErrorFormat(
                                "[LoadConfig] Type({1}) of config is not a sub class of 'ConfigFile'.",
                                types[i].FullName
                                );
                            continue;
                        }
                        ConfigFile.GetInternal(types[i], out config);
                    }
                }
            }
        }

        /// <summary>
        /// 从文件或网络读取Config的Bytes
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static byte[] LoadConfigBytesInternal(ConfigFile config)
        {
            string path = Path.Combine(rootDirectory, config.relativePath);

            byte[] bytes;

            switch (config.loadType)
            {
                // 使用Resources读取Bytes
                case ConfigFile.LoadType.Resources:
                    {
                        TextAsset asset = Resources.Load<TextAsset>(path);
                        if (asset == null)
                        {
                            Debug.LogError("Resources: Config is not found. Path: " + path);
                            return null;
                        }
                        bytes = asset.bytes;
                        break;
                    }
                // 使用WWW从本地读取Bytes
                case ConfigFile.LoadType.WWW:
                    {
                        LoadConfigBytesFromWWW(path, false, out bytes);
                        break;
                    }
                // 使用WWW从网络读取Bytes
                case ConfigFile.LoadType.WWWInternet:
                    {
                        LoadConfigBytesFromWWW(path, true, out bytes);
                        break;
                    }
                // 使用Resources读取Config所在AssetBundle，从AssetBundle中读取Bytes
                case ConfigFile.LoadType.Resources | ConfigFile.LoadType.AssetBundle:
                    {
                        AssetBundle bundle = Resources.Load<AssetBundle>(path);
                        if (bundle == null)
                        {
                            Debug.LogError("Resources: AssetBundle is not found. Path: " + path);
                            return null;
                        }

                        LoadConfigBytesFromAssetBundle(bundle, config.pathInAssetBundle, out bytes);
                        bundle.Unload(false);
                        break;
                    }
                // 使用WWW读取本地Config所在AssetBundle，从AssetBundle中读取Bytes
                case ConfigFile.LoadType.WWW | ConfigFile.LoadType.AssetBundle:
                    {
                        AssetBundle bundle = LoadAssetBundleFromWWW(path, false);
                        if (bundle == null)
                        {
                            Debug.LogError("WWW: AssetBundle is not found. Path: " + path);
                            return null;
                        }

                        LoadConfigBytesFromAssetBundle(bundle, config.pathInAssetBundle, out bytes);
                        bundle.Unload(false);
                        break;
                    }
                // 使用WWW读取网络Config所在AssetBundle，从AssetBundle中读取Bytes
                case ConfigFile.LoadType.WWWInternet | ConfigFile.LoadType.AssetBundle:
                    {
                        AssetBundle bundle = LoadAssetBundleFromWWW(path, true);
                        if (bundle == null)
                        {
                            Debug.LogError("WWWInternet: AssetBundle is not found. Path: " + path);
                            return null;
                        }

                        LoadConfigBytesFromAssetBundle(bundle, config.pathInAssetBundle, out bytes);
                        bundle.Unload(false);
                        break;
                    }
                // 不支持的读取类型，抛出异常
                default:
                    throw new NotSupportedException("Load Type '" + config.loadType.ToString() + "' is not supported.");
            }

            return bytes;
        }

        /// <summary>
        /// 使用WWW读取Bytes
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isInternet"></param>
        /// <param name="bytes"></param>
        private static void LoadConfigBytesFromWWW(string path, bool isInternet, out byte[] bytes)
        {
            FixRuntimeWWWPath(ref path, isInternet);
            using (WWW www = new WWW(path))
            {
                while (!www.isDone)
                {

                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogErrorFormat(
                        "WWW: {0}, Path: {1}.",
                        www.error,
                        path
                        );
                    www.Dispose();
                    bytes = null;
                    return;
                }

                bytes = www.bytes;
            }
        }

        /// <summary>
        /// 使用WWW读取AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isInternet"></param>
        /// <returns></returns>
        private static AssetBundle LoadAssetBundleFromWWW(string path, bool isInternet)
        {
            AssetBundle bundle;
            FixRuntimeWWWPath(ref path, isInternet);
            using (WWW www = new WWW(path))
            {
                while (!www.isDone)
                {

                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogErrorFormat(
                        "WWW: {0}, Path: {1}.",
                        www.error,
                        path
                        );
                    www.Dispose();
                    return null;
                }

                bundle = www.assetBundle;
            }
            return bundle;
        }

        /// <summary>
        /// 从AssetBundle中读取Bytes
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        private static void LoadConfigBytesFromAssetBundle(AssetBundle bundle, string path, out byte[] bytes)
        {
            TextAsset asset = bundle.LoadAsset<TextAsset>(path);
            if (asset == null)
            {
                bytes = null;
                Debug.LogErrorFormat(
                    "AssetBundle: Config is not found in AssetBundle({0}). Path: {1}", 
                    bundle.name, 
                    path);
                return;
            }

            bytes = asset.bytes;
        }

        /// <summary>
        /// 修复WWW的Path前缀的错误
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isInternet"></param>
        private static void FixRuntimeWWWPath(ref string path, bool isInternet)
        {
            path = path.Trim();
            if (isInternet)
            {
                if (!path.StartsWith("http://"))
                {
                    path = "http://" + path;
                }
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (path.Contains(Application.dataPath))
                    {
                        if (!path.StartsWith("jar:file://"))
                        {
                            if (path.StartsWith("file://"))
                            {
                                path = "jar:" + path;
                            }
                            else
                            {
                                path = "jar:file://" + path;
                            }
                        }
                    }
                    else
                    {
                        if (!path.StartsWith("file://"))
                        {
                            path = "file://" + path;
                        }
                    }
                }
                else
                {
                    if (!path.StartsWith("file://"))
                    {
                        path = "file://" + path;
                    }
                }
            }
        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				SwapTextureCache.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 01 Feb 2018 00:45:12 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ColorPalette
{
    /// <summary>
    /// SwapTexture缓存
    /// </summary>
    public static class SwapTextureCache
    {
        /// <summary>
        /// 缓存数据唯一标识
        /// </summary>
        private struct CacheKey
        {
            /// <summary>
            /// Color Chart的名字
            /// </summary>
            public string chartName;

            /// <summary>
            /// 源Texture的名字
            /// </summary>
            public string textureName;
        }

        /// <summary>
        /// 缓存Dictionary
        /// </summary>
        private static Dictionary<CacheKey, Texture2D> s_CacheDict;

        /// <summary>
        /// 缓存Texture的数量
        /// </summary>
        public static int count
        {
            get
            {
                if (s_CacheDict == null)
                {
                    return 0;
                }
                return s_CacheDict.Count;
            }
        }

        /// <summary>
        /// 尝试获取缓存数据
        /// </summary>
        /// <param name="chartName"></param>
        /// <param name="textureName"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static bool TryGetTexture2D(string chartName, string textureName, out Texture2D cache)
        {
            cache = null;
            if (s_CacheDict == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(chartName) || string.IsNullOrEmpty(textureName))
            {
                return false;
            }

            CacheKey key = new CacheKey()
            {
                chartName = chartName,
                textureName = textureName
            };

            if(!s_CacheDict.TryGetValue(key, out cache))
            {
                return false;
            }

            // 如果为null，成功取出，但在别处意外的被Destroy了，删除缓存
            if (cache == null)
            {
                s_CacheDict.Remove(key);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将SwapTexture添加进缓存
        /// </summary>
        /// <param name="chartName"></param>
        /// <param name="textureName"></param>
        /// <param name="swapTexture"></param>
        /// <returns></returns>
        public static bool AddTexture2D(string chartName, string textureName, Texture2D swapTexture)
        {
            if (string.IsNullOrEmpty(chartName) || string.IsNullOrEmpty(textureName) || swapTexture == null)
            {
                return false;
            }

            CacheKey key = new CacheKey()
            {
                chartName = chartName,
                textureName = textureName
            };

            if (s_CacheDict == null)
            {
                s_CacheDict = new Dictionary<CacheKey, Texture2D>();
            }

            if (s_CacheDict.ContainsKey(key))
            {
                return false;
            }

            s_CacheDict.Add(key, swapTexture);
            return true;
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public static void Clear()
        {
            if (s_CacheDict == null)
            {
                return;
            }

            List<CacheKey> keys = new List<CacheKey>(s_CacheDict.Keys);
            foreach (CacheKey key in keys)
            {
                if (s_CacheDict[key] != null)
                {
                    Texture2D.Destroy(s_CacheDict[key]);
                }
                s_CacheDict.Remove(key);
            }

            if (s_CacheDict.Count == 0)
            {
                s_CacheDict = null;
            }
        }
    }
}
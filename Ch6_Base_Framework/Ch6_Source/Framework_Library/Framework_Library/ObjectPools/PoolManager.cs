#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				PoolManager.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 23:28:04 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    public class PoolManager : Singleton<PoolManager>
    {
        public const string k_PoolParentName = "(singleton)PoolParent";

        #region Field
        public GameObject m_DontDestroyParent;
        private readonly Dictionary<string, ObjectPool> m_PoolDict = new Dictionary<string, ObjectPool>();
        #endregion

        #region Property
        /// <summary>
        /// 池数量
        /// </summary>
        public static int poolCount
        {
            get { return instance.m_PoolDict.Count; }
        }

        /// <summary>
        /// DontDestroyOnLoad的父对象
        /// </summary>
        public GameObject dontDestroyParent
        {
            get
            {
                if (m_DontDestroyParent == null)
                {
                    m_DontDestroyParent = new GameObject(k_PoolParentName);
                    m_DontDestroyParent.AddComponent<DontDestroyGameObject>();
                }
                return m_DontDestroyParent;
            }
        }

        /// <summary>
        /// pool索引器
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public ObjectPool this[string poolName]
        {
            get { return instance.m_PoolDict[poolName]; }
        }
        #endregion

        #region Contains Method
        public bool Contains(string poolName)
        {
            return m_PoolDict.ContainsKey(poolName);
        }
        #endregion

        #region Create Method
        public static ObjectPool CreatePool(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogError("[PoolManager] Create pool error. Augument named 'poolName' is null or empty.");
                return null;
            }

            if (instance.m_PoolDict.ContainsKey(poolName))
            {
                Debug.LogError("[PoolManager] Create pool error. Pool name is exist.");
                return null;
            }

            GameObject go = new GameObject(poolName);
            ObjectPool pool = go.AddComponent<ObjectPool>();
            return pool;
        }
        #endregion

        #region Destroy Method
        public static void DestroyPool(string poolName)
        {
            if (instance.m_PoolDict.ContainsKey(poolName))
            {
                Debug.LogErrorFormat("[PoolManager] Destroy pool error. Pool is not found. Name: {0}", poolName);
                return;
            }

            GameObject.Destroy(instance.m_PoolDict[poolName].gameObject);
        }
        #endregion

        #region Add/Remove Method
        internal bool AddPoolInternal(ObjectPool pool)
        {
            if (m_PoolDict.ContainsKey(pool.poolName))
            {
                Debug.LogError("[PoolManager] Pool is exist. Add pool error.");
                return false;
            }

            if (pool.dontDestroy)
            {
                pool.transform.SetParent(dontDestroyParent.transform, false);
            }

            m_PoolDict.Add(pool.poolName, pool);
            return true;
        }

        internal bool RemovePoolInternal(ObjectPool pool)
        {
            if (string.IsNullOrEmpty(pool.poolName))
            {
                Debug.LogError("[PoolManager] Pool name is null. Remove pool error.");
                return false;
            }

            return m_PoolDict.Remove(pool.poolName);
        }
        #endregion

        protected override void OnDispose()
        {
            ObjectPool[] pools = new ObjectPool[m_PoolDict.Count];
            m_PoolDict.Values.CopyTo(pools, 0);
            foreach (ObjectPool pool in pools)
            {
                GameObject.DestroyImmediate(pool.gameObject);
            }

            if (m_DontDestroyParent != null)
            {
                GameObject.Destroy(m_DontDestroyParent);
                m_DontDestroyParent = null;
            }
        }
    }
}
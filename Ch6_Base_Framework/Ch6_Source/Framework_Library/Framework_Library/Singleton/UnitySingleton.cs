#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				UnitySingleton.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 15:13:19 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_Instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static T instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = GameObject.FindObjectOfType<T>();
                    if (s_Instance == null)
                    {
                        GameObject go = new GameObject("(singeton)" + typeof(T).Name);
                        s_Instance = go.AddComponent<T>();
                    }

                    if (s_Instance.GetComponent<DontDestroyGameObject>() == null)
                    {
                        s_Instance.gameObject.AddComponent<DontDestroyGameObject>();
                    }
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// 是否需要重新建立Instance
        /// </summary>
        public static bool needCreated
        {
            get { return s_Instance == null; }
        }


        /// <summary>
        /// 立即销毁单例
        /// </summary>
        public static void DestroyInstance()
        {
            if (s_Instance != null)
            {
                GameObject.DestroyImmediate(s_Instance.gameObject);
            }
        }

        #region Unity Callback
        protected virtual void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this as T;
            }
            else if (s_Instance != this)
            {
                MonoBehaviour.DestroyImmediate(this);
                return;
            }

            if (s_Instance.GetComponent<DontDestroyGameObject>() == null)
            {
                s_Instance.gameObject.AddComponent<DontDestroyGameObject>();
            }
        }

        protected virtual void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        #endregion
    }
}
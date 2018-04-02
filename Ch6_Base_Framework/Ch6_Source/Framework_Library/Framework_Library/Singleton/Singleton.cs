#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Singleton.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 15:06:14 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class Singleton<T> : IDisposable where T : Singleton<T>, new()
    {
        /// <summary>
        /// 用于线程锁定
        /// </summary>
        private static object s_lock = new object();

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
                    lock (s_lock)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new T();
                        }
                    }
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// 释放单例内存空间
        /// </summary>
        public static void Release()
        {
            if (s_Instance != null)
            {
                ((IDisposable)s_Instance).Dispose();
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        protected Singleton()
        {
            OnConstruct();
        }

        /// <summary>
        /// 用于override的构造器
        /// </summary>
        protected virtual void OnConstruct()
        {

        }

        /// <summary>
        /// 释放内存空间
        /// </summary>
        void IDisposable.Dispose()
        {
            OnDispose();
            s_Instance = null;
        }

        /// <summary>
        /// 用于override的释放内存空间
        /// </summary>
        protected virtual void OnDispose()
        {

        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ViewManagerBase.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 08 Mar 2018 19:13:26 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    /// <summary>
    /// 单例，ViewManager基类，核心算法ViewDictionary已被分离成单独的类。
    /// 你可以不使用它，而只使用继承的ViewDictionary，自己建立UIManager。
    /// 核心算法全部在ViewDictionary中。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TDictionary"></typeparam>
    public abstract class ViewManagerBase<T, TDictionary> : UnitySingleton<T> 
        where T : ViewManagerBase<T, TDictionary> 
        where TDictionary : ViewDictionary
    {
        private static TDictionary s_Views;
        /// <summary>
        /// View核心
        /// </summary>
        public static TDictionary views
        {
            get
            {
                if (s_Views == null)
                {
                    s_Views = instance.CreateViewDictionary();
                    s_Views.gameObject = instance.gameObject;
                    s_Views.prefabDirectory = prefabDirectory;
                }
                return s_Views;
            }
        }

        [SerializeField, Tooltip("Root path of prefab.")]
        private string m_PrefabDirectory = "Prefab/UI/";
        /// <summary>
        /// Prefab根路径
        /// </summary>
        public static string prefabDirectory
        {
            get { return instance.m_PrefabDirectory; }
            set
            {
                instance.m_PrefabDirectory = value;
                if (s_Views != null)
                {
                    s_Views.prefabDirectory = value;
                }
            }
        }

        /// <summary>
        /// 创建核心算法的Dictionary
        /// </summary>
        /// <returns></returns>
        protected virtual TDictionary CreateViewDictionary()
        {
            Type type = typeof(TDictionary);
            if (type.IsAbstract)
            {
                string error = string.Format("[ViewManagerBase] Type named '{0}' is abstract.", type.FullName);
                throw new TypeLoadException(error);
            }
            return Activator.CreateInstance(type) as TDictionary;
        }

        /// <summary>
        /// 释放空间
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (s_Views != null)
            {
                s_Views.Dispose();
                s_Views = null;
            }
        }
    }
}
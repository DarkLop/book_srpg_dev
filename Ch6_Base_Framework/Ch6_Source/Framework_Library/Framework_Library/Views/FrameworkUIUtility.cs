#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FrameworkUIUtility.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 08 Mar 2018 00:23:32 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;
using UnityEngine.EventSystems;

namespace DR.Book.SRPG_Dev.Framework
{
    public static class FrameworkUIUtility
    {
        /// <summary>
        /// 寻找或新建EventSystem
        /// </summary>
        /// <returns></returns>
        public static EventSystem FindOrCreateEventSystem()
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystem = go.GetComponent<EventSystem>();
            }
            return eventSystem;
        }
    }
}
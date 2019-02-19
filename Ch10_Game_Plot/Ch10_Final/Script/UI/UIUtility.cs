#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				UIUtility.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 28 Dec 2018 23:08:10 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.UI
{
    public static class UIUtility
    {
        public static T FindChildComponent<T>(this MonoBehaviour ui, string name) where T : Component
        {
            if (ui == null)
            {
                return null;
            }

            return ui.transform.Find(name).GetComponent<T>();
        }

        public static T FindChildComponetRecursion<T>(this MonoBehaviour ui, string name) where T : Component
        {
            if (ui == null)
            {
                return null;
            }

            if (ui.transform.childCount == 0)
            {
                return null;
            }

            Transform transform = ui.transform;

            T find = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                find = FindChildRecursion<T>(transform, name);
                if (find != null)
                {
                    break;
                }
            }

            return find;
        }

        private static T FindChildRecursion<T>(Transform transform, string name) where T : Component
        {
            T find = null;

            if (transform.name == name)
            {
                find = transform.GetComponent<T>();
            }

            if (find == null && transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    find = FindChildRecursion<T>(child, name);
                    if (find != null)
                    {
                        break;
                    }
                }
            }

            return find;
        }
    }
}
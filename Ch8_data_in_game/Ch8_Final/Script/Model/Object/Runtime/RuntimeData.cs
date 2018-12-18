#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				RuntimeData.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 14 Oct 2018 14:10:26 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Reflection;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    [Serializable]
    public abstract class RuntimeData<T> : IRuntimeData<T> where T : RuntimeData<T>, new()
    {
        public virtual T Clone()
        {
            T clone = new T();
            CopyTo(clone);
            return clone;
        }

        public virtual void CopyTo(T data)
        {
            if (data == null)
            {
                Debug.LogError("RuntimeData -> CopyTo: data is null.");
                return;
            }

            if (data == this)
            {
                return;
            }

            Type type = GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.GetField
                | BindingFlags.SetField);
            for (int i = 0; i < fields.Length; i++)
            {
                object val = fields[i].GetValue(this);
                fields[i].SetValue(data, val);
            }
        }

        object ICloneable.Clone()
        {
            return ((IRuntimeData<T>)this).Clone();
        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ModelManager.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 15:59:58 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;

namespace DR.Book.SRPG_Dev.Models.Old
{
    public static class ModelManager
    {
        public static readonly ModelDictionary models = new ModelDictionary();
    }

    public class ModelDictionary : IDictionary<Type, IModel>
    {
        #region Field
        private Dictionary<Type, IModel> m_ModelDict = new Dictionary<Type, IModel>();
        #endregion

        #region Method
        public T Get<T>() where T : class, IModel, new()
        {
            Type type = typeof(T);
            IModel model;
            if (!m_ModelDict.TryGetValue(type, out model))
            {
                model = Activator.CreateInstance<T>();
                model.Load();
                m_ModelDict.Add(type, model);
            }
            return model as T;
        }

        public void RegisterModel<T>() where T : class, IModel, new()
        {
            Type type = typeof(T);
            if (!m_ModelDict.ContainsKey(type))
            {
                IModel model = Activator.CreateInstance<T>();
                model.Load();
                m_ModelDict.Add(type, model);
            }
        }

        public void UnregisterModel<T>() where T : class, IModel, new()
        {
            Type type = typeof(T);
            IModel model;
            if (m_ModelDict.TryGetValue(type, out model))
            {
                model.Dispose();
                m_ModelDict.Remove(type);
            }
        }
        #endregion

        #region IDictionary<Type, ModelBase> Interface
        public IModel this[Type key]
        {
            get { return m_ModelDict[key]; }
            set { throw new NotImplementedException("Read Only"); }
        }

        public ICollection<Type> Keys
        {
            get { return m_ModelDict.Keys; }
        }

        public ICollection<IModel> Values
        {
            get { return m_ModelDict.Values; }
        }

        public int Count
        {
            get { return m_ModelDict.Count; }
        }

        bool ICollection<KeyValuePair<Type, IModel>>.IsReadOnly
        {
            get { return true; }
        }

        void IDictionary<Type, IModel>.Add(Type key, IModel value)
        {
            throw new NotImplementedException("Not Supported.");
        }

        void ICollection<KeyValuePair<Type, IModel>>.Add(KeyValuePair<Type, IModel> item)
        {
            throw new NotImplementedException("Not Supported.");
        }

        void ICollection<KeyValuePair<Type, IModel>>.Clear()
        {
            throw new NotImplementedException("Not Supported.");
        }

        bool ICollection<KeyValuePair<Type, IModel>>.Contains(KeyValuePair<Type, IModel> item)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public bool ContainsKey(Type key)
        {
            return m_ModelDict.ContainsKey(key);
        }

        void ICollection<KeyValuePair<Type, IModel>>.CopyTo(KeyValuePair<Type, IModel>[] array, int arrayIndex)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public IEnumerator<KeyValuePair<Type, IModel>> GetEnumerator()
        {
            return m_ModelDict.GetEnumerator();
        }

        bool IDictionary<Type, IModel>.Remove(Type key)
        {
            throw new NotImplementedException("Not Supported.");
        }

        bool ICollection<KeyValuePair<Type, IModel>>.Remove(KeyValuePair<Type, IModel> item)
        {
            throw new NotImplementedException("Not Supported.");
        }

        public bool TryGetValue(Type key, out IModel value)
        {
            return m_ModelDict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_ModelDict.GetEnumerator();
        }
        #endregion
    }
}
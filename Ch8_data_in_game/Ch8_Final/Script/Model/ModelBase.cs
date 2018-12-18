#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ModelBase.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 07 Apr 2018 00:33:41 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    public class ModelBase : IModel
    {
        private bool m_Loaded = false;

        public bool loaded
        {
            get { return m_Loaded; }
        }

        void IModel.Load()
        {
            if (m_Loaded)
            {
                return;
            }

            OnLoad();
            m_Loaded = true;
        }

        protected virtual void OnLoad()
        {

        }


        void IDisposable.Dispose()
        {
            if (!m_Loaded)
            {
                return;
            }

            OnDispose();
            m_Loaded = false;
        }

        protected virtual void OnDispose()
        {

        }
    }
}
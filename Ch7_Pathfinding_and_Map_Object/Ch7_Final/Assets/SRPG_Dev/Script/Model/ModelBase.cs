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


namespace DR.Book.SRPG_Dev.Models.Old
{
    public class ModelBase : IModel
    {
        private bool m_Loaded = false;

        public bool loaded
        {
            get { return m_Loaded; }
        }

        public virtual void Dispose()
        {

        }

        public void Load()
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
    }
}
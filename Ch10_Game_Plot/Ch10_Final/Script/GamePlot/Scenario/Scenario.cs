#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Scenario.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 31 Dec 2018 05:51:20 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public abstract class Scenario : IScenario
    {
        #region Fields
        private string m_Name = string.Empty;
        private string m_Buffer = string.Empty;
        protected string m_FormatError = null;
        #endregion

        #region Properties
        /// <summary>
        /// File name.
        /// </summary>
        public string name
        {
            get { return m_Name; }
            protected set { m_Name = value; }
        }

        /// <summary>
        /// Scenario buffer.
        /// </summary>
        public string buffer
        {
            get { return m_Buffer; }
            protected set { m_Buffer = value; }
        }

        /// <summary>
        /// Error message.
        /// </summary>
        public string formatError
        {
            get { return m_FormatError; }
            protected set { m_FormatError = value; }
        }

        /// <summary>
        /// If file is loaded.
        /// </summary>
        public virtual bool isLoaded
        {
            get { return !string.IsNullOrEmpty(m_Buffer); }
        }

        /// <summary>
        /// The number of `contents`.
        /// </summary>
        public abstract int contentCount { get; }

        /// <summary>
        /// Get the content with index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract IScenarioContent GetContent(int index);
        #endregion

        #region Load, Reset and Format Methods
        /// <summary>
        /// Format scenario with text.
        /// </summary>
        /// <param name="scriptName"></param>
        /// <param name="scriptText"></param>
        /// <returns></returns>
        public virtual bool Load(string scriptName, string scriptText)
        {
            string script = Regex.Unescape(scriptText).Trim();

            if (string.IsNullOrEmpty(script))
            {
                formatError = "Scenario Load -> `scriptText` is null or empty";
                return false;
            }

            Reset();

            bool loaded = FormatScript(script, out m_FormatError);
            if (loaded)
            {
                name = scriptName;
                buffer = script;
            }

            return loaded;
        }

        /// <summary>
        /// Reset the vars
        /// </summary>
        public virtual void Reset()
        {
            name = string.Empty;
            buffer = string.Empty;
            formatError = null;
        }

        /// <summary>
        /// Format the script text.
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        protected abstract bool FormatScript(string script, out string error);
        #endregion

        #region Helper
        public override string ToString()
        {
            if (!isLoaded)
            {
                return base.ToString();
            }

            return buffer;
        }
        #endregion
    }

    public abstract class Scenario<T> : Scenario, IList<T> where T : class, IScenarioContent
    {
        #region Properties
        /// <summary>
        /// List of `T`.
        /// </summary>
        protected abstract IList<T> contents { get; }

        /// <summary>
        /// The number of `contents`.
        /// </summary>
        public override int contentCount
        {
            get
            {
                if (!isLoaded || contents == null)
                {
                    return 0;
                }
                return contents.Count;
            }
        }

        /// <summary>
        /// Get the content with index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override IScenarioContent GetContent(int index)
        {
            if (!isLoaded || contents == null)
            {
                return null;
            }
            return contents[index];
        }
        #endregion

        #region IList<T> Properties
        int ICollection<T>.Count
        {
            get { return contentCount; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                if (contents == null)
                {
                    return true;
                }
                return (contents as ICollection<T>).IsReadOnly;
            }
        }

        public T this[int index]
        {
            get { return GetContent(index) as T; }
            set { throw new NotImplementedException("Readonly"); }
        }
        #endregion

        #region Reset Methods
        /// <summary>
        /// Reset the vars
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (contents != null)
            {
                contents.Clear();
            }
        }
        #endregion

        #region IList<T> Methods
        public virtual int IndexOf(T item)
        {
            if (contents != null)
            {
                return -1;
            }
            return contents.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException("Readonly");
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotImplementedException("Readonly");
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException("Readonly");
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException("Readonly");
        }

        public virtual bool Contains(T item)
        {
            if (contents == null)
            {
                return false;
            }
            return contents.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (contents == null)
            {
                return;
            }
            contents.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException("Readonly");
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            if (contents == null)
            {
                yield break;
            }

            foreach (T item in contents)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ViewDictionary.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 00:03:01 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    /// <summary>
    /// 利用Stack的View算法。
    /// 已从ViewManager分离，可自己编写单例Manager，直接使用它。
    /// ViewManager的核心算法，继承或修改它。
    /// </summary>
    public class ViewDictionary : IDictionary<string, ViewBase>, IDisposable
    {
        #region Field
        private GameObject m_GameObject;
        private ViewBase m_CurrentView;
        private string m_PrefabDirectory = "";

        /// <summary>
        /// 已读取的Prefab
        /// </summary>
        private Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();

        /// <summary>
        /// 已读取的View
        /// </summary>
        private Dictionary<string, ViewBase> m_ViewDict = new Dictionary<string, ViewBase>();

        /// <summary>
        /// Stack中打开的View
        /// </summary>
        private Stack<ViewBase> m_OpenViews = new Stack<ViewBase>();
        #endregion

        #region Property
        /// <summary>
        /// Manager所在GameObject
        /// </summary>
        public GameObject gameObject
        {
            get { return m_GameObject; }
            set { m_GameObject = value; }
        }

        /// <summary>
        /// 当前激活的View
        /// </summary>
        public ViewBase currentView
        {
            get { return m_CurrentView; }
            private set { m_CurrentView = value; }
        }

        /// <summary>
        /// Prefab根路径
        /// </summary>
        public string prefabDirectory
        {
            get { return m_PrefabDirectory; }
            set { m_PrefabDirectory = value; }
        }

        /// <summary>
        /// 获取所有Stack打开的View
        /// </summary>
        public ViewBase[] openedStackViews
        {
            get { return m_OpenViews.ToArray(); }
        }

        /// <summary>
        /// 获取所有打开的View
        /// </summary>
        public ViewBase[] openedViews
        {
            get { return m_ViewDict.Values.Where(view => view.gameObject.activeSelf).ToArray(); }
        }
        #endregion

        #region Interface IDictionary<string, ViewBase> Property

        public ICollection<string> Keys
        {
            get { return m_ViewDict.Keys; }
        }

        public ICollection<ViewBase> Values
        {
            get { return m_ViewDict.Values; }
        }

        public int Count
        {
            get { return m_ViewDict.Count; }
        }

        bool ICollection<KeyValuePair<string, ViewBase>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, ViewBase>>)m_ViewDict).IsReadOnly; }
        }

        public ViewBase this[string key]
        {
            get { return m_ViewDict[key]; }
            set { throw new NotImplementedException("[ViewManager] Indexer is Read only."); }
        }
        #endregion

        #region Load Prefab/Component
        /// <summary>
        /// 获取View组件类型。
        /// 默认使用了默认程序集的反射，viewName需要和Type的FullName一致，
        /// FullName是包含命名空间(Namespaces)的。
        /// 如果要自定义或对反射不是很了解，尽量不用使用反射，请继承或修改它。
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        protected virtual Type GetViewComponentType(string viewName)
        {
            return Assembly.GetCallingAssembly().GetType(viewName);
        }

        /// <summary>
        /// 获取Prefab路径。
        /// 默认使用了System.IO.Path，
        /// 所以prefabDirectory不能为null，可以为empty。
        /// 如果需要自定义，请继承或修改它。
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        protected virtual string GetPrefabPath(string viewName)
        {
            return Path.Combine(prefabDirectory, viewName);
        }

        /// <summary>
        /// 读取Prefab。
        /// 默认使用了Resources.Load，
        /// 如果需要自定义，请继承或修改它。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected virtual GameObject LoadPrefab(string path)
        {
            return Resources.Load<GameObject>(path);
        }
        #endregion

        #region Prefab Method
        /// <summary>
        /// 获取或读取Prefab。
        /// 错误：找不到Prefab时。
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private GameObject GetOrLoadPrefab(string viewName)
        {
            GameObject prefab;
            if (!m_PrefabDict.TryGetValue(viewName, out prefab))
            {
                string path = GetPrefabPath(viewName);
                prefab = LoadPrefab(path);
                if (prefab == null)
                {
                    Debug.LogErrorFormat(
                        "[GetOrLoadPrefab] Prefab of view name '{0}' is not found. Path: {1}",
                        viewName,
                        path
                        );
                    return null;
                }
                m_PrefabDict.Add(viewName, prefab);
            }
            return prefab;
        }
        #endregion

        #region Instantiate View Method
        /// <summary>
        /// 获取或实例化view。
        /// 实例化时：
        /// 1、预制体带组件，直接返回。
        /// 2、预制体不带组件，动态添加组件。
        /// 错误：
        /// 1、找不到组件的类型。
        /// 2、组件类型是Abstract。
        /// 3、组件类型没有继承自ViewBase。
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        protected ViewBase GetOrInstantiateView(string viewName)
        {
            ViewBase view;
            if (!m_ViewDict.TryGetValue(viewName, out view))
            {
                // 获取Prefab
                GameObject prefab = GetOrLoadPrefab(viewName);
                if (prefab == null)
                {
                    Debug.LogError("[InstantiateView] Prefab is not found.");
                    return null;
                }

                // 实例化并检测组件
                GameObject viewGameObject = GameObject.Instantiate(prefab);
                view = viewGameObject.GetComponent<ViewBase>();
                if (view == null)
                {
                    // 没有找到组件，动态添加
                    Type type = GetViewComponentType(viewName);
                    if (type == null || type.IsAbstract || !type.IsSubclassOf(typeof(ViewBase)))
                    {
                        Debug.LogErrorFormat(
                            "[InstantiateView] Get view type error. Type of view name '{0}' is null or is abstract or is not sub class of ViewBase.",
                            viewName
                            );
                        GameObject.Destroy(viewGameObject);
                        return null;
                    }
                    view = viewGameObject.AddComponent(type) as ViewBase;
                }

                view.LoadViewInternal(viewName);
                OnInstantiateView(view);
                m_ViewDict.Add(viewName, view);
            }

            return view;
        }

        /// <summary>
        /// 实例化后时的动作
        /// </summary>
        /// <param name="view"></param>
        protected virtual void OnInstantiateView(ViewBase view)
        {
            view.transform.SetParent(gameObject.transform, false);
        }

        /// <summary>
        /// 预实例化View
        /// </summary>
        /// <param name="viewNames"></param>
        public void PreInstantiateView(params string[] viewNames)
        {
            if (viewNames == null)
            {
                return;
            }

            for (int i = 0; i < viewNames.Length; i++)
            {
                if (string.IsNullOrEmpty(viewNames[i]))
                {
                    Debug.LogWarningFormat(
                        "[PreInstantiateView] View name index '{0}' at argument named 'viewNames' is null or empty.",
                        i.ToString()
                        );
                    continue;
                }

                if (m_ViewDict.ContainsKey(viewNames[i]))
                {
                    Debug.LogWarningFormat(
                        "[PreInstantiateView] View named '{0}' is exist.",
                        viewNames[i]
                        );
                    continue;
                }

                ViewBase view = GetOrInstantiateView(viewNames[i]);
                if (view != null && view.gameObject.activeSelf)
                {
                    view.gameObject.SetActive(false);
                }
            }
        }
        #endregion

        #region Open/Close View Method
        /// <summary>
        /// 打开一个View，不加入Stack
        /// 如果View已经被打开，且已经加入到Stack中，给出警告，直接返回
        /// 如果没有实例化，先实例化再打开。
        /// 如果你用来做UIManager，且是Canvas与Panel混用，
        /// Panel的实例化请不要用OpenViewNotStack，请用OpenView，否则不会被加入到Stack中。
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="closeOthers"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ViewBase OpenViewNotStack(string viewName, bool closeOthers, params object[] args)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Debug.LogError("[OpenViewNotStack] Argument named 'viewName' is null or empty.");
                return null;
            }

            if (closeOthers)
            {
                // 关闭所有的view
                CloseViewsAll(false);
            }

            // 获取或实例化
            ViewBase view = GetOrInstantiateView(viewName);
            if (view == null)
            {
                Debug.LogErrorFormat("[OpenViewNotStack] View named '{0}' is not found.", viewName);
                return null;
            }

            // 如果已经在Stack中打开，就返回，并给出警告
            if (m_OpenViews.Contains(view))
            {
                Debug.LogWarningFormat("[OpenViewNotStack] View named '{0}' is in the stack.", viewName);
                return view;
            }
            else
            {
                view.OpenInternal(args);
            }
            return view;
        }

        /// <summary>
        /// 打开一个View。
        /// 如果View已经被打开，且在Stack的中间层，那么将关闭其上方所有View。
        /// 如果没有实例化，先实例化再打开。
        /// 如果你用来做UIManager，且是Canvas与Panel混用，
        /// Canvas的实例化请不要用OpenView，请用OpenViewNotStack，否则会被加入到Stack中。
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="closeStack"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ViewBase OpenView(string viewName, bool closeStack = false, params object[] args)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Debug.LogError("[OpenView] Argument named 'viewName' is null or empty.");
                return null;
            }

            if (closeStack)
            {
                // 关闭所有Stack中的view
                CloseViewsAll(true);
            }

            // 获取或实例化
            ViewBase view = GetOrInstantiateView(viewName);
            if (view == null)
            {
                Debug.LogErrorFormat("[OpenView] View named '{0}' is not found.", viewName);
                return null;
            }

            // 如果已经打开，关闭其上层view
            if (m_OpenViews.Contains(view))
            {
                if (!string.IsNullOrEmpty(view.nextViewNameInternal))
                {
                    CloseView(view.nextViewNameInternal);
                }
            }
            // 如果没有打开，打开并添加进Stack，如果已经打开且没有加入stack，就加入stack
            else
            {
                view.OpenInternal(args);
                ViewBase previous = m_OpenViews.Count > 0 ? m_OpenViews.Peek() : null;
                m_OpenViews.Push(view);
                OnStackViewOpenOrClose(previous, view, true, args);
                currentView = view;
            }

            return view;
        }

        /// <summary>
        /// 泛型打开View
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="viewName"></param>
        /// <param name="closeStack"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TView OpenView<TView>(string viewName, bool closeStack = true, params object[] args) where TView : ViewBase
        {
            return OpenView(viewName, closeStack, args) as TView;
        }

        /// <summary>
        /// 关闭最上层的View。
        /// </summary>
        public void CloseView()
        {
            if (m_OpenViews.Count > 0)
            {
                ViewBase current = m_OpenViews.Pop();
                current.CloseInternal();
                ViewBase previous = m_OpenViews.Count > 0 ? m_OpenViews.Peek() : null;
                OnStackViewOpenOrClose(previous, current, false);
                currentView = previous;
            }
        }

        /// <summary>
        /// 关闭中间层View和其上层的View或没有加入Stack的View
        /// </summary>
        /// <param name="viewName"></param>
        public void CloseView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Debug.LogError("[CloseView] Argument named 'viewName' is null or empty.");
                return;
            }

            // 没有实例化的View
            ViewBase view;
            if (!m_ViewDict.TryGetValue(viewName, out view))
            {
                return;
            }

            // View没有在Stack
            if (!m_OpenViews.Contains(view))
            {
                view.CloseInternal();
                return;
            }

            while (m_OpenViews.Count > 0)
            {
                ViewBase current = m_OpenViews.Peek();
                CloseView();
                if (current == view)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭所有View。
        /// </summary>
        /// <param name="onlyStack"></param>
        public void CloseViewsAll(bool onlyStack)
        {
            // 关闭Stack中的view
            while (m_OpenViews.Count > 0)
            {
                CloseView();
            }

            if (!onlyStack)
            {
                // 获取所有activeSelf为true的view
                ViewBase[] opened = m_ViewDict.Values.Where(view => view.gameObject.activeSelf).ToArray();
                for (int i = 0; i < opened.Length; i++)
                {
                    if (opened[i] != null)
                    {
                        opened[i].CloseInternal();
                    } 
                }
            }
        }

        /// <summary>
        /// 当打开或关闭View时的动作，
        /// 继承它，请一定调用base.OnViewOpenOrClose(previous, current, open)。
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="current"></param>
        /// <param name="open"></param>
        protected virtual void OnStackViewOpenOrClose(ViewBase previous, ViewBase current, bool open, params object[] openArgs)
        {
            if (open)
            {
                if (previous != null)
                {
                    previous.nextViewNameInternal = current.viewName;
                    current.previousViewNameInternal = previous.viewName;
                }
            }
            else
            {
                if (previous != null)
                {
                    previous.nextViewNameInternal = null;
                }
                current.previousViewNameInternal = null;
            }
        }

        #endregion

        #region Destroy View Method
        /// <summary>
        /// 销毁最上层的打开的View。
        /// </summary>
        public void DestroyOpenView()
        {
            if (m_OpenViews.Count > 0)
            {
                ViewBase current = m_OpenViews.Pop();
                current.CloseInternal();
                ViewBase previous = m_OpenViews.Count > 0 ? m_OpenViews.Peek() : null;
                OnStackViewOpenOrClose(previous, current, false);
                currentView = previous;

                m_ViewDict.Remove(current.viewName);
                GameObject.Destroy(current.gameObject);
            }
        }

        /// <summary>
        /// 销毁一个View.
        /// 如果View没有打开，就直接销毁。
        /// 如果View被打开了，那么：
        /// 1、在最上层，直接关闭并销毁；
        /// 2、在中间层，链接其两边的View，只销毁它自己。
        /// </summary>
        /// <param name="viewName"></param>
        public void DestroyView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Debug.LogError("[DestroyView] Argument named 'viewName' is null or empty.");
                return;
            }

            // 没有实例化的View
            ViewBase view;
            if (!m_ViewDict.TryGetValue(viewName, out view))
            {
                return;
            }

            // View没有在Stack中打开, 直接Destroy。
            // 如果是UI的话，Canvas下可能有Panel，
            // 请注意，这将直接Destroy掉下面的Panel，可能造成错误，
            // 要在View的代码的OnDestroy中修改，具体请查看，Example中例子。
            if (!m_OpenViews.Contains(view))
            {
                m_ViewDict.Remove(view.viewName);
                GameObject.Destroy(view.gameObject);
                return;
            }

            // 如果最上方的是这个View，就直接Destroy
            ViewBase current = m_OpenViews.Peek();
            if (current == view)
            {
                DestroyOpenView();
                return;
            }

            Stack<ViewBase> opens = new Stack<ViewBase>();
            while (m_OpenViews.Count > 0)
            {
                // 取出在其上打开的View
                current = m_OpenViews.Pop();

                // 如果找到了，将其删除，并将前后的View相联结
                if (current == view)
                {
                    // 如果在最下层的View
                    if (m_OpenViews.Count == 0)
                    {                       
                        opens.Peek().previousViewNameInternal = null;
                    }
                    // 如果在中间层
                    else
                    {
                        opens.Peek().previousViewNameInternal = m_OpenViews.Peek().viewName;
                        m_OpenViews.Peek().nextViewNameInternal = opens.Peek().viewName;
                    }
                    view.CloseInternal();
                    m_ViewDict.Remove(view.viewName);
                    GameObject.Destroy(view.gameObject);
                    break;
                }

                opens.Push(current);
            }

            // 将取出的重新加入
            while (opens.Count > 0)
            {
                m_OpenViews.Push(opens.Pop());
            }
        }

        /// <summary>
        /// 销毁所有View。
        /// </summary>
        public void DestroyViewAll()
        {
            ViewBase[] views = m_ViewDict.Values.ToArray();

            m_OpenViews.Clear();
            m_ViewDict.Clear();
            m_CurrentView = null;

            foreach (ViewBase view in views)
            {
                if (view != null)
                {
                    view.CloseInternal();
                    GameObject.Destroy(view.gameObject);
                }
            }
            views = null;
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// View是否打开。
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public bool IsOpen(string viewName)
        {
            ViewBase view;
            if (!m_ViewDict.TryGetValue(viewName, out view))
            {
                return false;
            }
            return view.gameObject.activeSelf;
        }

        /// <summary>
        /// 获取View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName"></param>
        public T GetView<T>(string viewName) where T : ViewBase
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return null;
            }

            ViewBase view;
            if (!m_ViewDict.TryGetValue(viewName, out view))
            {
                return null;
            }
            return view as T;
        }

        /// <summary>
        /// 打印所有实例化的View
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string[] keys = m_ViewDict.Keys.ToArray();
            return string.Format("Views: [{0}]", string.Join(", ", keys));
        }

        /// <summary>
        /// 打印所有打开/关闭的View
        /// </summary>
        /// <returns></returns>
        public string ToString(bool isOpen)
        {
            string[] keys = m_ViewDict.Values.Where(view => view.gameObject.activeSelf == isOpen).Select(view => view.viewName).ToArray();
            return string.Format("{0} Views: [{1}]", isOpen ? "Opened" : "Closed", string.Join(", ", keys));
        }
        #endregion

        #region Interface IDictionary<string, ViewBase> Method
        /// <summary>
        /// 添加外部view。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, ViewBase value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[Add] Argument named 'key' is null or empty.");
                return;
            }

            if (value == null)
            {
                Debug.LogError("[Add] Argument named 'value' is null.");
                return;
            }

            if (m_ViewDict.ContainsKey(key) || m_ViewDict.ContainsValue(value))
            {
                Debug.LogErrorFormat("[Add] Add unmanageed view({0}) error. View is exist.", key);
                return;
            }

            value.LoadViewInternal(key);
            OnInstantiateView(value);
            value.gameObject.SetActive(false);
            m_ViewDict.Add(key, value);
        }

        /// <summary>
        /// View是否已经实例化。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return m_ViewDict.ContainsKey(key);
        }

        bool IDictionary<string, ViewBase>.Remove(string key)
        {
            throw new NotImplementedException("Not supported. View can only be destroyed. Use 'DestroyView' instead.");
        }

        /// <summary>
        /// 尝试获取View
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out ViewBase value)
        {
            return m_ViewDict.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<string, ViewBase>>.Add(KeyValuePair<string, ViewBase> item)
        {
            throw new NotImplementedException("Not supported. Use other 'Add' method instead.");
        }

        void ICollection<KeyValuePair<string, ViewBase>>.Clear()
        {
            throw new NotImplementedException("Not supported. View can only be destroyed. Use 'DestroyViewAll' instead.");
        }

        bool ICollection<KeyValuePair<string, ViewBase>>.Contains(KeyValuePair<string, ViewBase> item)
        {
            return ((ICollection<KeyValuePair<string, ViewBase>>)m_ViewDict).Contains(item);
        }

        void ICollection<KeyValuePair<string, ViewBase>>.CopyTo(KeyValuePair<string, ViewBase>[] array, int arrayIndex)
        {
            throw new NotImplementedException("Not supported.");
        }

        bool ICollection<KeyValuePair<string, ViewBase>>.Remove(KeyValuePair<string, ViewBase> item)
        {
            throw new NotImplementedException("Not supported. View can only be destroyed. Use 'DestroyView' instead.");
        }

        public IEnumerator<KeyValuePair<string, ViewBase>> GetEnumerator()
        {
            return m_ViewDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_ViewDict.GetEnumerator();
        }

        #endregion

        #region Interface IDisposable Method
        /// <summary>
        /// 销毁所有View并释放空间，此Dictionary不可再次使用
        /// </summary>
        public void Dispose()
        {
            DestroyViewAll();

            m_GameObject = null;
            m_CurrentView = null;
            m_PrefabDirectory = null;
            m_PrefabDict = null;
            m_ViewDict = null;
            m_OpenViews = null;

            OnDispose();
        }

        /// <summary>
        /// 用于override的释放空间方法
        /// </summary>
        protected virtual void OnDispose()
        {

        }
        #endregion
    }
}
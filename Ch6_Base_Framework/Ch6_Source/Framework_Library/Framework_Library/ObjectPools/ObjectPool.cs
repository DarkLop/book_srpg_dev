#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ObjectPool.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 19:03:20 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    [AddComponentMenu("SRPG/Object Pool")]
    public sealed class ObjectPool : MonoBehaviour
    {
        #region Class InstanceList
        /// <summary>
        /// Instance集合
        /// </summary>
        [Serializable]
        public class InstanceCollection : IList<GameObject>, IDisposable
        {
            #region Field
            private ObjectPool m_Pool;

            [SerializeField, Tooltip("The prefab.")]
            private GameObject m_Prefab;
            [SerializeField, Tooltip("The number of preload count.")]
            private int m_PreCount = 1;
            [SerializeField, Tooltip("Pre-create instance per frame.")]
            private int m_PreCountPerFrame = 10;
            [SerializeField, Tooltip("Reparent to pool when the instance on created.")]
            private bool m_ReparentOnCreated = true;

            private List<GameObject> m_Instances;

            /// <summary>
            /// 回收的Instances
            /// </summary>
            private List<GameObject> m_RecycledInstances;
            #endregion

            #region Property
            /// <summary>
            /// 所在池子
            /// </summary>
            public ObjectPool ownerPool
            {
                get { return m_Pool; }
                internal set { m_Pool = value; }
            }

            /// <summary>
            /// 预制体
            /// </summary>
            public GameObject prefab
            {
                get { return m_Prefab; }
            }

            /// <summary>
            /// 预制体名字
            /// </summary>
            public string prefabName
            {
                get { return m_Prefab.name; }
            }

            /// <summary>
            /// 预读取数量
            /// </summary>
            public int preCount
            {
                get { return m_PreCount; }
                set { m_PreCount = Mathf.Max(0, value); }
            }

            /// <summary>
            /// 每帧创建预读instance的数量，最小每帧1个
            /// </summary>
            public int preCountPerFrame
            {
                get { return m_PreCountPerFrame; }
                set { m_PreCountPerFrame = Mathf.Max(1, value); }
            }

            /// <summary>
            /// 在Create时，是否将父对象设置成Pool
            /// </summary>
            public bool reparentOnCreated
            {
                get { return m_ReparentOnCreated; }
                set { m_ReparentOnCreated = value; }
            }

            /// <summary>
            /// 未使用的Instance的Count
            /// </summary>
            public int recycledCount
            {
                get { return m_RecycledInstances.Count; }
            }
            #endregion

            #region Constuctor
            /// <summary>
            /// Editor Only.
            /// </summary>
            public InstanceCollection()
            {
                m_PreCount = 0;
                m_PreCountPerFrame = 10;
            }

            public InstanceCollection(GameObject prefab)
            {
                if (prefab == null)
                {
                    throw new ArgumentNullException("InstanceList Constructor: Arugment named 'prefab' is null.");
                }

                m_Prefab = prefab;
                m_PreCount = 0;
                m_PreCountPerFrame = 10;
                m_Instances = new List<GameObject>();
                m_RecycledInstances = new List<GameObject>();
            }
            #endregion

            #region Pool Method
            /// <summary>
            /// 获取未使用Instance，如果没有，则创建一个Instance
            /// </summary>
            /// <returns></returns>
            internal GameObject GetOrCreateRecycledInstance()
            {
                GameObject instance;
                if (recycledCount > 0)
                {
                    instance = m_RecycledInstances[recycledCount - 1];
                    m_RecycledInstances.RemoveAt(recycledCount - 1);
                }
                else
                {
                    instance = GameObject.Instantiate(prefab);
                    RenameInstance(instance);
                    m_Instances.Add(instance);

                    ReusableObject reusable = GetOrAddReusableObject(instance);
                    reusable.isFromPool = true;
                    reusable.onDestroyed = ReusableObject_onDestroyed;
                    reusable.onDespawn = ReusableObject_onDespawn;
                    
                    if (reparentOnCreated)
                    {
                        instance.transform.SetParent(m_Pool.transform, false);
                    }
                }

                return instance;
            }

            private void ReusableObject_onDestroyed(ReusableObject reusable)
            {
                m_RecycledInstances.Remove(reusable.gameObject);
                m_Instances.Remove(reusable.gameObject);
            }

            private void ReusableObject_onDespawn(ReusableObject reusable)
            {
                m_RecycledInstances.Add(reusable.gameObject);

                if (reparentOnCreated && reusable.transform.parent != m_Pool.transform)
                {
                    reusable.transform.SetParent(m_Pool.transform, false);
                } 
            }

            public void DespawnInstancesAll()
            {
                for (int i = 0; i < m_Instances.Count; i++)
                {
                    DespawnUnsafe(m_Instances[i]);
                }
            }

            /// <summary>
            /// 立即Destroy未使用的Instance, 
            /// count: 数量，小于或等于0时Destroy所有未使用Instance
            /// </summary>
            public void DestroyRecycledImmediate(int count = 0)
            {
                if (recycledCount == 0)
                {
                    return;
                }

                int total = count <= 0 ? recycledCount : Mathf.Min(recycledCount, count);

                for (int i = total - 1; i >= 0; i--)
                {
                    if (i < m_RecycledInstances.Count)
                    {
                        GameObject.DestroyImmediate(m_RecycledInstances[i].gameObject);
                    }
                }
            }
            #endregion

            #region Helper Method
            /// <summary>
            /// 重命名Instance，在Hierarchy面板更直观
            /// </summary>
            /// <param name="instance"></param>
            private void RenameInstance(GameObject instance)
            {
                instance.name += m_Instances.Count.ToString("#000");
            }

            /// <summary>
            /// 打印prefab与所有instance的名字
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{ ");
                sb.Append(prefab.name);
                sb.Append(" [");
                for (int i = 0; i < m_Instances.Count; i++)
                {
                    sb.Append(m_Instances[i].name);
                    if (i == m_Instances.Count - 1)
                    {
                        sb.Append("]");
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(" }");
                return sb.ToString();
            }
            #endregion

            #region Interface IList<GameObject>
            public GameObject this[int index]
            {
                get { return m_Instances[index]; }
                set { throw new NotImplementedException("Read only."); }
            }

            /// <summary>
            /// Instance的Count
            /// </summary>
            public int Count
            {
                get { return m_Instances.Count; }
            }

            bool ICollection<GameObject>.IsReadOnly
            {
                get { return ((ICollection<GameObject>)m_Instances).IsReadOnly; }
            }

            /// <summary>
            /// 添加外部Instance。要保证使用的Prefab和这里的是同一个
            /// </summary>
            /// <param name="item"></param>
            public void Add(GameObject item)
            {
                if (item == null)
                {
                    return;
                }

                if (m_Instances.Contains(item))
                {
                    return;
                }

                RenameInstance(item);
                m_Instances.Add(item);

                ReusableObject reusable = GetOrAddReusableObject(item);
                reusable.isFromPool = true;
                reusable.onDestroyed = ReusableObject_onDestroyed;
                reusable.onDespawn = ReusableObject_onDespawn;

                if (reparentOnCreated)
                {
                    item.transform.SetParent(m_Pool.transform, false);
                }

                if (item.gameObject.activeSelf)
                {
                    reusable.Spawn();
                }
                else
                {
                    m_RecycledInstances.Add(item);
                }
            }

            void ICollection<GameObject>.Clear()
            {
                throw new NotImplementedException("Can not be clear.");
            }

            public bool Contains(GameObject item)
            {
                return m_Instances.Contains(item);
            }

            public void CopyTo(GameObject[] array, int arrayIndex)
            {
                m_Instances.CopyTo(array, arrayIndex);
            }

            public IEnumerator<GameObject> GetEnumerator()
            {
                return m_Instances.GetEnumerator();
            }

            public int IndexOf(GameObject item)
            {
                return m_Instances.IndexOf(item);
            }

            /// <summary>
            /// 添加外部Instance。要保证使用的Prefab和这里的是同一个
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public void Insert(int index, GameObject item)
            {
                if (item == null)
                {
                    return;
                }

                if (m_Instances.Contains(item))
                {
                    return;
                }

                RenameInstance(item);
                m_Instances.Insert(index, item);

                ReusableObject reusable = GetOrAddReusableObject(item);
                reusable.isFromPool = true;
                reusable.onDestroyed = ReusableObject_onDestroyed;
                reusable.onDespawn = ReusableObject_onDespawn;

                if (reparentOnCreated)
                {
                    item.transform.SetParent(m_Pool.transform, false);
                }

                if (item.gameObject.activeSelf)
                {
                    reusable.Spawn();
                }
                else
                {
                    m_RecycledInstances.Add(item);
                }
            }

            /// <summary>
            /// 移除出池子，
            /// 注意，如果它的父对象是DontDestroy，你需要手动销毁它或改变它的父对象让场景销毁它。
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Remove(GameObject item)
            {
                if (!m_Instances.Contains(item))
                {
                    return false;
                }

                ReusableObject reusable = item.GetComponent<ReusableObject>();
                reusable.isFromPool = false;
                m_RecycledInstances.Remove(item);
                return m_Instances.Remove(item);
            }

            void IList<GameObject>.RemoveAt(int index)
            {
                throw new NotImplementedException("Can not be remove at, Use 'Remove' instead.");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return m_Instances.GetEnumerator();
            }
            #endregion

            #region Interface IDisposable
            /// <summary>
            /// 释放集合，并销毁所有Instances
            /// </summary>
            public void Dispose()
            {
                if (m_Pool != null)
                {
                    // 从Pool中移除此collection
                    m_Pool.RemoveCollectionInternal(this);
                    m_Pool = null;
                }
                
                m_Prefab = null;
                m_RecycledInstances = null;

                // Destroy所有instances
                for (int i = 0; i < m_Instances.Count; i++)
                {
                    GameObject instance = m_Instances[i];
                    ReusableObject reusable = instance.GetComponent<ReusableObject>();
                    reusable.onDespawn = null;
                    reusable.onDestroyed = null;
                    GameObject.Destroy(instance);
                }

                m_Instances = null;
            }
            #endregion
        }

        #endregion

        #region Field
        [SerializeField, Tooltip("If editor log.")]
        public bool m_EditorLog = false;

        [SerializeField, Tooltip("The name of pool.")]
        private string m_PoolName;
        [SerializeField, Tooltip("Set to dont destroy parent.")]
        private bool m_DontDestroy = true;
        [SerializeField, Tooltip("The pre-prefabs of pool.")]
        private InstanceCollection[] m_PrePrefabs;

        private List<InstanceCollection> m_InstanceCollections = new List<InstanceCollection>();
        private Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();
        #endregion

        #region Property
        /// <summary>
        /// 对象池名字
        /// </summary>
        public string poolName
        {
            get { return m_PoolName; }
            set { m_PoolName = value; }
        }

        /// <summary>
        /// 是否将Pool对象放在DontDestroy物体下
        /// </summary>
        public bool dontDestroy
        {
            get { return m_DontDestroy; }
            set
            {
                if (m_DontDestroy != value)
                {
                    m_DontDestroy = value;
                    if (value && transform.parent != PoolManager.instance.dontDestroyParent)
                    {
                        transform.SetParent(PoolManager.instance.dontDestroyParent.transform);
                    }
                }
            }
        }

        /// <summary>
        /// 预制体数量
        /// </summary>
        public int prefabCount
        {
            get { return m_PrefabDict.Count; }
        }

        /// <summary>
        /// 获取所有prefab
        /// </summary>
        public GameObject[] prefabs
        {
            get { return m_PrefabDict.Values.ToArray(); }
        }

        /// <summary>
        /// 获取预制体的索引器
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        public GameObject this[string prefabName]
        {
            get { return m_PrefabDict[prefabName]; }
        }
        #endregion

        #region Unity Callback
        private void Awake()
        {
            if (string.IsNullOrEmpty(poolName))
            {
                m_PoolName = gameObject.name;
            }

            if (m_PrePrefabs != null && m_PrePrefabs.Length > 0)
            {
                for (int i = 0; i < m_PrePrefabs.Length; i++)
                {
                    InstanceCollection pre = m_PrePrefabs[i];
                    if (pre.prefab == null)
                    {
                        Debug.LogWarningFormat("{0} -> Pre-prefab is null.", poolName);
                        continue;
                    }

                    if (m_PrefabDict.ContainsKey(pre.prefabName))
                    {
                        Debug.LogWarningFormat("{0} -> Pre-prefab is exist.", poolName);
                        continue;
                    }

                    InstanceCollection collection = new InstanceCollection(pre.prefab)
                    {
                        preCount = pre.preCount,
                        preCountPerFrame = pre.preCountPerFrame
                    };
                    AddCollection(collection);
                }
            }

            if (!PoolManager.instance.AddPoolInternal(this))
            {
                Debug.LogError("Destroying...");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            while (m_InstanceCollections.Count > 0)
            {
                m_InstanceCollections[0].Dispose();
            }

            if (!PoolManager.instance.RemovePoolInternal(this))
            {
                Debug.LogErrorFormat("{0} -> UNEXPECTED ERROR! May be the pool name was changed at runtime.", poolName);
            }
        }
        #endregion

        #region Collection Method
        /// <summary>
        /// 加入到Pool中，并预读
        /// </summary>
        /// <param name="collection"></param>
        public void AddCollection(InstanceCollection collection)
        {
            if (collection == null)
            {
                Debug.LogError("{0} -> Argument named 'collection' is null.");
                return;
            }

            if (collection.ownerPool != null)
            {
                Debug.LogErrorFormat("{0} -> Collection has belonged to a pool({1}).", poolName, collection.ownerPool.poolName);
                return;
            }

            if (m_PrefabDict.ContainsKey(collection.prefabName))
            {
                Debug.LogErrorFormat("{0} -> The prefab of collection is exist.", poolName);
                return;
            }

            collection.ownerPool = this;
            m_PrefabDict.Add(collection.prefabName, collection.prefab);
            m_InstanceCollections.Add(collection);
            StartCoroutine(PreCreateInstance(collection));
        }

        /// <summary>
        /// 加入外部Instance，由RuntimePrePoolObject组件调用
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        internal bool AddUnpooledObject(RuntimePrePoolObject runtime)
        {
            if (!m_PrefabDict.ContainsKey(runtime.m_PrefabName))
            {
                Debug.LogErrorFormat(
                    "{0} -> Add unpooled object '{1}' The prefab is not found.", 
                    poolName,
                    runtime.gameObject.name
                    );
                return false;
            }

            InstanceCollection collection = m_InstanceCollections.Find(c => c.prefabName == runtime.m_PrefabName);
            if (collection == null)
            {
                Debug.LogErrorFormat("{0} -> UNEXPECTED ERROR! May be the pool name was changed at runtime.", poolName);
                return false;
            }

            collection.Add(runtime.gameObject);
            return true;
        }

        /// <summary>
        /// 移除集合和prefab
        /// </summary>
        /// <param name="collection"></param>
        internal void RemoveCollectionInternal(InstanceCollection collection)
        {
            m_PrefabDict.Remove(collection.prefabName);
            m_InstanceCollections.Remove(collection);
        }
        #endregion

        #region Get or Create Instance Method
        /// <summary>
        /// 预创建Instance
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private IEnumerator PreCreateInstance(InstanceCollection collection)
        {
            if (collection == null)
            {
                yield break;
            }

            if (m_EditorLog)
            {
                Debug.LogFormat(
                    "{0} -> {1} -> Start pre-load instances. Count('{2}').",
                    poolName,
                    collection.prefabName,
                    collection.preCount
                    );
            }

            yield return null;

            while (collection.Count < collection.preCount)
            {
                int count = Mathf.Min(collection.preCount - collection.Count, collection.preCountPerFrame);
                for (int i = 0; i < count; i++)
                {
                    GameObject go = GameObject.Instantiate(collection.prefab);
                    ReusableObject reusable = GetOrAddReusableObject(go);
                    reusable.Despawn(false);
                    collection.Add(go);
                }
                yield return null;
            }
        }

        /// <summary>
        /// 获取未使用Instance，如果没有，则创建一个Instance
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private GameObject GetOrCreateRecycledInstance(GameObject prefab)
        {
            InstanceCollection collection = m_InstanceCollections.Find(c => c.prefab == prefab);
            if (collection == null)
            {
                //throw new KeyNotFoundException(string.Format("{0}: The collection of prefab '{1}' is not found.", poolName, prefab.name));
                collection = new InstanceCollection(prefab);
                AddCollection(collection);
            }

            return collection.GetOrCreateRecycledInstance();
        }

        #endregion

        #region Spawn Method
        /// <summary>
        /// 生成一个Instance
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Vector3 position)
        {
            if (prefab == null)
            {
                Debug.LogErrorFormat("{0} -> Argument named 'prefab' is null.", poolName);
                return null;
            }
            GameObject instance = GetOrCreateRecycledInstance(prefab);
            instance.transform.position = position;
            ReusableObject reusable = GetOrAddReusableObject(instance);
            instance.gameObject.SetActive(true);
            reusable.Spawn();

            if (m_EditorLog)
            {
                Debug.LogFormat(
                    "{0} -> Spawn a instance named '{1}'.",
                    poolName,
                    instance.name
                    );
            }

            return instance;
        }

        /// <summary>
        /// 生成一个Instance
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab)
        {
            return Spawn(prefab, transform.position);
        }

        /// <summary>
        /// 生成一个Instance
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject Spawn(string prefabName, Vector3 position)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                Debug.LogErrorFormat("{0} -> Argument named 'prefabName' is null or empty.", poolName);
                return null;
            }

            GameObject prefab;
            if (!m_PrefabDict.TryGetValue(prefabName, out prefab))
            {
                Debug.LogErrorFormat("{0} -> Prefab named '{1}' is not found.", poolName, prefabName);
                return null;
            }

            return Spawn(prefab, position);
        }

        /// <summary>
        /// 生成一个Instance
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Spawn(string prefabName)
        {
            return Spawn(prefabName, transform.position);
        }
        #endregion

        #region Despawn Method
        /// <summary>
        /// 回收一个Instance。
        /// true：回收成功。 
        /// 有debug warning：参数instance为null，return false。 
        /// 有debug error：这个GameObject不是由Pool创建的，return false。
        /// false：当没有error和warning时，instance是从其它pool里生成的，或者是没有添加到pool的外部instance。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool Despawn(GameObject instance)
        {
            if (instance == null)
            {
                Debug.LogWarningFormat("{0} -> You tryed despawn a 'null' instance.", poolName);
                return false;
            }

            ReusableObject reusable = instance.GetComponent<ReusableObject>();
            if (reusable == null)
            {
                Debug.LogErrorFormat("{0} -> This GameObject is not a instance from pool.");
                return false;
            }

            // 外部instance，没有加入任何pool
            if (!reusable.isFromPool)
            {
                return false;
            }

            // 找到存在这个instance的collection
            InstanceCollection collection = m_InstanceCollections.Find(c => c.Contains(instance));

            // 没有找到，说明是其它pool里生成的
            if (collection == null)
            {
                return false;
            }

            reusable.Despawn(true);

            if (m_EditorLog)
            {
                Debug.LogFormat(
                    "{0} -> Despawn a instance named '{1}'.",
                    poolName,
                    instance.name
                    );
            }

            return true;
        }

        public void DespawnAll()
        {
            for (int i = 0; i < m_InstanceCollections.Count; i++)
            {
                m_InstanceCollections[i].DespawnInstancesAll();
            }
        }

        /// <summary>
        /// Unsafe回收一个Instance。如果确定它来自pool，使用此方法更有效率。
        /// 如果它不是一个instance或者它不是来自任何Pool：
        /// 如果destroyOnNotFromPool参数为true，就Destroy它；
        /// 如果destroyOnNotFromPool参数为false，
        /// 存在父对象，直到它自己被Destroy，或父对象被Destroy，
        /// 不存在父对象，直到它自己被Destroy，或切换场景被场景Destroy。
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="destroyNotFromPool"></param>
        public static void DespawnUnsafe(GameObject instance, bool destroyNotFromPool = true)
        {
            if (instance == null)
            {
                return;
            }

            ReusableObject reusable = instance.GetComponent<ReusableObject>();
            if (reusable == null)
            {
                if (destroyNotFromPool)
                {
                    GameObject.Destroy(instance);
                }
            }
            else
            {
                if (!reusable.isFromPool && destroyNotFromPool)
                {
                    GameObject.Destroy(instance);
                }
                else
                {
                    reusable.Despawn(true);
                }
            }
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// 立即Destroy未使用的Instance, 
        /// count: 数量，小于或等于0时Destroy所有未使用Instance
        /// </summary>
        public void DestroyRecycledImmediate(GameObject prefab, int count = 0)
        {
            if (prefab == null)
            {
                Debug.LogErrorFormat("{0} -> Argument named 'prefab' is null.", poolName);
                return;
            }

            InstanceCollection collection = m_InstanceCollections.Find(c => c.prefab == prefab);
            if (collection == null)
            {
                Debug.LogWarningFormat("{0} -> 'prefab' is not in this pool.", poolName);
                return;
            }

            if (m_EditorLog)
            {
                Debug.LogFormat(
                    "{0} -> {1} -> Destroy {2} recycled instances.",
                    poolName,
                    prefab.name,
                    count <= 0 ? collection.recycledCount : count
                    );
            }

            collection.DestroyRecycledImmediate(count);
        }

        /// <summary>
        /// 立即Destroy未使用的Instance, 
        /// count: 数量，小于或等于0时Destroy所有未使用Instance
        /// </summary>
        public void DestroyRecycledImmediate(int count = 0)
        {
            for (int i = 0; i < m_InstanceCollections.Count; i++)
            {
                if (m_EditorLog && m_InstanceCollections[i].recycledCount > 0)
                {
                    Debug.LogFormat(
                        "{0} -> {1} -> Destroy {2} recycled instances.",
                        poolName,
                        m_InstanceCollections[i].prefabName,
                        count <= 0 ? m_InstanceCollections[i].recycledCount : count
                        );
                }
                m_InstanceCollections[i].DestroyRecycledImmediate(count);
            }
        }

        /// <summary>
        /// 获取或添加ReusableObject组件
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ReusableObject GetOrAddReusableObject(GameObject instance)
        {
            ReusableObject reusable = instance.GetComponent<ReusableObject>();
            if (reusable == null)
            {
                reusable = instance.AddComponent<ReusableObject>();
            }
            return reusable;
        }

        /// <summary>
        /// 打印所有Prefab与Instances
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            InstanceCollection[] collections = m_InstanceCollections.ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Pool '{0}':", poolName);
            sb.AppendLine();
            for (int i = 0; i < collections.Length; i++)
            {
                sb.Append(i.ToString());
                sb.Append("\t");
                sb.Append(collections[i].ToString());
                sb.AppendLine();
            }

            collections = null;

            return sb.ToString();
        }
        #endregion
    }
}
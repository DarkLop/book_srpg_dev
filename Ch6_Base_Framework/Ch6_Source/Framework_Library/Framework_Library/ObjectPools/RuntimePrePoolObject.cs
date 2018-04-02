#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				RuntimePrePoolObject.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 21:35:23 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    [AddComponentMenu("SRPG/Runtime Pre-Pool-Object")]
    [RequireComponent(typeof(ReusableObject))]
    public sealed class RuntimePrePoolObject : MonoBehaviour
    {
        #region Field
        [SerializeField, Tooltip("The pool to added.")]
        public string m_PoolName;
        [SerializeField, Tooltip("The prefab to added.")]
        public string m_PrefabName;
        [SerializeField, Tooltip("When add pool done, Destroy this component.")]
        public bool m_DestroyThis = true;
        #endregion

        #region Unity Callback
        private void Start()
        {
            if (!PoolManager.instance.Contains(m_PoolName))
            {
                Debug.LogErrorFormat(
                    "Runtime Pre {0} -> The pool named '{1}' is not found.",
                    name,
                    m_PoolName);
                return;
            }

            if (!PoolManager.instance[m_PoolName].AddUnpooledObject(this))
            {
                Debug.LogErrorFormat(
                    "Runtime Pre {0} -> The pool named '{1}' add unpooled object failure.",
                    name,
                    m_PoolName);
                return;
            }

            if (m_DestroyThis)
            {
                RuntimePrePoolObject.Destroy(this);
            }
        }
        #endregion
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ReusableObject.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 18:58:35 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    [AddComponentMenu("SRPG/Reusable Object")]
    public sealed class ReusableObject : MonoBehaviour
    {
        #region Field/Property
        private List<IReusableComponent> m_Components = new List<IReusableComponent>();

        private bool m_IsFromPool;
        public bool isFromPool
        {
            get { return m_IsFromPool; }
            internal set { m_IsFromPool = value; }
        }

        internal Action<ReusableObject> onDespawn;
        internal Action<ReusableObject> onDestroyed;
        #endregion

        #region Unity Callback
        /// <summary>
        /// 初始化，获取所有IReusableComponent
        /// </summary>
        private void Awake()
        {
            MonoBehaviour[] monos = GetComponentsInChildren<MonoBehaviour>();
            if (monos != null && monos.Length > 0)
            {
                foreach (MonoBehaviour mono in monos)
                {
                    if (mono is IReusableComponent)
                    {
                        m_Components.Add(mono as IReusableComponent);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (onDestroyed != null)
            {
                onDestroyed(this);
                onDestroyed = null;
            }

            if (onDespawn != null)
            {
                onDespawn = null;
            }
        }
        #endregion

        #region Method
        internal void Spawn()
        {
            foreach (IReusableComponent component in m_Components)
            {
                component.OnSpawn();
            }
        }

        internal void Despawn(bool triggerComponent)
        {
            gameObject.SetActive(false);

            if (onDespawn != null)
            {
                onDespawn(this);
            }

            if (triggerComponent)
            {
                foreach (IReusableComponent component in m_Components)
                {
                    component.OnDespawn();
                }
            }
        }
        #endregion
    }
}
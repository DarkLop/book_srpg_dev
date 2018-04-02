#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestReusableComponent.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 22:48:09 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TestReusableComponent : MonoBehaviour, IReusableComponent
    {

        /// <summary>
        /// 原始颜色
        /// </summary>
        private Color m_SourceColor;

        /// <summary>
        /// SpriteRenderer的Color
        /// </summary>
        public Color m_Color = Color.white;

        private void Awake()
        {
            m_SourceColor = GetComponent<SpriteRenderer>().color;
        }

        public void OnDespawn()
        {
            GetComponent<SpriteRenderer>().color = m_SourceColor;

            Debug.LogFormat("{0} OnDespawn: Clear color.", name);
        }

        public void OnSpawn()
        {
            GetComponent<SpriteRenderer>().color = m_Color;

            Debug.LogFormat("{0} OnSpawn: Set color to {1}.", name, m_Color);
        }
    }
}
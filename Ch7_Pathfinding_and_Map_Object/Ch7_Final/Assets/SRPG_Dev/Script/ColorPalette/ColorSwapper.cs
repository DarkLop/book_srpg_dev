#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ColorSwapper.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 31 Jan 2018 11:45:22 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ColorPalette
{
    /// <summary>
    /// 颜色转换器
    /// </summary>
    [AddComponentMenu("SRPG/Color Palette/Color Swapper")]
    public class ColorSwapper : MonoBehaviour
    {
        #region Field
        [SerializeField]
        private SpriteRenderer m_Renderer;
        [SerializeField]
        private ColorChart m_SrcChart;
        [SerializeField]
        private List<ColorChart> m_SwapCharts = new List<ColorChart>();
        [SerializeField]
        private string m_StartChart;

        private Texture2D m_MainTexture;
        private MaterialPropertyBlock m_SwapMaterialBlock;
        #endregion

        #region Property
        /// <summary>
        /// Sprite渲染器
        /// </summary>
        public new SpriteRenderer renderer
        {
            get { return m_Renderer; }
            set { m_Renderer = value; }
        }

        /// <summary>
        /// 主贴图
        /// </summary>
        private Texture2D mainTexture
        {
            get
            {
                if (m_MainTexture == null)
                {
                    if (m_Renderer == null || m_Renderer.sprite == null)
                    {
                        return null;
                    }
                    m_MainTexture = m_Renderer.sprite.texture;
                }
                return m_MainTexture;
            }
        }

        /// <summary>
        /// 源颜色组
        /// </summary>
        public ColorChart srcChart
        {
            get { return m_SrcChart; }
            set { m_SrcChart = value; }
        }

        /// <summary>
        /// 映射颜色组
        /// </summary>
        public List<ColorChart> swapCharts
        {
            get { return m_SwapCharts; }
        }

        public string startChart
        {
            get { return m_StartChart; }
            set { m_StartChart = value; }
        }
        #endregion

        #region Unity Callback
        private void Start()
        {
            if (!string.IsNullOrEmpty(m_StartChart))
            {
                SwapColors(m_StartChart);
            }
        }

        private void LateUpdate()
        {
            if (renderer != null && renderer.sprite != null && m_SwapMaterialBlock != null)
            {
                // 如果有SpriteRenderer的动画，必须每帧调用才有效
                renderer.SetPropertyBlock(m_SwapMaterialBlock);
            }
        }

        private void OnDestroy()
        {
            if (m_MainTexture != null)
            {
                m_MainTexture = null;
            }

            if (m_SwapMaterialBlock != null)
            {
                m_SwapMaterialBlock = null;
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 通过name转换颜色
        /// </summary>
        /// <param name="chartName"></param>
        public void SwapColors(string chartName)
        {
            if (string.IsNullOrEmpty(chartName) || mainTexture == null || srcChart == null || swapCharts == null)
            {
                return;
            }

            // 如果是源颜色组的name，清除颜色，否则转换
            if (chartName == srcChart.name)
            {
                ClearSwapColors();
            }
            else
            {
                ColorChart chart = swapCharts.Find(c => c.name == chartName);
                if (chart != null)
                {
                    Texture2D swapTexture;
                    if (chart.TryGetSwapTexture(mainTexture, srcChart, out swapTexture))
                    {
                        SetMaterialPropertyBlockTexture(swapTexture);
                    }
                }
            }
        }

        /// <summary>
        /// 创建转换颜色的 MaterialPropertyBlock
        /// </summary>
        /// <param name="swapTexture"></param>
        private void SetMaterialPropertyBlockTexture(Texture2D swapTexture)
        {
            if (swapTexture == null)
            {
                return;
            }

            if (m_SwapMaterialBlock == null)
            {
                m_SwapMaterialBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(m_SwapMaterialBlock);
            }
            m_SwapMaterialBlock.SetTexture("_MainTex", swapTexture);
        }

        /// <summary>
        /// 清除Swap颜色
        /// </summary>
        public void ClearSwapColors()
        {
            if (m_SwapMaterialBlock != null)
            {
                m_SwapMaterialBlock.SetTexture("_MainTex", mainTexture);
                renderer.SetPropertyBlock(m_SwapMaterialBlock);
                m_SwapMaterialBlock = null;
                m_MainTexture = null;
            }
        }
        #endregion
    }
}
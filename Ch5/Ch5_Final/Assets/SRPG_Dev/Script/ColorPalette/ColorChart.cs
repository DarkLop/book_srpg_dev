#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ColorChart.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 31 Jan 2018 10:53:30 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ColorPalette
{
    /// <summary>
    /// 颜色组(颜色表)
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Color Chart.asset", menuName = "SRPG/Color Chart")]
    public class ColorChart : ScriptableObject, IEnumerable<Color>
    {
        #region Field
        [SerializeField]
        private Color[] m_Colors = new Color[] { };
        #endregion

        #region Property
        /// <summary>
        /// 颜色数量
        /// </summary>
        public int count
        {
            get { return m_Colors.Length; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (m_Colors.Length != value)
                {
                    Array.Resize<Color>(ref m_Colors, value);
                }
            }
        }

        /// <summary>
        /// 颜色索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Color this[int index]
        {
            get { return m_Colors[index]; }
            set { m_Colors[index] = value; }
        }
        #endregion

        #region Swap Method
        /// <summary>
        /// 获取在本表中的对应位置的颜色，如果没有找到，返回原颜色
        /// </summary>
        /// <param name="color"></param>
        /// <param name="srcChart"></param>
        /// <returns></returns>
        public Color GetSwapColor(Color color, ColorChart srcChart)
        {
            if (srcChart == null)
            {
                return color;
            }

            int index = srcChart.IndexOf(color);
            if (index == -1 || index >= m_Colors.Length)
            {
                return color;
            }

            return m_Colors[index];
        }

        /// <summary>
        /// 尝试获取转换后的Texture2D
        /// </summary>
        /// <param name="srcTexture"></param>
        /// <param name="srcChart"></param>
        /// <param name="swappingTexture"></param>
        /// <returns></returns>
        public bool TryGetSwapTexture(Texture2D srcTexture, ColorChart srcChart, out Texture2D swapTexture)
        {
            swapTexture = null;
            if (srcTexture == null || srcChart == null)
            {
                return false;
            }

            // 动态创建没有命名，或动态把名称更改成了空。
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(srcTexture.name))
            {
                Debug.LogError("Name of SwapChart or Name of SrcTexture is null or empty.");
                return false;
            }

            // 尝试获取缓存Texture2D
            if (SwapTextureCache.TryGetTexture2D(name, srcTexture.name, out swapTexture))
            {
                return true;
            }

            // 获取源图所有颜色，并转换颜色
            Color[] colors = srcTexture.GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i].a != 0)
                {
                    colors[i] = GetSwapColor(colors[i], srcChart);
                }                
            }

            // 和源相同的设置创建Texture2D
            Texture2D clone = new Texture2D(srcTexture.width, srcTexture.height)
            {
                alphaIsTransparency = srcTexture.alphaIsTransparency,
                wrapMode = srcTexture.wrapMode,
                filterMode = srcTexture.filterMode,
            };
            // 填充转换后的颜色，并保存
            clone.SetPixels(colors);
            clone.Apply();

            /// 如果加入缓存失败，销毁swapTexture，
            /// 重新尝试获取，
            /// 异步执行有极小的可能发生。
            if (!SwapTextureCache.AddTexture2D(name, srcTexture.name, clone))
            {
                Texture2D.Destroy(clone);
                return TryGetSwapTexture(srcTexture, srcChart, out swapTexture);
            }

            swapTexture = clone;
            return true;
        }

        /// <summary>
        /// 获取转换后的Texture2D
        /// </summary>
        /// <param name="srcTexture"></param>
        /// <param name="srcChart"></param>
        /// <returns></returns>
        public Texture2D GetSwapTexture(Texture2D srcTexture, ColorChart srcChart)
        {
            Texture2D swapTexture;
            if (!TryGetSwapTexture(srcTexture, srcChart, out swapTexture))
            {
                return null;
            }
            return swapTexture;
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="colors"></param>
        public void SetColors(Color[] colors)
        {
            if (colors == null)
            {
                m_Colors = new Color[0];
            }
            else
            {
                m_Colors = new Color[colors.Length];
                Array.Copy(colors, m_Colors, colors.Length);
            }
        }

        /// <summary>
        /// 转换成数组
        /// </summary>
        public Color[] ToArray()
        {
            Color[] colors = new Color[count];
            Array.Copy(m_Colors, colors, count);
            return colors;
        }

        /// <summary>
        /// 获取颜色在表中Index,
        /// 由于Color并不是按255，255，255的整形方式存储，
        /// 所以采用近似值的方式判断是不是同一个颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public int IndexOf(Color color)
        {
            if (m_Colors != null)
            {
                for (int i = 0; i < m_Colors.Length; i++)
                {
                    Color tmp = m_Colors[i];
                    if (Mathf.Approximately(color.r, tmp.r)
                        && Mathf.Approximately(color.g, tmp.g)
                        && Mathf.Approximately(color.b, tmp.b)
                        && Mathf.Approximately(color.a, tmp.a))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 克隆一个颜色表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T Clone<T>(string newName) where T : ColorChart
        {
            if (string.IsNullOrEmpty(newName))
            {
                Debug.LogError("Name can not be null or empty."); 
                return null;
            }

            if (newName == name)
            {
                Debug.LogError("Duplicated Name!");
                return null;
            }

            T clone = ColorChart.CreateInstance<T>();
            clone.name = newName;
            clone.SetColors(m_Colors);
            return clone;
        }

        /// <summary>
        /// 从Texture中读取颜色
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static bool LoadColorsFromTexture(ColorChart chart, Texture2D texture)
        {
            if (chart == null || texture == null)
            {
                return false;
            }

            Color[] colors = texture.GetPixels();
            List<Color> list = new List<Color>();
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i].a != 0 && !list.Contains(colors[i]))
                {
                    list.Add(colors[i]);
                }
            }
            chart.m_Colors = list.ToArray();
            return true;
        }

        /// <summary>
        /// 复制颜色
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcIndex"></param>
        /// <param name="dst"></param>
        /// <param name="dstIndex"></param>
        /// <param name="length"></param>
        public static void CopyColors(ColorChart src, int srcIndex, ColorChart dst, int dstIndex, int length)
        {
            Array.Copy(src.m_Colors, srcIndex, dst.m_Colors, dstIndex, length);
        }
        #endregion

        #region Interface
        public IEnumerator<Color> GetEnumerator()
        {
            foreach (Color color in m_Colors)
            {
                yield return color;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Colors.GetEnumerator();
        }
        #endregion
    }
}
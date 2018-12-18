#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				EditorSrpgDataEditor.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 08:19:27 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
//using UnityEditorInternal;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Framework;

    public class EditorSrpgDataEditorWindow : EditorWindow
    {
        #region Static Window Method
        private static EditorSrpgDataEditorWindow s_Window;
        [MenuItem("Window/SRPG/SRPG Data Editor")]
        public static EditorSrpgDataEditorWindow OpenEditorSrpgDataEditorWindow()
        {
            if (s_Window != null)
            {
                s_Window.Focus();
                return s_Window;
            }
            s_Window = EditorWindow.GetWindow<EditorSrpgDataEditorWindow>(false, "SRPG Data");
            s_Window.minSize = new Vector2(480, 480);
            s_Window.Show();
            return s_Window;
        }
        #endregion

        #region Fields
        private EditorSrpgData m_SrpgData;
        private SerializedObject m_SerializedObject;
        //private ReorderableList m_DataList;

        private Vector2Int m_SelectedRange;
        private Vector2 m_Scroll;

        private GUILayoutOption m_BtnWidth = GUILayout.MaxWidth(120);
        #endregion

        #region Properties
        public EditorSrpgData srpgData
        {
            get { return m_SrpgData; }
            set
            {
                if (m_SrpgData == value)
                {
                    return;
                }
                m_SrpgData = value;

                // 删除以前的
                if (m_SerializedObject != null)
                {
                    m_SerializedObject.Dispose();
                    m_SerializedObject = null;
                }

                // 重新建立
                if (m_SrpgData != null)
                {
                    m_SerializedObject = new SerializedObject(m_SrpgData);
                    //CreateDataList(m_SerializedObject);
                }
            }
        }
        #endregion

        #region Unity Callback
        private void OnDestroy()
        {
            this.srpgData = null;
            s_Window = null;
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            srpgData = (EditorSrpgData)EditorGUILayout.ObjectField("SRPG Data Editor", srpgData, typeof(EditorSrpgData), false);
            EditorGUI.EndDisabledGroup();
            if (srpgData == null || m_SerializedObject == null)
            {
                EditorGUILayout.HelpBox("Please re-open a SRPG Data Editor Window.", MessageType.Info);
                return;
            }

            m_SerializedObject.Update();

            // 绘制选择类型
            SerializedProperty curConfigTypeProperty = m_SerializedObject.FindProperty("currentConfig");
            EditorGUILayout.PropertyField(curConfigTypeProperty, true);
            EditorGUILayout.Space();

            // 绘制按钮
            if (!DoDrawButtons())
            {
                return;
            }

            // 绘制数据
            if (!DoDrawDatas())
            {
                return;
            }
        }
        #endregion

        #region Draw Method
        /// <summary>
        /// 绘制按钮
        /// </summary>
        private bool DoDrawButtons()
        {
            IEditorConfigSerializer config = srpgData.GetCurConfig();
            if (config == null)
            {
                EditorGUILayout.HelpBox(
                    string.Format("{0} Config is not found.", srpgData.currentConfig.ToString()), 
                    MessageType.Error);
                return false;
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save To File", m_BtnWidth))
                {
                    SaveToFile(config);
                }

                if (GUILayout.Button("Load From File", m_BtnWidth))
                {
                    LoadFromFile(config);
                }

                if (GUILayout.Button("Check Keys", m_BtnWidth))
                {
                    CheckDuplicateKeys(config);
                }

                if (GUILayout.Button("Sort Datas", m_BtnWidth))
                {
                    SortWithKeys(config);
                }
            }
            EditorGUILayout.EndHorizontal();

            return true;
        }

        /// <summary>
        /// 绘制具体信息
        /// </summary>
        private bool DoDrawDatas()
        {
            SerializedProperty curConfigProperty = GetConfigProperty();
            if (curConfigProperty == null)
            {
                EditorGUILayout.HelpBox(
                    string.Format("{0} Config Property is not found.", srpgData.currentConfig.ToString()),
                    MessageType.Error);
                return false;
            }

            SerializedProperty curArrayDatasProperty = curConfigProperty.FindPropertyRelative("datas");

            EditorGUI.BeginChangeCheck();

            // 设置数量
            int arraySize = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", curArrayDatasProperty.arraySize));
            curArrayDatasProperty.arraySize = arraySize;

            if (arraySize != 0)
            {
                // 最少显示20个
                m_SelectedRange = EditorGUILayout.Vector2IntField("Index Range", m_SelectedRange);
                m_SelectedRange.x = Mathf.Max(0, Mathf.Min(m_SelectedRange.x, arraySize - 20));
                m_SelectedRange.y = Mathf.Min(arraySize - 1, Mathf.Max(m_SelectedRange.x + 19, m_SelectedRange.y));
                Vector2 range = m_SelectedRange;
                EditorGUILayout.MinMaxSlider(
                    ref range.x,
                    ref range.y,
                    0,
                    arraySize - 1);
                m_SelectedRange.x = (int)range.x;
                m_SelectedRange.y = (int)range.y;

                // 绘制数据
                m_Scroll = EditorGUILayout.BeginScrollView(m_Scroll, "box");
                {
                    for (int i = m_SelectedRange.x; i <= m_SelectedRange.y; i++)
                    {
                        SerializedProperty property = curArrayDatasProperty.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(property, true);
                    }

                    //m_DataList.serializedProperty = curArrayDatasProperty;
                    //m_DataList.DoLayoutList();
                }
                EditorGUILayout.EndScrollView();
            }

            m_SerializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(srpgData);
            }

            return true;
        }
        #endregion

        #region Helper
        /// <summary>
        /// 获取当前config
        /// </summary>
        /// <returns></returns>
        private SerializedProperty GetConfigProperty()
        {
            switch (srpgData.currentConfig)
            {
                case EditorSrpgData.ConfigType.MoveConsumption:
                    return m_SerializedObject.FindProperty("moveConsumptionConfig");
                case EditorSrpgData.ConfigType.Class:
                    return m_SerializedObject.FindProperty("classConfig");
                case EditorSrpgData.ConfigType.Character:
                    return m_SerializedObject.FindProperty("characterInfoConfig");
                case EditorSrpgData.ConfigType.Item:
                    return m_SerializedObject.FindProperty("itemInfoConfig");
                case EditorSrpgData.ConfigType.Text:
                    return m_SerializedObject.FindProperty("textInfoConfig");
                default:
                    return null;
            }
        }

        //private void CreateDataList(SerializedObject serializedObject)
        //{
        //    // 第二个参数是 SerializedProperty elements 数组
        //    // 我们的数组是根据不同配置文件获取的，所以动态设置，初始化为 null
        //    m_DataList = new ReorderableList(serializedObject, null)
        //    {
        //        drawHeaderCallback = DataList_DrawHeaderCallback,
        //        elementHeightCallback = DataList_ElementHeightCallback,
        //        drawElementCallback = DataList_DrawElementCallback
        //    };
        //}

        //private void DataList_DrawHeaderCallback(Rect rect)
        //{
        //    EditorGUI.LabelField(rect, "Datas");
        //}

        //private float DataList_ElementHeightCallback(int index)
        //{
        //    SerializedProperty dataProperty = m_DataList.serializedProperty.GetArrayElementAtIndex(index);
        //    return EditorGUI.GetPropertyHeight(dataProperty);
        //}

        //private void DataList_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        //{
        //    SerializedProperty dataProperty = m_DataList.serializedProperty.GetArrayElementAtIndex(index);
        //    EditorGUI.PropertyField(rect, dataProperty, true);
        //}
        #endregion

        #region Save, Load, Check and Sort
        private void SaveToFile(IEditorConfigSerializer config)
        {
            string ext = (config is XmlConfigFile) ? "xml" : "txt";
            string path = EditorUtility.SaveFilePanel(
                "Save", Application.streamingAssetsPath, config.GetType().Name, ext);

            if (!string.IsNullOrEmpty(path))
            {
                if (!CheckDuplicateKeys(config))
                {
                    Debug.LogError("Config to save has some `Duplicate Keys`. Save Failure.");
                    return;
                }

                try
                {
                    byte[] bytes = config.EditorSerializeToBytes();
                    File.WriteAllBytes(path, bytes);
                    AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    Debug.LogError("Save ERROR: " + e.ToString());
                    return;
                }
            }
        }

        private void LoadFromFile(IEditorConfigSerializer config)
        {
            string ext = (config is XmlConfigFile) ? "xml" : "txt";
            string path = EditorUtility.OpenFilePanel(
                "Load", Application.streamingAssetsPath, ext);

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    config.EditorDeserializeToObject(bytes);
                    EditorUtility.SetDirty(srpgData);
                    Repaint();
                }
                catch (Exception e)
                {
                    Debug.LogError("Load ERROR: " + e.ToString());
                    return;
                }

                if (!CheckDuplicateKeys(config))
                {
                    Debug.LogError("Loaded File has some `Duplicate Keys`.");
                    return;
                }
            }
        }

        /// <summary>
        /// 检查重复的Key
        /// </summary>
        /// <returns></returns>
        private bool CheckDuplicateKeys(IEditorConfigSerializer config)
        {
            // 获取所有key
            Array keys = config.EditorGetKeys();

            // key : index
            Dictionary<object, int> keySet = new Dictionary<object, int>();

            // dumplicate [key : indexes]
            Dictionary<object, HashSet<string>> duplicateKeys = new Dictionary<object, HashSet<string>>();

            for (int i = 0; i < keys.Length; i++)
            {
                object key = keys.GetValue(i);

                // 如果key重复了
                if (keySet.ContainsKey(key))
                {
                    // 如果重复key的set没有建立
                    if (!duplicateKeys.ContainsKey(key))
                    {
                        // 建立set，并加入最初的下标
                        duplicateKeys[key] = new HashSet<string>
                        {
                            keySet[key].ToString()
                        };
                    }

                    // 加入当前下标
                    duplicateKeys[key].Add(i.ToString());
                }
                else
                {
                    keySet.Add(key, i);
                }
            }

            if (duplicateKeys.Count != 0)
            {
                // 打印所有重复的keys
                foreach (var kvp in duplicateKeys)
                {
                    Debug.LogErrorFormat(
                        "Duplicate Keys \"{0}\": Index [{1}]",
                        kvp.Key.ToString(),
                        string.Join(", ", kvp.Value.ToArray()));
                }
                return false;
            }

            return true;
        }

        private void SortWithKeys(IEditorConfigSerializer config)
        {
            config.EditorSortDatas();
        }
        #endregion
    }
}
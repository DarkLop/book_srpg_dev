#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestUIPoolPanel.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 10 Mar 2018 22:28:52 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    public class TestUIPoolPanel : UIBase
    {
        public GameObject[] m_PrefabsToAdd;

        private Dropdown m_PrefabDropdown;
        private Button m_SpawnBtn;
        private Button m_AddPrefabBtn;
        private Button m_DespawnAllBtn;
        private Button m_DestroyRecycledBtn;
        private Button m_UnloadSceneBtn;

        private string m_Prefab;

        protected override IEnumerator OnLoadingView()
        {
            m_PrefabDropdown = transform.Find("Layout/PrefabDropdown").GetComponent<Dropdown>();
            m_SpawnBtn = transform.Find("Layout/SpawnBtn").GetComponent<Button>();
            m_AddPrefabBtn = transform.Find("Layout/AddPrefabBtn").GetComponent<Button>();
            m_DespawnAllBtn = transform.Find("Layout/DespawnAllBtn").GetComponent<Button>();
            m_DestroyRecycledBtn = transform.Find("Layout/DestroyRecycledBtn").GetComponent<Button>();
            m_UnloadSceneBtn = transform.Find("Layout/UnloadSceneBtn").GetComponent<Button>();

            m_PrefabDropdown.onValueChanged.AddListener(PrefabDropdown_onValueChanged);
            m_SpawnBtn.onClick.AddListener(SpawnBtn_onClick);
            m_AddPrefabBtn.onClick.AddListener(AddPrefabBtn_onClick);
            m_DespawnAllBtn.onClick.AddListener(DespawnAllBtn_onClick);
            m_DestroyRecycledBtn.onClick.AddListener(DestroyRecycledBtn_onClick);
            m_UnloadSceneBtn.onClick.AddListener(UnloadSceneBtn_onClick);

            firstSelected = m_SpawnBtn.gameObject;
            yield return null;
            OnFocus();
        }

        #region Control Callback
        private void PrefabDropdown_onValueChanged(int value)
        {
            if (value < 0 || m_PrefabDropdown.options.Count == 0)
            {
                return;
            }

            m_Prefab = m_PrefabDropdown.options[value].text;
        }

        /// <summary>
        /// 生成一个instance
        /// </summary>
        private void SpawnBtn_onClick()
        {
            if (string.IsNullOrEmpty(m_Prefab))
            {
                Debug.Log("Not selected a prefab.");
                return;
            }

            Vector3 position = Vector3.zero;
            position.x = UnityEngine.Random.Range(11f, 21f);
            position.y = UnityEngine.Random.Range(-5f, 5f);
            PoolManager.instance["ShapePool"].Spawn(m_Prefab, position);
        }

        /// <summary>
        /// 往池子中加入prefab
        /// </summary>
        private void AddPrefabBtn_onClick()
        {
            if (m_PrefabsToAdd == null || m_PrefabsToAdd.Length == 0)
            {
                Debug.LogError("Prefab to add is null.");
                return;
            }

            for (int i = 0; i < m_PrefabsToAdd.Length; i++)
            {
                if (m_PrefabsToAdd[i] == null)
                {
                    continue;
                }

                ObjectPool.InstanceCollection collection = new ObjectPool.InstanceCollection(m_PrefabsToAdd[i])
                {
                    preCount = 5,
                    preCountPerFrame = 5
                };
                PoolManager.instance["ShapePool"].AddCollection(collection);
            }
            
            ResetDropdown();

            m_AddPrefabBtn.transform.Find("Text").GetComponent<Text>().text = "Only add once";
            m_AddPrefabBtn.interactable = false;
        }

        /// <summary>
        /// 回收所有Instances
        /// </summary>
        private void DespawnAllBtn_onClick()
        {
            PoolManager.instance["ShapePool"].DespawnAll();
        }

        /// <summary>
        /// 销毁未使用的Instances
        /// </summary>
        private void DestroyRecycledBtn_onClick()
        {
            PoolManager.instance["ShapePool"].DestroyRecycledImmediate(0);
        }

        private void UnloadSceneBtn_onClick()
        {
            TestGameMain.instance.UnloadSceneAsync(TestGameMain.k_TestFrameworkAddSceneName);
        }
        #endregion

        protected override void OnOpen(params object[] args)
        {
            MessageCenter.AddListener(TestGameMain.k_Event_OnSceneUnloaded, Listener_onSceneUnloaded);
            ResetDropdown();
        }

        protected override void OnClose()
        {
            MessageCenter.RemoveListener(TestGameMain.k_Event_OnSceneUnloaded, Listener_onSceneUnloaded);
            m_AddPrefabBtn.transform.Find("Text").GetComponent<Text>().text = "Add Prefab";
            m_AddPrefabBtn.interactable = true;
        }

        private void Listener_onSceneUnloaded(string message, object sender, MessageArgs messageArgs, params object[] messageParams)
        {
            if ((messageArgs as OnSceneUnloadedArgs).scene.name == TestGameMain.k_TestFrameworkAddSceneName)
            {
                TestUIManager.views.CloseView(viewName);
            }
        }

        private void ResetDropdown()
        {
            GameObject[] prefabs = PoolManager.instance["ShapePool"].prefabs;
            List<string> options = prefabs.Select(prefab => prefab.name).ToList();
            m_PrefabDropdown.ClearOptions();
            m_PrefabDropdown.AddOptions(options);
            if (options.Count == 0)
            {
                m_PrefabDropdown.value = -1;
                m_Prefab = null;
            }
            else
            {
                m_PrefabDropdown.value = 0;
                m_Prefab = m_PrefabDropdown.options[0].text;
            }
        }
    }
}
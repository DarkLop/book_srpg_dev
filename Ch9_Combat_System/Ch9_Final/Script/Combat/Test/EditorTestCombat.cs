#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				EditorTestCombat.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 16 Nov 2018 17:47:31 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.CombatManagement.Testing
{
    using DR.Book.SRPG_Dev.Maps;
    using DR.Book.SRPG_Dev.Models;
    using DR.Book.SRPG_Dev.Framework;

    public class EditorTestCombat : MonoBehaviour
    {
        public string m_ConfigDirectory;
        public MapGraph m_Map;
        //public Combat m_Combat;
        public MapClass m_TestClassPrefab;

        public bool m_DebugInfo = true;
        public bool m_DebugStep = true;

        private CombatAnimaController m_CombatAnimaController;

        private MapClass m_TestClass1;
        private MapClass m_TestClass2;

        #region Unity Callback
#if UNITY_EDITOR
        private void Awake()
        {
            if (string.IsNullOrEmpty(m_ConfigDirectory))
            {
                m_ConfigDirectory = Application.streamingAssetsPath + "/Config";
            }

            ConfigLoader.rootDirectory = m_ConfigDirectory;
        }

        private void Start()
        {
            if (m_Map == null)
            {
                m_Map = GameObject.FindObjectOfType<MapGraph>();
            }

            if (m_Map == null)
            {
                Debug.LogError("EditorTestCombat -> Map was not found.");
                return;
            }

            //m_Combat = m_Map.gameObject.GetComponent<Combat>();
            //if (m_Combat == null)
            //{
            //    m_Combat = m_Map.gameObject.AddComponent<Combat>();
            //}

            m_CombatAnimaController = Combat.GetOrAdd(m_Map.gameObject);
            m_CombatAnimaController.onPlay.AddListener(CombatAnimaController_onPlay);
            m_CombatAnimaController.onStop.AddListener(CombatAnimaController_onStop);
            m_CombatAnimaController.onStep.AddListener(CombatAnimaController_onStep);

            m_Map.InitMap();

            if (m_TestClassPrefab == null)
            {
                Debug.LogError("EditorTestCombat -> Class Prefab is null.");
                return;
            }

            m_TestClass1 = m_Map.CreateMapObject(m_TestClassPrefab, new Vector3Int(5, 5, 0)) as MapClass;
            m_TestClass2 = m_Map.CreateMapObject(m_TestClassPrefab, new Vector3Int(6, 5, 0)) as MapClass;
            if (!m_TestClass1.Load(0, RoleType.Unique) || !m_TestClass2.Load(1, RoleType.Unique))
            {
                Debug.LogError("EditorTestCombat -> Load role Error.");
                return;
            }

            ItemModel model = ModelManager.models.Get<ItemModel>();
            m_TestClass1.role.AddItem(model.CreateItem(0));
            m_TestClass2.role.AddItem(model.CreateItem(1));

            //Debug.LogFormat("{0}.hp = {1}, {0}.atk = {4}, {2}.hp = {3}, {2}.atk = {5}",
            //    m_TestClass1.role.character.info.name,
            //    m_TestClass1.role.hp,
            //    m_TestClass2.role.character.info.name,
            //    m_TestClass2.role.hp,
            //    m_TestClass1.role.attack,
            //    m_TestClass2.role.attack);

            //m_Combat.LoadCombatUnit(m_TestClass1, m_TestClass2);
            //m_Combat.BattleBegin();
            //for (int i = 0; i < m_Combat.stepCount; i++)
            //{
            //    CombatVariable var0 = m_Combat.steps[i].atkVal;
            //    CombatVariable var1 = m_Combat.steps[i].defVal;
            //    Debug.LogFormat("({4}) -> Animation Type: ({0}, {1}), ({2}, {3})",
            //        var0.position.ToString(),
            //        var0.animaType.ToString(),
            //        var1.position.ToString(),
            //        var1.animaType.ToString(),
            //        i);
            //}
            //m_Combat.BattleEnd();

            //Debug.LogFormat("{0}.hp = {1}, {2}.hp = {3}",
            //    m_TestClass1.role.character.info.name,
            //    m_TestClass1.role.hp,
            //    m_TestClass2.role.character.info.name,
            //    m_TestClass2.role.hp);

            ReloadCombat();
        }

        private void Update()
        {
            if (!m_Map 
                || !m_CombatAnimaController 
                || !m_TestClass1 
                || !m_TestClass2)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_CombatAnimaController.PlayAnimas(true);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ReloadCombat();
            }
        }

        private void OnDestroy()
        {
            if (m_Map != null && m_CombatAnimaController != null)
            {
                m_CombatAnimaController.onPlay.RemoveListener(CombatAnimaController_onPlay);
                m_CombatAnimaController.onStop.RemoveListener(CombatAnimaController_onStop);
                m_CombatAnimaController.onStep.RemoveListener(CombatAnimaController_onStep);
            }
        }
#endif
        #endregion

        #region Combat
#if UNITY_EDITOR

        private void CombatAnimaController_onPlay(CombatAnimaController combatAnima, bool inMap)
        {
            if (m_DebugInfo)
            {
                Debug.LogFormat("Begin Battle Animations: {0} animations", combatAnima.combat.stepCount);
            }

        }

        private void CombatAnimaController_onStop(CombatAnimaController combatAnima, bool inMap)
        {
            combatAnima.combat.BattleEnd();

            if (m_DebugInfo)
            {
                Debug.Log("End Battle Animations");
            }
        }

        private void CombatAnimaController_onStep(CombatAnimaController combatAnima, int index, float wait, bool end)
        {
            if (!m_DebugInfo || !m_DebugStep)
            {
                return;
            }

            CombatStep step = combatAnima.combat.steps[index];
            CombatVariable var0 = step.GetCombatVariable(0);
            CombatVariable var1 = step.GetCombatVariable(1);

            Debug.LogFormat("({4}, {5}) -> Animation Type: ({0}, {1}), ({2}, {3})",
                var0.position.ToString(),
                var0.animaType.ToString(),
                var1.position.ToString(),
                var1.animaType.ToString(),
                index,
                end ? "End" : "Begin");
        }

        private void ReloadCombat()
        {
            if (m_DebugInfo)
            {
                Debug.Log("--------------------");
                Debug.Log("Reload Combat.");
            }

            if (!m_CombatAnimaController.LoadCombatUnit(m_TestClass1, m_TestClass2))
            {
                Debug.LogError("Reload Combat Error: Check the code.");
            }
        }

#endif
        #endregion
    }
}
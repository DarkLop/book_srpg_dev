#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Combat.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Nov 2018 02:16:02 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    using DR.Book.SRPG_Dev.Maps;

    [DisallowMultipleComponent]
    [AddComponentMenu("SRPG/Combat System/Combat")]
    public class Combat : MonoBehaviour
    {
        #region Static
        public static CombatAnimaController GetOrAdd(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Debug.Log("Combat -> argument named `gameObject` is null.");
                return null;
            }

            CombatAnimaController combat = gameObject.GetComponent<CombatAnimaController>();
            if (combat == null)
            {
                combat = gameObject.AddComponent<CombatAnimaController>();
                if (combat.combat.m_BattleActionDict.Count == 0)
                {
                    combat.combat.m_BattleActions = new BattleAction[]
                    {
                        ScriptableObject.CreateInstance<PrepareAction>(),
                        ScriptableObject.CreateInstance<AttackAction>()
                    };
                    combat.combat.InitBattleActions();
                }
            }

            return combat;
        }

        public static CombatAnimaController GetOrAdd(GameObject gameObject, MapClass role0, MapClass role1)
        {
            CombatAnimaController combat = GetOrAdd(gameObject);
            if (combat == null || !combat.LoadCombatUnit(role0, role1))
            {
                return null;
            }
            return combat;
        }
        #endregion

        #region Field
        public BattleAction[] m_BattleActions;
        private Dictionary<BattleActionType, BattleAction> m_BattleActionDict = new Dictionary<BattleActionType, BattleAction>();
        #endregion

        #region Property

        public CombatUnit unit0 { get; protected set; } // 如果是群攻，所有unit用List或数组
        public CombatUnit unit1 { get; protected set; } // 如果是群攻，所有unit用List或数组
        public List<CombatStep> steps { get; protected set; }
        
        public bool isLoaded
        {
            get { return unit0.mapClass != null && unit1.mapClass != null; }
        }

        public int stepCount
        {
            get { return steps.Count; }
        }

        #endregion

        #region Unity Callback
        private void Awake()
        {
            InitBattleActions();

            unit0 = new CombatUnit(0);
            unit1 = new CombatUnit(1);
            steps = new List<CombatStep>();
        }

        private void InitBattleActions()
        {
            if (m_BattleActions != null && m_BattleActions.Length > 0)
            {
                for (int i = 0; i < m_BattleActions.Length; i++)
                {
                    if (m_BattleActions[i] == null)
                    {
                        continue;
                    }

                    BattleAction action = m_BattleActions[i];
                    if (m_BattleActionDict.ContainsKey(action.actionType))
                    {
                        Debug.LogWarningFormat(
                            "Battle Action {0} is exist. OVERRIDE.",
                            action.actionType.ToString());
                    }
                    m_BattleActionDict[action.actionType] = action;
                }
            }
        }

        private void OnDestroy()
        {
            unit0.Dispose();
            unit0 = null;
            unit1.Dispose();
            unit1 = null;
            steps = null;
        }
        #endregion

        #region Load
        public bool LoadCombatUnit(MapClass mapClass0, MapClass mapClass1)
        {
            return unit0.Load(mapClass0) && unit1.Load(mapClass1);
        }
        #endregion

        #region Get Combat Unit
        public CombatUnit GetCombatUnit(int position)
        {
            switch (position)
            {
                case 0:
                    return unit0;
                case 1:
                    return unit1;
                default:
                    Debug.LogError("Combat -> GetCombatUnit: index is out of range.");
                    return null;
            }
        }
        #endregion

        #region Battle
        /// <summary>
        /// 开始战斗
        /// </summary>
        public void BattleBegin()
        {
            if (!isLoaded)
            {
                Debug.LogError("Combat -> StartBattle: please load combat unit first.");
                return;
            }

            if (stepCount > 0)
            {
                Debug.LogError("Combat -> StartBattle: battle is not end.");
                return;
            }

            BattleAction action;
            if (!m_BattleActionDict.TryGetValue(BattleActionType.Prepare, out action))
            {
                Debug.LogError("Combat -> StartBattle: BattleActionType.Prepare is not found, check the code.");
                return;
            }

            // 准备阶段
            CombatStep firstStep = action.CalcBattle(this, default(CombatVariable), default(CombatVariable));
            // firstStep.message = action.message;
            steps.Add(firstStep);

            if (!action.IsBattleEnd(this, firstStep.atkVal, firstStep.defVal))
            {
                CalcBattle(firstStep.atkVal, firstStep.defVal);
            }
        }

        /// <summary>
        /// 计算战斗数据
        /// </summary>
        private void CalcBattle(CombatVariable atkVal, CombatVariable defVal)
        {
            CombatUnit atker = GetCombatUnit(atkVal.position);
            BattleActionType actionType = GetBattleActionType(atker.weaponType);
            BattleAction action;
            if (!m_BattleActionDict.TryGetValue(actionType, out action))
            {
                Debug.LogErrorFormat(
                    "Combat -> StartBattle: BattleActionType.{0} is not found, check the code.",
                    actionType.ToString());
                return;
            }


            CombatStep step = action.CalcBattle(this, atkVal, defVal);
            // step.message = action.message;
            steps.Add(step);

            // 如果战斗没有结束，交换攻击者与防守者
            if (!action.IsBattleEnd(this, step.atkVal, step.defVal))
            {
                if (step.defVal.canAtk)
                {
                    CalcBattle(step.defVal, step.atkVal);
                }
                else
                {
                    // 如果防守方不可反击
                    defVal = step.defVal;
                    defVal.action = true;
                    if (!action.IsBattleEnd(this, defVal, step.atkVal))
                    {
                        CalcBattle(step.atkVal, defVal);
                    }
                }
            }
            else
            {
                // TODO 如果死亡，播放死亡动画（我把死亡动画忘记了）
                // if (step.defVal.isDead) 播放死亡动画
            }
        }

        /// <summary>
        /// 战斗是否结束
        /// </summary>
        /// <param name="atkVal"></param>
        /// <param name="defVal"></param>
        /// <returns></returns>
        private bool IsBattleEnd(ref CombatVariable atkVal, ref CombatVariable defVal)
        {
            // 攻击者死亡，防守者死亡
            if (atkVal.isDead || defVal.isDead)
            {
                return true;
            }

            // 如果防守者可反击
            if (defVal.canAtk)
            {
                // 交换攻击者与防御者
                SwapCombatVariable(ref atkVal, ref defVal);
            }

            // 如果攻击者行动过了
            if (atkVal.action)
            {
                //CombatUnit atker = GetCombatUnit(atkVal.position);
                //CombatUnit defer = GetCombatUnit(defVal.position);

                // TODO 是否继续攻击，必要时需要在 CombatVariable 加入其它控制变量
                // atker.role.skill 包含继续战斗的技能
                // defer.role.skill 包含继续战斗的技能
                //if ( condition )
                //{
                //    // atkVal.action = false;
                //}
            }

            // 攻击者行动过了
            return atkVal.action;
        }

        /// <summary>
        /// 交换攻击者与防御者
        /// </summary>
        /// <param name="atkVal"></param>
        /// <param name="defVal"></param>
        public void SwapCombatVariable(ref CombatVariable atkVal, ref CombatVariable defVal)
        {
            CombatVariable tmp = atkVal;
            atkVal = defVal;
            defVal = tmp;
        }

        /// <summary>
        /// 战斗结束
        /// </summary>
        public void BattleEnd()
        {
            if (stepCount > 0)
            {
                CombatStep result = steps[stepCount - 1];

                CombatVariable unit0Result = result.GetCombatVariable(0);
                CombatVariable unit1Result = result.GetCombatVariable(1);

                // TODO 经验值战利品
                unit0.mapClass.OnBattleEnd(unit0Result.hp, unit0Result.mp, unit0.durability);
                unit1.mapClass.OnBattleEnd(unit1Result.hp, unit1Result.mp, unit1.durability);

                steps.Clear();
            }

            unit0.ClearMapClass();
            unit1.ClearMapClass();
        }

        /// <summary>
        /// 获取行动方式（如何计算战斗数据）
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        private BattleActionType GetBattleActionType(WeaponType weaponType)
        {
            // TODO 由于没有动画支持，所以并没有其他武器
            // 你可以添加其他武器到这里

            switch (weaponType)
            {
                case WeaponType.Sword:
                //case WeaponType.Lance:
                //case WeaponType.Axe:
                //case WeaponType.Bow:
                return BattleActionType.Attack;
                //case WeaponType.Staff:
                    //if ( 如果法杖是治疗 )
                    //{
                    //    return BattleActionType.Heal;
                    //}
                    //else if ( 法杖是其它等 )
                    //{
                    //    return BattleActionType.自定义类型;
                    //}
                //case WeaponType.Fire:
                //case WeaponType.Thunder:
                //case WeaponType.Wind:
                //case WeaponType.Holy:
                //case WeaponType.Dark:
                //    return BattleActionType.MageAttack;
                default:
                    return BattleActionType.Unknow;
            }
        }
        #endregion
    }
}
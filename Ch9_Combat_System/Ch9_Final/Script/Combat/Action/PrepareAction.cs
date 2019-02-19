#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				PrepareAction.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 20 Dec 2018 01:52:30 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    using DR.Book.SRPG_Dev.Models;

    [CreateAssetMenu(fileName = "CombatPrepareAction.asset", menuName = "SRPG/Combat Prepare Action")]
    public class PrepareAction : BattleAction
    {
        public override BattleActionType actionType
        {
            get { return BattleActionType.Prepare; }
        }

        public override CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            CombatUnit atker = combat.GetCombatUnit(0);
            CombatUnit defer = combat.GetCombatUnit(1);

            // 防守者是否可反击
            bool canDeferAtk = false;
            if (defer.role.equipedWeapon != null)
            {
                Vector3Int offset = defer.mapClass.cellPosition - atker.mapClass.cellPosition;
                int dist = Mathf.Abs(offset.x) + Mathf.Abs(offset.y);
                WeaponUniqueInfo defInfo = defer.role.equipedWeapon.uniqueInfo;

                //if (defInfo.weaponType != WeaponType.Staff)
                {
                    // 如果在反击范围内
                    if (dist >= defInfo.minRange && dist <= defInfo.maxRange)
                    {
                        canDeferAtk = true;
                    }
                }
            }
            
            // 根据速度初始化攻击者与防守者
            if (canDeferAtk)
            {
                if (atker.speed < defer.speed)
                {
                    CombatUnit tmp = atker;
                    atker = defer;
                    defer = tmp;
                }
            }

            // 更新信息
            this.message = "战斗开始";

            atkVal = new CombatVariable(atker.position, atker.hp, atker.mp, true, atker.durability, CombatAnimaType.Prepare);
            defVal = new CombatVariable(defer.position, defer.hp, defer.mp, canDeferAtk, defer.durability, CombatAnimaType.Prepare);
            
            // 准备阶段
            CombatStep firstStep = new CombatStep(atkVal, defVal);
            return firstStep;
        }

        public override bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            return false;
        }
    }
}
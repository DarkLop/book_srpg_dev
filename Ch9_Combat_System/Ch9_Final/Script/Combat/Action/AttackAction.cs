#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				AttackAction.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 20 Dec 2018 01:47:08 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    [CreateAssetMenu(fileName = "CombatAttackAction.asset", menuName = "SRPG/Combat Attack Action")]
    public class AttackAction : BattleAction
    {
        public sealed override BattleActionType actionType
        {
            get { return BattleActionType.Attack; }
        }

        public override CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            CombatUnit atker = combat.GetCombatUnit(atkVal.position);
            CombatUnit defer = combat.GetCombatUnit(defVal.position);

            atkVal.animaType = CombatAnimaType.Attack;

            // 真实命中率 = 攻击者命中 - 防守者回避
            int realHit = atker.hit - defer.avoidance;

            // 概率是否击中
            int hitRate = UnityEngine.Random.Range(0, 100);
            bool isHit = hitRate <= realHit;
            if (isHit)
            {
                bool crit = false; // TODO 是否爆击
                int realAtk = atker.atk;

                ///////////////////
                // TODO 触发伤害技能
                // 这里写触发技能后伤害变化（比如武器特效等），
                // 或者触发某些状态（比如中毒等）
                //////////////////

                if (crit)
                {
                    realAtk *= 2; // 假定爆击造成双倍伤害
                }

                // 掉血 = 攻击者攻击力 - 防守者防御力
                // 最少掉一滴血
                int damageHp = Mathf.Max(1, realAtk - defer.def);
                if (damageHp > defVal.hp)
                {
                    damageHp = defVal.hp;
                }
                defVal.hp = defVal.hp - damageHp; 

                atkVal.crit = crit;
                defVal.animaType = CombatAnimaType.Damage;

                // 更新此次攻击信息
                this.message = string.Format(
                    "{0} 对 {1} 的攻击造成了 {2} 点伤害{3}。",
                    atker.role.character.info.name,
                    defer.role.character.info.name,
                    damageHp,
                    crit ? "(爆击)" : string.Empty);

                if (defVal.isDead)
                {
                    this.message += string.Format(" {0}被击败了。", defer.role.character.info.name);
                }
            }
            else
            {
                defVal.animaType = CombatAnimaType.Evade;

                // 更新此次躲闪信息
                this.message = string.Format(
                    "{1} 躲闪了 {0} 的攻击。",
                    atker.role.character.info.name,
                    defer.role.character.info.name);
            }

            // 只有玩家才会减低耐久度
            if (atker.role.attitudeTowards == AttitudeTowards.Player)
            {
                // 攻击者武器耐久度-1
                atkVal.durability = Mathf.Max(0, atkVal.durability - 1);
            }

            // 攻击者行动过了
            atkVal.action = true;

            CombatStep step = new CombatStep(atkVal, defVal);
            return step;
        }

        public override bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal)
        {
            // 防守者死亡
            if (defVal.isDead)
            {
                return true;
            }

            // 如果防守者行动过了
            if (defVal.action)
            {
                //CombatUnit atker = GetCombatUnit(atkVal.position);
                //CombatUnit defer = GetCombatUnit(defVal.position);

                // TODO 是否继续攻击，必要时需要在 CombatVariable 加入其它控制变量
                // 比如，触发过技能或物品了
                // atker.role.skill/item 包含继续战斗的技能或物品
                // defer.role.skill/item 包含继续战斗的技能或物品
                //if ( 触发继续战斗 )
                //{
                //    // return false;
                //}

                return true;
            }

            return false;
        }
    }
}
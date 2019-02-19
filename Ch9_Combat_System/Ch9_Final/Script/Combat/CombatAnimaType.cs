#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CombatAnimaType.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 12 Nov 2018 22:31:39 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.CombatManagement
{
    public enum CombatAnimaType
    {
        Unknow,
        Prepare, // 准备
        Attack, // 攻击
        Heal, //治疗
        Evade, // 躲闪
        Damage, // 受到攻击
        Dead, // 死亡
    }
}
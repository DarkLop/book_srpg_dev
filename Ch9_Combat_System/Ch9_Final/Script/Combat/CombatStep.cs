#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CombatStep.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Nov 2018 17:04:00 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    /// <summary>
    /// 战斗每一步结果
    /// </summary>
    public class CombatStep
    {
        // 如果你要在Inspector面板显示每次战斗结果，在这里更改。
        // 将属性改成字段，加上[SerializeField]，
        // 同时更改Combat类List<CombatStep>也需要这样改
        // 在此类和CombatVariable上加上[Serializable]
        // 也可以自定义UnityEditor
        // 例如：
        // [SerializeField]
        // private CombatVariable m_AtkVal;
        // public CombatVariable atkVal
        // {
        //     get { return m_AtkVal; }
        //     private set { m_AtkVal = value; }
        // }

        /// <summary>
        /// 当前进攻方
        /// </summary>
        public CombatVariable atkVal { get; private set; }

        /// <summary>
        /// 当前防守方
        /// </summary>
        public CombatVariable defVal { get; private set; }

        public CombatStep(CombatVariable atker, CombatVariable defer)
        {
            this.atkVal = atker;
            this.defVal = defer;
        }

        /// <summary>
        /// 根据位置获取战斗变量
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CombatVariable GetCombatVariable(int position)
        {
            if (atkVal.position == position)
            {
                return atkVal;
            }

            if (defVal.position == position)
            {
                return defVal;
            }

            Debug.LogError("CombatStep -> position is out of range.");
            return default(CombatVariable);
        }
    }
}
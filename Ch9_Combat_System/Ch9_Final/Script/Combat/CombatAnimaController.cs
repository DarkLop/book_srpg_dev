#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CombatManager.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Nov 2018 01:13:08 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    //using DR.Book.SRPG_Dev.Framework;
    using DR.Book.SRPG_Dev.Maps;

    //public class CombatAnimaPlayArgs : MessageArgs
    //{
    //    public Combat combat;
    //    public bool inMap;
    //    public bool end;

    //    protected override void OnDispose()
    //    {
    //        combat = null;
    //        inMap = false;
    //        end = false;
    //    }
    //}

    //public class CombatAnimaStepArgs : MessageArgs
    //{
    //    public int index;
    //    public float wait;
    //    public bool end;

    //    protected override void OnDispose()
    //    {
    //        index = 0;
    //        wait = 0f;
    //        end = false;
    //    }
    //}

    [DisallowMultipleComponent, RequireComponent(typeof(Combat))]
    [AddComponentMenu("SRPG/Combat System/Combat Anima Controller")]
    public class CombatAnimaController : MonoBehaviour
    {
        //#region Enum AnimaStatus
        //public enum AnimaStatus
        //{
        //    Ready,
        //    Running,
        //    Waiting,
        //}
        //#endregion

        #region Fields
        //private readonly CombatAnimaPlayArgs m_AnimaPlayArgs = new CombatAnimaPlayArgs();
        //private readonly CombatAnimaStepArgs m_AnimaStepArgs = new CombatAnimaStepArgs();

        //private AnimaStatus m_Status = AnimaStatus.Ready;

        [SerializeField]
        private float m_AnimationInterval = 1f;

        private Combat m_Combat;
        private Coroutine m_AnimaCoroutine;
        #endregion

        #region Events
        /// <summary>
        /// 当动画播放开始/结束时。
        /// Args: 
        ///     CombatAnimaController combatAnima, 
        ///     bool inMap, // 是否是地图动画
        /// </summary>
        [Serializable]
        public class OnAnimaPlayEvent : UnityEvent<CombatAnimaController, bool> { }

        /// <summary>
        /// 当每一次行动开始/结束时。
        /// Args: 
        ///     CombatAnimaController combatAnima, 
        ///     int index, // step的下标
        ///     float wait, // 每一次行动的动画播放时间
        ///     bool end // step的播放开始还是结束
        /// </summary>
        [Serializable]
        public class OnAnimaStepEvent : UnityEvent<CombatAnimaController, int, float, bool> { }

        [Space]
        [SerializeField]
        private OnAnimaPlayEvent m_OnPlayEvent = new OnAnimaPlayEvent();

        /// <summary>
        /// 当动画播放开始时。
        /// Args: 
        ///     CombatAnimaController combatAnima, 
        ///     bool inMap, // 是否是地图动画
        /// </summary>
        public OnAnimaPlayEvent onPlay
        {
            get
            {
                if (m_OnPlayEvent == null)
                {
                    m_OnPlayEvent = new OnAnimaPlayEvent();
                }
                return m_OnPlayEvent;
            }
            set { m_OnPlayEvent = value; }
        }

        [SerializeField]
        private OnAnimaPlayEvent m_OnStopEvent = new OnAnimaPlayEvent();

        /// <summary>
        /// 当动画播放结束时。
        /// Args: 
        ///     CombatAnimaController combatAnima, 
        ///     bool inMap, // 是否是地图动画
        /// </summary>
        public OnAnimaPlayEvent onStop
        {
            get
            {
                if (m_OnStopEvent == null)
                {
                    m_OnStopEvent = new OnAnimaPlayEvent();
                }
                return m_OnStopEvent;
            }
            set { m_OnStopEvent = value; }
        }

        [SerializeField]
        private OnAnimaStepEvent m_OnStepEvent = new OnAnimaStepEvent();

        /// <summary>
        /// 当每一次行动开始/结束时。
        /// Args: 
        ///     CombatAnimaController combatAnima, 
        ///     int index, // step的下标
        ///     float wait, // 每一次行动的动画播放时间
        ///     bool end // step的播放开始还是结束
        /// </summary>
        public OnAnimaStepEvent onStep
        {
            get
            {
                if (m_OnStepEvent == null)
                {
                    m_OnStepEvent = new OnAnimaStepEvent();
                }
                return m_OnStepEvent;
            }
            set { m_OnStepEvent = value; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 每个动画的间隔时间
        /// </summary>
        public float animationInterval
        {
            get { return m_AnimationInterval; }
            set { m_AnimationInterval = value; }
        }

        public bool isCombatLoaded
        {
            get { return combat.isLoaded; }
        }

        public bool isBattleCalced
        {
            get { return combat.stepCount > 0; }
        }

        public int stepCount
        {
            get { return combat.stepCount; }
        }

        public bool isAnimaRunning
        {
            get { return m_AnimaCoroutine != null; }
        }

        public Combat combat
        {
            get
            {
                if (m_Combat == null)
                {
                    m_Combat = GetComponent<Combat>();
                }
                return m_Combat;
            }
        }
        #endregion

        #region Unity Callback
        protected virtual void OnDestroy()
        {
            if (m_AnimaCoroutine != null)
            {
                StopCoroutine(m_AnimaCoroutine);
                m_AnimaCoroutine = null;
            }

            if (m_OnPlayEvent != null)
            {
                m_OnPlayEvent.RemoveAllListeners();
                m_OnPlayEvent = null;
            }

            if (m_OnStepEvent != null)
            {
                m_OnStepEvent.RemoveAllListeners();
                m_OnStepEvent = null;
            }

            //m_AnimaPlayArgs.Dispose();
            //m_AnimaStepArgs.Dispose();
        }
        #endregion


        public void ClearEvents()
        {
            onPlay = null;
            onStep = null;
        }

        /// <summary>
        /// 初始化战斗双方
        /// </summary>
        /// <param name="mapClass0"></param>
        /// <param name="mapClass1"></param>
        /// <returns></returns>
        public bool LoadCombatUnit(MapClass mapClass0, MapClass mapClass1)
        {
            return combat.LoadCombatUnit(mapClass0, mapClass1);
        }

        /// <summary>
        /// 运行动画
        /// </summary>
        /// <param name="combat"></param>
        /// <param name="inMap"></param>
        public void PlayAnimas(bool inMap)
        {
            if (!isCombatLoaded || isAnimaRunning)
            {
                Debug.LogError("CombatAnimaController -> combat is not loaded, or the animation is running.");
                return;
            }

            // 如果没有计算，则先计算
            if (!isBattleCalced)
            {
                combat.BattleBegin();
                if (!isBattleCalced)
                {
                    Debug.LogError("CombatAnimaController -> calculate error! check the `Combat` code.");
                    return;
                }
            }

            // 播放动画
            m_AnimaCoroutine = StartCoroutine(RunningAnimas(inMap));
        }

        ///// <summary>
        ///// 继续动画
        ///// </summary>
        //public void AnimaConitnue()
        //{
        //    if (m_Status == AnimaStatus.Waiting)
        //    {
        //        m_Status = AnimaStatus.Running;
        //    }
        //}

        /// <summary>
        /// 开始运行动画
        /// </summary>
        /// <param name="combat"></param>
        /// <param name="inMap"></param>
        /// <returns></returns>
        private IEnumerator RunningAnimas(bool inMap)
        {
            // 状态设置为运行中
            //m_Status = AnimaStatus.Running;

            onPlay.Invoke(this, inMap);

            if (inMap)
            {
                // 在地图中
                yield return RunningAnimasInMap();
            }
            else
            {
                // TODO 单独场景
            }

            onStop.Invoke(this, inMap);

            m_AnimaCoroutine = null;

            //m_Status = AnimaStatus.Ready;
        }

        private IEnumerator RunningAnimasInMap()
        {
            CombatUnit unit0 = combat.GetCombatUnit(0);
            CombatUnit unit1 = combat.GetCombatUnit(1);
            List<CombatStep> steps = combat.steps;

            Direction[] dirs = new Direction[2];
            dirs[0] = GetAnimaDirectionInMap(unit0.mapClass.cellPosition, unit1.mapClass.cellPosition);
            dirs[1] = GetAnimaDirectionInMap(unit1.mapClass.cellPosition, unit0.mapClass.cellPosition);

            yield return null;

            int curIndex = 0;
            CombatStep step;
            while (curIndex < steps.Count)
            {
                step = steps[curIndex];

                // 根据动画不同，播放时间应该是不同的
                // 这需要一些参数或者算法来控制
                // （例如一些魔法，在配置表中加上一个特效的变量，
                // 人物施法动画是这个，特效还要另算，需要计算在内）
                // 这里我只是简单的定义为同时播放

                float len0 = RunAniamAndGetLengthInMap(step.atkVal, step.defVal, dirs);
                float len1 = RunAniamAndGetLengthInMap(step.defVal, step.atkVal, dirs);
                float wait = Mathf.Max(len0, len1);

                onStep.Invoke(this, curIndex, wait, false);
                yield return new WaitForSeconds(wait);
                onStep.Invoke(this, curIndex, animationInterval, true);
                yield return new WaitForSeconds(animationInterval);
                curIndex++;
            }
        }

        /// <summary>
        /// 获取方向
        /// </summary>
        /// <param name="cellPosition0"></param>
        /// <param name="cellPosition1"></param>
        /// <returns></returns>
        protected Direction GetAnimaDirectionInMap(Vector3Int cellPosition0, Vector3Int cellPosition1)
        {
            Vector3Int offset = cellPosition1 - cellPosition0;

            if (Mathf.Abs(offset.x) < Mathf.Abs(offset.y))
            {
                return offset.y > 0 ? Direction.Up : Direction.Down;
            }
            else
            {
                return offset.x > 0 ? Direction.Right : Direction.Left;
            }
        }

        /// <summary>
        /// 运行动画，并返回长度
        /// </summary>
        /// <param name="combat"></param>
        /// <param name="actor"></param>
        /// <param name="other"></param>
        /// <param name="dirs"></param>
        /// <returns></returns>
        protected virtual float RunAniamAndGetLengthInMap(CombatVariable actor, CombatVariable other, Direction[] dirs)
        {
            CombatUnit actorUnit = combat.GetCombatUnit(actor.position);
            if (actorUnit == null || actorUnit.mapClass == null)
            {
                return 0f;
            }

            ClassAnimatorController actorAnima = actorUnit.mapClass.animatorController;
            Direction dir = dirs[actor.position];
            float length = 0.5f;

            switch (actor.animaType)
            {
                case CombatAnimaType.Prepare:
                    actorAnima.PlayPrepareAttack(dir, actorUnit.weaponType);
                    break;
                case CombatAnimaType.Attack:
                case CombatAnimaType.Heal:
                    actorAnima.PlayAttack();
                    length = actorAnima.GetAttackAnimationLength(dir, actorUnit.weaponType);
                    break;
                case CombatAnimaType.Evade:
                    actorAnima.PlayEvade();
                    length = actorAnima.GetEvadeAnimationLength(dir);
                    break;
                case CombatAnimaType.Damage:
                    actorAnima.PlayDamage();
                    length = actorAnima.GetDamageAnimationLength(dir);

                    // TODO 受到爆击的额外动画，假定是晃动
                    // if (other.crit)
                    // {
                    //     CommonAnima.PlayShake(actorUnit.mapClass.gameObject);
                    //     length = Mathf.Max(length, CommonAnima.shakeLength);
                    // }

                    break;
                case CombatAnimaType.Dead:
                    // TODO 播放死亡动画，我把死亡忘记了
                    break;
                default:
                    break;
            }
            return length;
        }
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ClassAnimatorController.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 01 Feb 2018 23:49:32 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps
{
    [RequireComponent(typeof(Animator))]
    public class ClassAnimatorController : MonoBehaviour
    {
        #region Field
        private Animator m_Animator;
        #endregion

        #region Property
        public Animator animator
        {
            get
            {
                if (m_Animator == null)
                {
                    m_Animator = GetComponent<Animator>();
                }
                return m_Animator;
            }
        }
        #endregion

        #region Unity Callback
        private void Awake()
        {
            if (m_Animator == null)
            {
                m_Animator = GetComponent<Animator>();
            }
        }
        #endregion

        #region Clip Length Method
        public float GetAttackAnimationLength(Direction direction, WeaponType weapon)
        {
            string clipName = GetAttackAnimationName(direction, weapon);
            return animator.GetClipLength(clipName);
        }
        #endregion

        #region Play Method
        public void SetMoveDirection(Direction direction)
        {
            if (animator.GetBool("PrepareAttack"))
            {
                return;
            }

            animator.SetInteger("Direction", direction.ToInteger());
        }

        public void PlayMove()
        {
            if (!animator.GetBool("PrepareAttack") && !animator.GetBool("Move"))
            {
                animator.SetBool("Move", true);
            }
        }

        public void StopMove()
        {
            if (animator.GetBool("Move"))
            {
                animator.SetBool("Move", false);
            }
        }

        public void PlayPrepareAttack(Direction direction, WeaponType weapon)
        {
            StopMove();

            if (!animator.GetBool("PrepareAttack"))
            {
                animator.SetInteger("Direction", direction.ToInteger());
                animator.SetInteger("Weapon", weapon.ToInteger());
                animator.SetBool("PrepareAttack", true);
            }
        }

        public void StopPrepareAttack()
        {
            if (animator.GetBool("PrepareAttack"))
            {
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Evade");
                animator.ResetTrigger("Damage");
                animator.SetBool("PrepareAttack", false);
            }
        }

        public void PlayAttack()
        {
            if (!animator.GetBool("PrepareAttack"))
            {
                return;
            }

            animator.SetTrigger("Attack");
        }

        public void PlayEvade()
        {
            if (!animator.GetBool("PrepareAttack"))
            {
                return;
            }

            animator.SetTrigger("Evade");
        }

        public void PlayDamage()
        {
            if (!animator.GetBool("PrepareAttack"))
            {
                return;
            }

            animator.SetTrigger("Damage");
        }
        #endregion

        #region Static Method
        public static string GetMoveAnimationName(Direction direction)
        {
            return "Move" + direction.ToString();
        }

        public static string GetAttackAnimationName(Direction direction, WeaponType weapon)
        {
            return weapon.ToString() + direction.ToString();
        }

        public static string GetEvadeAnimationName(Direction direction)
        {
            return "Evade" + direction.ToString();
        }

        public static string GetDamageAnimationName(Direction direction)
        {
            return "Damage" + direction.ToString();
        }
        #endregion
    }
}
#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TypeExtension.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 01 Feb 2018 23:55:45 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev
{
    public static class TypeExtension
    {
        public static int ToInteger(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 在Animator中获取动画Clip
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="clipName"></param>
        /// <returns></returns>
        public static AnimationClip FindClip(this Animator animator, string clipName)
        {
            if (animator == null || string.IsNullOrEmpty(clipName))
            {
                return null;
            }

            if (animator.runtimeAnimatorController == null)
            {
                return null;
            }

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips == null || clips.Length == 0)
            {
                return null;
            }

            return Array.Find<AnimationClip>(clips, clip => clip != null && clip.name == clipName);
        }

        public static float GetClipLength(this Animator animator, string clipName)
        {
            AnimationClip clip = FindClip(animator, clipName);
            if (clip == null)
            {
                return 0;
            }
            return clip.length;
        }
    }
}
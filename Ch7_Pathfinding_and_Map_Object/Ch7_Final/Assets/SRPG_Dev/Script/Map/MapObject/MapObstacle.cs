#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MapObstacle.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 05 Apr 2018 17:16:25 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps
{
    using DR.Book.SRPG_Dev.ColorPalette;

    [AddComponentMenu("SRPG/Map Object/Map Obstacle")]
    public class MapObstacle : MapObject
    {
        [SerializeField]
        private Animator m_Animator;
        [SerializeField]
        private ColorSwapper m_Swapper;

        public Animator animator
        {
            get { return m_Animator; }
            set { m_Animator = value; }
        }

        public ColorSwapper swapper
        {
            get { return m_Swapper; }
            set { m_Swapper = value; }
        }

        public override MapObjectType mapObjectType
        {
            get { return MapObjectType.Obstacle; }
        }
    }
}
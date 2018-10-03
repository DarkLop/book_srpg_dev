#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MapCursor.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 05 Apr 2018 16:28:02 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps
{
    [AddComponentMenu("SRPG/Map Object/Map Cursor")]
    public class MapCursor : MapObject
    {
        public enum CursorType : int
        {
            Mouse = 0,
            Move = 1,
            Attack = 2
        }


        [SerializeField]
        public Sprite[] m_CursorSprites;

        [SerializeField]
        private Animator m_Animator;

        public override MapObjectType mapObjectType
        {
            get { return MapObjectType.Cursor; }
        }

        public virtual CursorType cursorType
        {
            set
            {
                if (renderer == null)
                {
                    Debug.LogError("Cursor: SpriteRenderer was not found.");
                    return;
                }

                if (m_CursorSprites == null || m_CursorSprites.Length == 0)
                {
                    Debug.LogError("Cursor: there is no sprite.");
                    return;
                }

                int index = (int)value;

                if (index < 0 || index >= m_CursorSprites.Length)
                {
                    Debug.LogError("Cursor: index is out of range.");
                    return;
                }

                renderer.sprite = m_CursorSprites[index];
            }
        }

        public Animator animator
        {
            get { return m_Animator; }
            set { m_Animator = value; }
        }

        public override void OnSpawn()
        {
            if (renderer == null)
            {
                renderer = gameObject.GetComponentInChildren<SpriteRenderer>(true);
                if (renderer == null)
                {
                    Debug.LogError("Cursor: SpriteRenderer was not found.");
                    return;
                }
            }
        }

        public override void OnDespawn()
        {
            if (map != null && mapObjectType == MapObjectType.Cursor)
            {
                CellData cellData = map.GetCellData(cellPosition);
                if (cellData != null)
                {
                    cellData.hasCursor = false;
                }
            }

            base.OnDespawn();
        }
    }
}
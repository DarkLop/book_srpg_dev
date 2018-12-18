#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IConfigSerializer.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 08 Oct 2018 00:42:55 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    public interface IEditorConfigSerializer
    {
        Array EditorGetKeys();
        void EditorSortDatas();
        byte[] EditorSerializeToBytes();
        void EditorDeserializeToObject(byte[] bytes);
    }
}
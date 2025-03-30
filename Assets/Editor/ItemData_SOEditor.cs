using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData_SO))]
public class ItemData_SOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemData_SO itemData = (ItemData_SO)target;
        DrawDefaultInspector();

        if (itemData.ItemType == ItemTypeEnums.Equipment)
        {
            SerializedProperty equipmentTypeProperty = serializedObject.FindProperty("equipmentType");
            EditorGUILayout.PropertyField(equipmentTypeProperty, new GUIContent("Equipment Type"));
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(itemData);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
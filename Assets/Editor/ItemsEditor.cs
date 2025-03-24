using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Items))]
public class ItemsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        Items items = (Items)target;
        items.ItemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", items.ItemType);
        
        if (items.ItemType == ItemType.Equipment)
        {   
            items.EquipmentType = (EquipmentType)EditorGUILayout.EnumPopup("Equipment Type", items.EquipmentType);
        }
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestLunch))]
public class TestLunchEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestLunch comp = (TestLunch)target;

        if (GUILayout.Button("Test Button"))
            comp.LaunchTest();
    }
}

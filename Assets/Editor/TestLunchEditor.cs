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
[CustomEditor(typeof(TestLunch_Beta))]
public class TestLunchEditor_Beta : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TestLunch_Beta comp = (TestLunch_Beta)target;

        if (GUILayout.Button("Test Button"))
            comp.LaunchTest();
    }
}


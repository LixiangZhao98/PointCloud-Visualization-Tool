using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RunTimeControl))]
public class RunTimeControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RunTimeControl runTimeControl = (RunTimeControl)target;


        if (runTimeControl.custom)
        {
            runTimeControl.customGenerator = (CustomGeneratorEnum)EditorGUILayout.EnumPopup("Custom Generator", runTimeControl.customGenerator);
        }
        else
        {
            runTimeControl.dataset = (Dataset)EditorGUILayout.EnumPopup("Dataset", runTimeControl.dataset);
        }
        
        runTimeControl.custom = EditorGUILayout.Toggle("Use_Function_Defined_Yourself", runTimeControl.custom);


        // DrawDefaultInspector();
    }
}
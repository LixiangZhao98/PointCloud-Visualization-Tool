using UnityEditor;
using UnityEngine;
using System;
[CustomEditor(typeof(RunTimeControl))]
public class RunTimeControlEditor : Editor
{
    String s="";
    public override void OnInspectorGUI()
    {
        RunTimeControl runTimeControl = (RunTimeControl)target;

        runTimeControl.custom = EditorGUILayout.Toggle("Use_Function_Defined_Yourself", runTimeControl.custom);
        if (runTimeControl.custom)
        {
            EditorGUILayout.BeginHorizontal();
            runTimeControl.customGenerator = (CustomGeneratorEnum)EditorGUILayout.EnumPopup("Custom Function", runTimeControl.customGenerator);
            if (GUILayout.Button("Add Function", GUILayout.Width(100)))
            {
                OpenDataGeneratorScript();
            }
            EditorGUILayout.EndHorizontal();
            
            
            EditorGUILayout.BeginHorizontal();
            
            s= EditorGUILayout.TextField("Enter the name without extension to save as .bin", s);
            
            if (GUILayout.Button("Save",GUILayout.Width(100)))
            {
                if (s == "")
                {
                    runTimeControl.GetComponentInChildren<DataLoader>().SaveAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" +"CustomizedDataset.bin");

                }
                    else
                {
                    runTimeControl.GetComponentInChildren<DataLoader>().SaveAsNewData(Application.dataPath + "/PointCloud-Visualization-Tool/data/" + "/data/" + s+".bin");

                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            runTimeControl.dataset = (Dataset)EditorGUILayout.EnumPopup("Dataset", runTimeControl.dataset);
        }
        // DrawDefaultInspector();
    }
    
    private void OpenDataGeneratorScript()
    {
        string scriptPath = "Assets/PointCloud-Visualization-Tool/script/dataprocessing/DataGenerator.cs";

        EditorUtility.OpenWithDefaultApp(scriptPath);
    }
}
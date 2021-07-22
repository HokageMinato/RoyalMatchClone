using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridEditorUtility))]
public class GridEditor : Editor
{
  
    
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawGenerateButton();
        DrawResetButton();
    }

    private void DrawGenerateButton()
    {
        GridEditorUtility gridEditor = (GridEditorUtility) target;
        if (GUILayout.Button("Generate Grid"))
        {
            gridEditor.GenerateGrid();
        }
    }
    
    private void DrawResetButton()
    {
        GridEditorUtility gridEditor = (GridEditorUtility) target;
        if (gridEditor.IsGridGenerated)
        {
            if (GUILayout.Button("Reset Grid"))
            {
                gridEditor.ResetGrid();
            }
        }
    }
}

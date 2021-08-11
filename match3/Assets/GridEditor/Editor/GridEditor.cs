using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGeneratorUtility))]
public class GridEditor : Editor
{
  
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawGenerateButton();
        DrawResetButton();
        DrawSourcesButton();
    }

    private void DrawGenerateButton()
    {
        GridGeneratorUtility gridGenerator = (GridGeneratorUtility) target;
        if (!gridGenerator.IsGridGenerated)
        {
            if (GUILayout.Button("Generate Grid"))
            {
                gridGenerator.GenerateGrid();
            }
        }
    }
    
    private void DrawResetButton()
    {
        GridGeneratorUtility gridGenerator = (GridGeneratorUtility) target;
        if (gridGenerator.IsGridGenerated)
        {
            if (GUILayout.Button("Reset Grid"))
            {
                gridGenerator.ResetGrid();
            }
        }
    }
    private void DrawSourcesButton()
    {
        GridGeneratorUtility gridGenerator = (GridGeneratorUtility) target;
        if (gridGenerator.IsGridGenerated)
        {
            if (GUILayout.Button("SetSources"))
            {
                gridGenerator.SetSources();
            }
        }
    }
}

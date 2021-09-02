// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
//
// public class GridGeneratorUtility : MonoBehaviour
// {
//     #region EDITOR_VARS
//     [SerializeField] private GridCell gridPrefab;
//     [SerializeField] private GameObject seperatorPrefab;
//     [SerializeField] private int gridHeight;
//     [SerializeField] private int gridWidth;
//    
//     [SerializeField] private Transform gridPivot;
//     #endregion
//
//     #region PUBLIC_VARS
//     public bool IsGridGenerated => grid != null;
//     #if UNITY_EDITOR
//     public List<GridCell> cells;
//     public List<GameObject> seperators;
//     #endif
//     #endregion
//     
//     
//     #region PRIVATE_VARS
//     private GridCell[,] grid;
//     private int widthSpacing;
//     private int heightSpacing;
//     #endregion
//     
//     
//     
//     #region PUBLIC_METHODS
//     public void GenerateGrid()
//     {
//         if (!GetComponent<Grid>())
//         {
//             gameObject.AddComponent<Grid>();
//         }
//
//         if (ValidInputEntered())
//         {
//             grid = new GridCell[gridHeight,gridWidth];
//             Vector3 pivot = gridPivot.localPosition;
//             
//             for (int i = 0; i < grid.GetLength(0); i++)
//             {
//                 for (int j = 0; j < grid.GetLength(1); j++)
//                 {
//                     grid[i, j] = Instantiate(gridPrefab, transform);
//                     GridCell cell = grid[i, j];
//                     cell.gameObject.name = $"({i},{j})";
//                     cell.transform.localPosition = pivot;
//                     cells.Add(cell);
//                     cell.Init(i,j);
//                     pivot.x += widthSpacing;
//                 }
//
//                 seperators.Add(Instantiate(seperatorPrefab, transform));
//                 pivot.y -= heightSpacing;
//                 pivot.x = gridPivot.transform.localPosition.x;
//             }
//         }
//         else
//         {
//             Debug.LogError("Enter Valid Inputs");
//         }
//     }
//
//
//     public void ResetGrid()
//     {
//         foreach (GridCell gridCell in cells)
//         {
//             DestroyImmediate(gridCell.gameObject);
//         }
//
//         foreach (GameObject seperator in seperators)
//         {
//             DestroyImmediate(seperator.gameObject);
//         }
//         seperators.Clear();
//         cells.Clear();
//         grid = null;
//     }
//
//
//    
//   
//     public void OnValidate()
//     {
//         SetSizes();
//         FillArray();
//     }
//
//     #endregion
//     
//     #region PRIVATE_METHODS
//
//     private void FillArray()
//     {
//         if (cells.Count <= 0)
//         {
//             grid = null;
//             return;
//         }
//
//         if (grid == null)
//         {
//             grid = new GridCell[gridWidth,gridHeight];
//             int counter = 0;
//             for (int i = 0; i < grid.GetLength(0); i++)
//             {
//                 for (int j = 0; j < grid.GetLength(1); j++)
//                 {
//                     grid[i, j] = cells[counter];
//                 }
//             }
//         }
//
//     }
//
//     private bool ValidInputEntered()
//     {
//         bool isInputValid = true;
//
//         isInputValid = gridHeight > 0 && gridWidth > 0;
//         return isInputValid;
//     }
//
//     private void SetSizes()
//     {
//         if(!IsGridGenerated) return; 
//         
//         Vector3 pivot = gridPivot.localPosition;
//         
//         for (int i = 0; i < grid.GetLength(0); i++)
//         {
//             for (int j = 0; j < grid.GetLength(1); j++)
//             {
//                 GridCell cell = grid[i, j];
//                 cell.transform.localPosition = pivot;
//                 pivot.x += widthSpacing;
//             }
//
//             pivot.y += heightSpacing;
//             pivot.x = gridPivot.transform.localPosition.x;
//         }  
//     }
//     #endregion
//
//     
// }

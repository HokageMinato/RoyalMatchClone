using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameplayManager: Singleton<GameplayManager>
{
    
    public GridDesignTemp levelData;

    

    private void Start()
    {
        levelData = GridDesignTemp.GetDefault();
        Init();
    }

    public void Init()
    {
        Grid.instance.GenerateGrid();
        GameplayObstacleHandler.instance.Init(levelData);
        Grid.instance.UpdateInterreferences();
    }



}


public class GridDesignTemp
{
    public static float gridSpacing=.9f;

    public int[,] gridDesignTemp;
    public float maxWidthMidPointForThisPattern;
    public float maxHeightMidPointForThisPattern;
    public int gridHeight;
    public int gridWidth;

    public int this[int i, int j]
    {
        get { return gridDesignTemp[i,j]; }
    }


    public static GridDesignTemp GetDefault() {

        GridDesignTemp levelData = new GridDesignTemp();

        levelData.gridDesignTemp = new[,]
         {
            {1,1,1,1,1,66,1,1,1},
            {1,1,1,66,1,1,66,1,1},
            {1,1,66,1,66,66,66,1,1},
            {0,0,1,66,1,66,1,0,0},
            {0,0,1,1,1,1,1,0,0},
            {0,0,1,1,1,1,1,0,0},
            {1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1}
        };

        //levelData.gridDesignTemp = new[,]
        //{
        //    {1,1,1,1,1,1,1,1,1},
        //    {1,1,1,1,1,1,1,1,1},
        //    {1,1,1,1,1,1,1,1,1},
        //    {0,0,66,1,1,1,1,1,1},
        //    {0,0,1,1,66,1,1,1,1},
        //    {0,0,1,1,1,1,1,66,66},
        //    {3,1,1,1,1,1,1,66,66},
        //    {3,1,1,1,1,1,1,1,1},
        //    {1,1,1,1,1,1,1,1,1}
        //};


        levelData.maxWidthMidPointForThisPattern = 3.4f;
    levelData.maxHeightMidPointForThisPattern = 3.4f;
    levelData.gridHeight = 9;
    levelData.gridWidth = 9;


        //temp code random block generate
        //for (int i = 0; i <levelData.gridWidth ; i++)
        //{
        //    for (int j = 0; j < levelData.gridHeight; j++)
        //    {
        //        if (levelData.gridDesignTemp[i, j]!= GridConstants.NO_CELL) {

        //            if (Random.Range(0, 100) % 6 == 0) {
        //                levelData.gridDesignTemp[i, j] = 66;
        //            }
                    
        //        }
        //    }

        //}
        //



        return levelData;
}

    // height width calculation ??IMPLEMENT IN EDITOR
    //     for (int i = 0; i < 8; i++)
    // {
    //     for (int j = 0; j < 8; j++)
    //     {
    //         if (_grid[i,j] == null)
    //             continue;
    //
    //         gridHeight = Mathf.Max(Mathf.Abs(_grid[i, j].transform.localPosition.y),gridHeight);
    //         Debug.Log(gridHeight);
    //         Debug.Log(_grid[i, j].transform.localPosition.y);
    //     }
    // }


    
}
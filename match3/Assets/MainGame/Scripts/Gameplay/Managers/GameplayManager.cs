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
        GameplayObstacleHandler.instance.GenerateObstacles(levelData);
        Factory.instance.GenerateElementHandlers();
    }



}


public class GridDesignTemp
{

    public int[,] gridDesignTemp;
    public float gridSpacing = 2.0f;
    public float maxWidthMidPointForThisPattern = 7;
    public float maxHeightMidPointForThisPattern = 7;
    public int gridHeight = 8;
    public int gridWidth = 8;



    public static GridDesignTemp GetDefault() {

        GridDesignTemp levelData = new GridDesignTemp();

       levelData.gridDesignTemp = new[,]
    {
        {1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1},
        {1,1,66,1,1,1,1,1,1},
        {0,0,1,1,1,1,1,0,0},
        {0,0,1,1,1,1,1,0,0},
        {0,0,1,1,1,1,1,0,0},
        {2,2,1,1,1,1,1,2,2},
        {2,2,1,1,1,1,1,2,2},
        {2,2,1,1,1,1,1,2,2}
    };
    levelData.gridSpacing = 2.0f;
    levelData.maxWidthMidPointForThisPattern = 7;
    levelData.maxHeightMidPointForThisPattern = 7;
    levelData.gridHeight = 8;
    levelData. gridWidth = 8;

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
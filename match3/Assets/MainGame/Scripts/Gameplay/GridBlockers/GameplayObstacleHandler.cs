using System.Collections.Generic;
using UnityEngine;
using System;

public class GameplayObstacleHandler : Singleton<GameplayObstacleHandler>
{

    public List<CellBlocker> activeBlockers;
    
    public void GenerateObstacles(GridDesignTemp levelData)
    {
        ObstacleGenerator obstacleGenerator = Factory.instance.ObstacleGenerator;
        int[,] gridDesign = levelData.gridDesignTemp;
        Grid grid = Grid.instance;

        for (int i = 0; i < levelData.gridWidth; i++)
        {
            for (int j = 0; j < levelData.gridWidth; j++) 
            {
                CellBlocker blocker =  obstacleGenerator.GenerateBlocker(gridDesign[i, j]);
                if (blocker!=null)
                {
                    activeBlockers.Add(blocker);
                    blocker.Init(grid);
                }
                
            }

        }
       
    }


    public void CheckForNeighbourHit(MatchExecutionData executionData) {

        List<GridCell> matchedCell = executionData.patternCells;


        for (int i = 0; i < activeBlockers.Count; i++)
        {
                activeBlockers[i].Hit(matchedCell);
        }
    }

}

public enum BlockLayer { 

    BOTTOM_LAYER = 5,
    MIDDLE_LAYER = 6,
    TOP_LAYER = 7
}
using System;
using System.Collections.Generic;


public class GameplayObstacleHandler : Singleton<GameplayObstacleHandler>
{
    public List<CellBlocker> activeBlockers;
    
    public void GenerateObstacles(GridDesignTemp levelData)
    {
        ObstacleFactory obstacleGenerator = ObstacleFactory.instance;
        int[,] gridDesign = levelData.gridDesignTemp;
        Grid grid = Grid.instance;

        for (int i = 0; i < levelData.gridHeight; i++)
        {
            for (int j = 0; j < levelData.gridWidth; j++) 
            {
                int cellType = gridDesign[i, j];

                //if (obstacleGenerator.IsBlockerType(cellType)) {
                if((UnityEngine.Random.Range(0,100)) % 6 == 0 && cellType!=0){ 
                     //CellBlocker blocker = obstacleGenerator.GenerateBlocker(cellType);
                    CellBlocker blocker = obstacleGenerator.GenerateBlocker(66);
                    blocker.Init(grid, i, j);
                    activeBlockers.Add(blocker);

                    

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

    public void DiscardObstacle(CellBlocker blocker) {
        activeBlockers.Remove(blocker);
        Destroy(blocker.gameObject);
    }

    internal bool IsCellBelowObstacle(int i_HIndex, int j_WIndex)
    {
        for (int i = 0; i < activeBlockers.Count; i++)
        {
            CellBlocker blocker = activeBlockers[i];
            if (blocker.initial_h < i_HIndex && blocker.initial_w == j_WIndex)
                return true;
        }
        return false;
    }

    
}

public enum RenderLayer {

    GridLayer,
    ElementUnderlayBlockerLayer,
    ElementLayer,
    ElementBlockerLayer,
    OverlayBlockerLayer

}
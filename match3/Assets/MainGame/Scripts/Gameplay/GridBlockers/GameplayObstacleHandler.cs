using System.Collections.Generic;


public class GameplayObstacleHandler : Singleton<GameplayObstacleHandler>
{
    public List<CellBlocker> activeBlockers;
    
    public void GenerateObstacles(GridDesignTemp levelData)
    {
        ObstacleFactory obstacleGenerator = ElementGeneratorFactory.instance.ObstacleGenerator;
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
                    blocker.Init(grid,j-1,i-1);
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

}

public enum GameLayer {

    BASE_LAYER = 10,
    BOTTOM_LAYER=11,
    ELEMENT_LAYER=12,
    MIDDLE_LAYER=13,
    TOP_LAYER=14
}
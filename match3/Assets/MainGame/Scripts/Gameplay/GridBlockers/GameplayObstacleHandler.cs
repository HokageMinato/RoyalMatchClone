using System.Collections.Generic;
using UnityEngine;

public class GameplayObstacleHandler : Singleton<GameplayObstacleHandler>
{
    public List<CellBlocker> activeBlockers;
    private Dictionary<int, int> colIdxTopObsIdxLookup= new Dictionary<int, int>();

    public void Init(GridDesignTemp levelData)
    {
        GenerateObstacles(levelData);
        UpdateObstacleColIndexLookup();
    }

    private void GenerateObstacles(GridDesignTemp levelData)
    {
        ObstacleFactory obstacleGenerator = ObstacleFactory.instance;
        int[,] gridDesign = levelData.gridDesignTemp;
        Grid grid = Grid.instance;



        for (int i = 0; i < levelData.gridHeight; i++)
        {
            for (int j = 0; j < levelData.gridWidth; j++)
            {
                int cellType = gridDesign[i, j];

                if (obstacleGenerator.IsBlockerType(cellType))
                {
                    CellBlocker blocker = obstacleGenerator.GenerateBlocker(cellType);
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
        colIdxTopObsIdxLookup.Remove(blocker.initial_w);
        UpdateObstacleColIndexLookup();
        Destroy(blocker.gameObject);
    }

    private void UpdateObstacleColIndexLookup() {

      
        for (int i = 0; i < activeBlockers.Count; i++)
        {

            CellBlocker blocker = activeBlockers[i];
            if (!colIdxTopObsIdxLookup.ContainsKey(blocker.initial_w))
            {
                colIdxTopObsIdxLookup.Add(blocker.initial_w, blocker.initial_h);
            }
            else
            {
                int colPlacement = blocker.initial_h;
                colIdxTopObsIdxLookup[blocker.initial_w] = UnityEngine.Mathf.Min(colPlacement, colIdxTopObsIdxLookup[blocker.initial_w]);
            }

        }

       
    }

    internal bool IsCellBelowObstacle(int i_HIndex, int j_WIndex)
    {
        if (!colIdxTopObsIdxLookup.ContainsKey(j_WIndex)) {
            return false;
        }

        return colIdxTopObsIdxLookup[j_WIndex] < i_HIndex;
    }

    
}

public enum RenderLayer {

    GridLayer,
    ElementUnderlayBlockerLayer,
    ElementLayer,
    ElementBlockerLayer,
    OverlayBlockerLayer

}
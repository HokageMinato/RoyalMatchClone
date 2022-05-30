using System.Collections.Generic;
using UnityEngine;

public class GameplayObstacleHandler : Singleton<GameplayObstacleHandler>
{

    private List<BaseCellBlocker> activeBlockers=new List<BaseCellBlocker>();
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

        for (int i = 0; i < levelData.gridHeight; i++)
        {
            for (int j = 0; j < levelData.gridWidth; j++)
            {
                int cellType = gridDesign[i, j];

                if (obstacleGenerator.IsBlockerType(cellType))
                {
                    BaseCellBlocker blocker = obstacleGenerator.GenerateBlocker(cellType);
                    GridCell cell = Grid.instance[i, j];
                    blocker.Init(cell,i, j);
                    activeBlockers.Add(blocker);
                }

            }
        }
    }

    public void CheckForNeighbourHit(MatchExecutionData executionData) 
    {
        for (int i = 0; i < activeBlockers.Count; i++)
        {
            activeBlockers[i].Hit(executionData);
        }
       
    }

    public void DiscardObstacle(BaseCellBlocker blocker,GridCell targetCell) {

        activeBlockers.Remove(blocker);
        colIdxTopObsIdxLookup.Remove(blocker.initial_w);
        UpdateObstacleColIndexLookup();
        Destroy(blocker.gameObject);
    }

    private void UpdateObstacleColIndexLookup() {

      
        for (int i = 0; i < activeBlockers.Count; i++)
        {

            BaseCellBlocker blocker = activeBlockers[i];
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


    public bool IsCellBlocked(GridCell cell) 
    {
        for (int i = 0; i < activeBlockers.Count; i++)
        {
            if (activeBlockers[i].DoesBlockCell() && activeBlockers[i].TargetCell == cell) 
            {
                return true;
            }
        }
        return false;
    }

    public int GetBlockedCount() 
    {
        int count = 0;  
        for (int i = 0; i < activeBlockers.Count; i++)
        {
            if (activeBlockers[i].DoesBlockCell())
                count++;
        }
        return count;
    }

    
}

public enum RenderLayer {

    GridLayer,
    ElementUnderlayBlockerLayer,
    ElementLayer,
    ElementBlockerLayer,
    OverlayBlockerLayer

}
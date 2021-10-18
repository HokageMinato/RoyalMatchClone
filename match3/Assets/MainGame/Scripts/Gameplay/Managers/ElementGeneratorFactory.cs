using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementGeneratorFactory : Singleton<ElementGeneratorFactory>
{
    #region PRIVATE_VARIABLES
    [SerializeField] private ElementGenerator elementGeneratorActivePrefab;
    [SerializeField] private ObstacleFactory obstacleGenerator;
    #endregion
    
    #region PUBLIC_METHODS
    public ObstacleFactory ObstacleGenerator
    {
        get { return obstacleGenerator; }
    }


    public void GenerateElementHandlers()
    {
        GridDesignTemp levelData = GameplayManager.instance.levelData;
        Grid _grid = Grid.instance;
        for (int j = 0; j < levelData.gridWidth; j++)
        {
            ElementGenerator generator = Instantiate(elementGeneratorActivePrefab);
            _grid[j].Init(generator);
        }
    }
    #endregion
    
}

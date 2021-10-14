using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Singleton<Factory>
{

    [SerializeField] private ElementGenerator elementGeneratorActivePrefab;
    [SerializeField] private ObstacleGenerator obstacleGenerator;
    

    public ObstacleGenerator ObstacleGenerator
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

    
}

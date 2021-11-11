using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementGeneratorFactory : Singleton<ElementGeneratorFactory>
{
    #region PRIVATE_VARIABLES
    [SerializeField] private ElementFactory elementGeneratorActivePrefab;
    

    public void GenerateElementHandlers()
    {
        GridDesignTemp levelData = GameplayManager.instance.levelData;
        Grid _grid = Grid.instance;

        Transform layeredTransformParent = Grid.instance.GetLayerTransformParent(ElementConfig.renderLayer);

        for (int j = 0; j < levelData.gridWidth; j++)
        {
            ElementFactory generator = Instantiate(elementGeneratorActivePrefab,layeredTransformParent);
            int colToMyLeftIdx = j - 1;
            int colToMyRighttIdx = j + 1;

            GridColoumn leftColoumn=null, rightColoumn=null;
            

            if (colToMyLeftIdx > -1)
                leftColoumn = _grid[colToMyLeftIdx];

            if (colToMyRighttIdx < _grid.GridWidth)
                rightColoumn = _grid[colToMyRighttIdx];
            

            _grid[j].Init(generator,leftColoumn,rightColoumn);
        }

        

    }


    #endregion

}

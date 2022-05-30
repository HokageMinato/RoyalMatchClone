using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : Singleton<ObstacleFactory>
{
    #region PRIVATE_VARIABLES
    [SerializeField] private ObstacleData[] obstacleData;
    private Dictionary<int, BaseCellBlocker> prefabLookUp = new Dictionary<int, BaseCellBlocker>();
    #endregion


    #region UNITY_CALLLBACKS
    public override void Awake(
)    {
        base.Awake();
        GenerateLookUp();
    }
    #endregion



    #region PUBLIC_METHODS
    public bool IsBlockerType(int cellType) {
        return prefabLookUp.ContainsKey(cellType);
    }

    public BaseCellBlocker GenerateBlocker(int blockerType)
    {
        BaseCellBlocker blocker = Instantiate(prefabLookUp[blockerType]);
        blocker.UpdateRenderLayer();
        return blocker;
    }
    #endregion
    

    #region PRIVATE_VARIABLES
    private void GenerateLookUp()
    {
        for (int i = 0; i < obstacleData.Length; i++)
        {
            prefabLookUp.Add(obstacleData[i].obstacleId, obstacleData[i].prefab);
        }
    }
    #endregion


    #region INTERNAL_CLASS_DEFININTIONS
    [System.Serializable]
    public class ObstacleData
    {
        public int obstacleId;
        public BaseCellBlocker prefab;
    }
    #endregion  

}


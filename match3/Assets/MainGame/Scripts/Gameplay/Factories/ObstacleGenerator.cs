using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : Singleton<ObstacleGenerator>
{
    #region PRIVATE_VARIABLES
    [SerializeField] private ObstacleData[] obstacleData;
    private Dictionary<int, CellBlocker> prefabLookUp = new Dictionary<int, CellBlocker>();
    #endregion


    #region UNITY_CALLLBACKS
    public override void Awake(
)    {
        base.Awake();
        GenerateLookUp();
    }
    #endregion


    
    #region PUBLIC_METHODS
    public CellBlocker GenerateBlocker(int blockerType)
    {
        if (prefabLookUp.ContainsKey(blockerType))
            return Instantiate(prefabLookUp[blockerType]);

        return null;
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
        public CellBlocker prefab;
    }
    #endregion  

}


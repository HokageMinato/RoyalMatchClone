using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : Singleton<ObstacleGenerator>
{
    [SerializeField] private ObstacleData[] obstacleData;
    private Dictionary<int, CellBlocker> prefabLookUp;


    public override void Awake()
    {
        base.Awake();
        GenerateLookUp();
    }


    private void GenerateLookUp()
    {
        prefabLookUp = new Dictionary<int, CellBlocker>();

        for (int i = 0; i < obstacleData.Length; i++)
        {
            prefabLookUp.Add(obstacleData[i].obstacleId, obstacleData[i].prefab);
        }
    }


    public CellBlocker GenerateBlocker(int blockerType)
    {
        if (prefabLookUp.ContainsKey(blockerType))
            return Instantiate(prefabLookUp[blockerType]);

        return null;
    }


    [System.Serializable]
    public class ObstacleData
    {
        public int obstacleId;
        public CellBlocker prefab;
    }
}


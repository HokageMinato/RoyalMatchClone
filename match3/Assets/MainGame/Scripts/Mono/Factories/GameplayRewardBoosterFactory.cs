using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayRewardBoosterFactory : Singleton<GameplayRewardBoosterFactory>
{

    [SerializeField]
    private List<EntityTuple<GameplayRewardBoosterConfig, GameObject>> BoosterMap;
    private Dictionary<GameplayRewardBoosterConfig, GameObject> _boosterMap = new Dictionary<GameplayRewardBoosterConfig, GameObject>();

    public void Init()
    {
        foreach (var item in BoosterMap)
            _boosterMap.Add(item.Key, item.Value);
    }


    public IMatchGameplayRewardBooster GenerateGameplayBoosterByConfig(GameplayRewardBoosterConfig config)
    {
        GameObject element = Instantiate(_boosterMap[config]);
        element.transform.SetParent(Grid.instance.GetLayerTransformParent(RenderLayer.ElementLayer));
        return element.GetComponentInChildren<IMatchGameplayRewardBooster>(true);
    }

}

using System.Collections.Generic;
using UnityEngine;

public class ElementFactory : Singleton<ElementFactory>
{
    //Instantiate element with config, remove individual prefabs.
    #region PRIVATE_VARIABLES
    [SerializeField] private Element[] elementPrefabs;
    [SerializeField] private Element[] inGameBoosterElementCounterPartPrefabs;

    private Dictionary<ElementConfig,Element> keyValuePairs = new Dictionary<ElementConfig,Element>();
    private HashSet<ElementConfig> boosterElements = new HashSet<ElementConfig>();
    #endregion

    #region PUBLIC_METHODS
    public void Init() 
    {

        for (int i = 0; i < elementPrefabs.Length; i++)
            keyValuePairs.Add(elementPrefabs[i].ElementConfig, elementPrefabs[i]);

        for (int i = 0; i < inGameBoosterElementCounterPartPrefabs.Length; i++)
        {
            Element element = inGameBoosterElementCounterPartPrefabs[i];
            keyValuePairs.Add(element.ElementConfig, element);
            boosterElements.Add(element.ElementConfig);
        }
        
    }

    public Element GenerateRandomGameplayElement()
    {
        return GenerateElementByConfig(elementPrefabs[Random.Range(0, elementPrefabs.Length)].ElementConfig);
    }

    public Element GenerateElementByConfig(ElementConfig config)
    {
        Element element = Instantiate(keyValuePairs[config]);
        Transform elementTransformParent = Gridd.instance.GetLayerTransformParent(element.RenderLayer);
        element.transform.SetParent(elementTransformParent);
        return element;
    }

    public bool IsBooster(ElementConfig config) 
    { 
        return boosterElements.Contains(config);
    }

    #endregion
    
}

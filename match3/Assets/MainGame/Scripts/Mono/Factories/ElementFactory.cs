using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementFactory : Singleton<ElementFactory>
{

    #region PRIVATE_VARIABLES
    [SerializeField] private Element[] elementPrefabs;
    [SerializeField] private Element[] inGameBoosterElements;

    private Dictionary<ElementConfig,Element> keyValuePairs = new Dictionary<ElementConfig,Element>();
    #endregion

    #region PUBLIC_METHODS
    public void Init() 
    {

        for (int i = 0; i < elementPrefabs.Length; i++)
            keyValuePairs.Add(elementPrefabs[i].ElementConfig, elementPrefabs[i]);

        for (int i = 0; i < inGameBoosterElements.Length; i++)
            keyValuePairs.Add(inGameBoosterElements[i].ElementConfig, inGameBoosterElements[i]);
        
    }

    public Element GenerateRandomGameplayElement()
    {
        return GenerateElementByConfig(elementPrefabs[Random.Range(0, elementPrefabs.Length)].ElementConfig);
    }

    public Element GenerateElementByConfig(ElementConfig config)
    {
        Element element = Instantiate(keyValuePairs[config]);
        Transform elementTransformParent = Grid.instance.GetLayerTransformParent(element.RenderLayer);
        element.transform.SetParent(elementTransformParent);
        return element;
    }
  
    #endregion
    
}

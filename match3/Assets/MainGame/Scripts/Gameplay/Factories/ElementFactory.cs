using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementFactory : Singleton<ElementFactory>
{
    #region PUBLIC_VARIABLES
    public int ActiveElementCount { get { return _allElementsRecord.Count; } }
    #endregion

    #region PRIVATE_VARIABLES
    [SerializeField] private Element[] elementPrefab;
    private HashSet<Element> _allElementsRecord = new HashSet<Element>();
    #endregion

    #region PUBLIC_METHODS
    public Element GenerateRandomElement()
    {
        Element element = Instantiate(elementPrefab[Random.Range(0, elementPrefab.Length)]);
        Transform elementTransformParent = Grid.instance.GetLayerTransformParent(element.RenderLayer);
        element.transform.SetParent(elementTransformParent);
        element.RegisterOnDestory(OnDestoryElement);
        element.RegisterOnSet(OnElementSetToCell);
        return element;
    }

    public void OnElementSetToCell(Element setElement) 
    {
        _allElementsRecord.Add(setElement);
    }

    private void OnDestoryElement(Element element) 
    { 
        _allElementsRecord.Remove(element);
    }

  
    #endregion
    
}

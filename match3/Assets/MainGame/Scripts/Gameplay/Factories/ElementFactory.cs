﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementFactory : MonoBehaviour
{
    #region PRIVATE_VARIABLES
    [SerializeField] private Element[] elementPrefab;
    
    #endregion

    #region PUBLIC_METHODS
    public Element GetRandomElement()
    {
        Element element = Instantiate(elementPrefab[Random.Range(0, elementPrefab.Length)]);
        Transform elementTransformParent = Grid.instance.GetLayerTransformParent(element.RenderLayer);
        element.transform.SetParent(elementTransformParent);
        
        
        return element;

    }
    #endregion
    
}

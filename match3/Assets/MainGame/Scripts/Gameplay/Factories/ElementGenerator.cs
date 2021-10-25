using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementGenerator : MonoBehaviour
{
    #region PRIVATE_VARIABLES
    [SerializeField] private Element[] elementPrefab;
    #endregion

    #region PUBLIC_METHODS
    public Element GetRandomElement()
    {
       return  Instantiate(elementPrefab[Random.Range(0, elementPrefab.Length)]);
    }
    #endregion
    
}

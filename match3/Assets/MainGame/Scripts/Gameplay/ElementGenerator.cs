using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementGenerator : MonoBehaviour
{
    public Element[] elementPrefab;

    public Element GetRandomElement(GridCell parentCell)
    {
        Debug.Log("Generating");
       return  Instantiate(elementPrefab[Random.Range(0, elementPrefab.Length)], parentCell.transform);
    }
}

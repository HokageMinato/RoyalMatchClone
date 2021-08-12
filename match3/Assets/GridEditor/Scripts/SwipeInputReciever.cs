using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInputReciever : MonoBehaviour
{

    public static SwipeInputReciever Instance;
    
    public void Awake()
    {
       Instance = this;
    }

    public GridCell selectedCell;
    public GridCell secondCell;

}

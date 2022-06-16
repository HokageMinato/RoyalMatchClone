using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T: Singleton<T>
{
    public static T instance;

    public virtual void Awake()
    {
        if(instance == null)
            instance = this as T;
        
    }

    public virtual void OnDestroy()
    {
        instance = null;
        //GC Cleanup
    }
}

[System.Serializable]
public class SerizTuple<T, K> where T : class where K : class 
{
    public T serializedKeyItem;
    public K serializedValueItem;
}

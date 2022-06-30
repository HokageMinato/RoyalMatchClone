using System;
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
public class EntityTuple<T, K> : IEquatable<EntityTuple<T, K>>
{
    public T Key;
    public K Value;

    public bool Equals(EntityTuple<T, K> other)
    {
        return other.Key.Equals(Key) && other.Value.Equals(Value);
    }

    public override string ToString()
    {
        return $"Key:{Key} Value{Value}";
    }

}

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

public readonly struct PersistantBool
{
    private readonly string _key;

    public PersistantBool(string key)
    {
        _key = key;
    }

    public bool Val
    {
        get { return PlayerPrefs.GetInt(_key, 0) == 1; }
        set { PlayerPrefs.SetInt(_key, (value) ? 1 : 0); }
    }
}

public readonly struct PersistantInt
{
    private readonly string _key;

    public PersistantInt(string key)
    {
        _key = key;
    }

    public int Val
    {
        get { return PlayerPrefs.GetInt(_key, 0); }
        set { PlayerPrefs.SetInt(_key, value); }
    }
}

public readonly struct PeristantString
{
    private readonly string _key;
    private readonly string[] _seperator;

    public string SeperatorValue
    {
        get { return _seperator[0] ?? string.Empty; }
    }

    public PeristantString(string key)
    {
        _key = key;
        _seperator = null;
    }
   
    public string Val
    {
        get { return PlayerPrefs.GetString(_key, string.Empty); }
        set { PlayerPrefs.SetString(_key, value); }
    }
}


public static class FastConvertorUtils 
{
    public static T[] FastHashSetToArray<T>(HashSet<T> source) 
    {
        T[] array = new T[source.Count];
        int idx = 0;

        foreach (T item in source)
        {
            array[idx] = item;
            idx++;
        }

        return array;
    }
    
    public static List<T> FastHashSetToList<T>(HashSet<T> source) 
    {
        List<T> list= new List<T>(source.Count);
        int idx = 0;

        foreach (T item in source)
        {
            list[idx] = item;
            idx++;
        }

        return list;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Registry<T>
{
    private readonly Dictionary<string, T> _registry = new Dictionary<string, T>();

    public void Register(string id, T item){
        if(!_registry.ContainsKey(id)){
            _registry.Add(id, item);
            Debug.Log($"Registered {typeof(T).Name}: {id}");
        }else{
            throw new Exception($"{typeof(T).Name} with ID '{id}' is already registered.");
        }
    }

    public T Get(string id){
        if(_registry.TryGetValue(id, out T item)){
            return item;
        }
        throw new Exception($"{typeof(T).Name} with ID '{id}' is not found.");
    }

    public bool Contains(string id){
        return _registry.ContainsKey(id);
    }

    public IEnumerable<string> GetAllKeys(){
        return _registry.Keys;
    }

    public IEnumerable<T> GetAllValues(){
        return _registry.Values;
    }
}

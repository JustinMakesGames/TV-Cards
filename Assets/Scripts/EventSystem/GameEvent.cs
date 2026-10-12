using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Diagnostics;
using System;


public class GameEvent<T> : GameEvent
{
    private event Action<T> _response;

    public virtual void Invoke(T parameter)
    {
        _response?.Invoke(parameter);
    }
    public void AddResponse(Action<T> method)
    {
        _response += method;
    }

    public void RemoveResponse(Action<T> method)
    {
        _response -= method;
    }
}

public class GameEvent : ScriptableObject { }

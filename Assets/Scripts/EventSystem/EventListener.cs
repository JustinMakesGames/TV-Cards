using System;
using UnityEngine;
using UnityEngine.Events;

public class EventListener<T> : MonoBehaviour
{
    public GameEvent<T> _event;

    public UnityEvent<T> _response;

    private void OnEnable()
    {
        if (_event == null)
        {
            Debug.LogError($"No Event Found on {name}");
            return;
        }
        _event.AddResponse(OnEvent);
    }

    private void OnDisable()
    {
        if (_event == null)
        {
            Debug.LogError($"No Event Found on {name}");
            return;
        }

        _event.RemoveResponse(OnEvent);
    }

    public virtual void OnEvent(T value)
    {
        if (_response == null)
        {
            Debug.LogError($"Event listener {name} does not have a response when an event is fired.");
            return;
        }
        Debug.Log($"Heard event on object");
        _response.Invoke(value);
    }

}

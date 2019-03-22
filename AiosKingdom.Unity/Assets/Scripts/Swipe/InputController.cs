using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(SwipeManager))]
public class InputController : MonoBehaviour
{
    public static InputController This { get; set; }

    public class InputCallback
    {
        public event Action<SwipeDirection> Callback;

        public void Call(SwipeDirection direction)
        {
            if (Callback != null)
            {
                Callback.Invoke(direction);
            }
        }
    }
    private Dictionary<string, InputCallback> _callbacks = new Dictionary<string, InputCallback>();
    private string _currentId = string.Empty;

    void Start()
    {
        This = this;
        DontDestroyOnLoad(this.gameObject);

        SwipeManager swipeManager = GetComponent<SwipeManager>();
        swipeManager.onSwipe += HandleSwipe;
        swipeManager.onLongPress += HandleLongPress;
    }

    public void SetId(string id)
    {
        _currentId = id;
    }

    public void AddCallback(string id, Action<SwipeDirection> callback)
    {
        if (_callbacks.ContainsKey(id))
        {
            _callbacks[id].Callback += callback;
        }
        else
        {
            var inputCallback = new InputCallback();
            inputCallback.Callback += callback;
            _callbacks.Add(id, inputCallback);
        }
    }

    private void _invokeCallback(string id, SwipeDirection direction)
    {
        if (_callbacks.ContainsKey(id))
        {
            _callbacks[id].Call(direction);
        }
    }

    void HandleSwipe(SwipeAction swipeAction)
    {
        if (swipeAction.direction == SwipeDirection.Up || swipeAction.direction == SwipeDirection.UpRight)
        {
            _invokeCallback(_currentId, SwipeDirection.Up);
        }
        else if (swipeAction.direction == SwipeDirection.Right || swipeAction.direction == SwipeDirection.DownRight)
        {
            _invokeCallback(_currentId, SwipeDirection.Right);
        }
        else if (swipeAction.direction == SwipeDirection.Down || swipeAction.direction == SwipeDirection.DownLeft)
        {
            _invokeCallback(_currentId, SwipeDirection.Down);
        }
        else if (swipeAction.direction == SwipeDirection.Left || swipeAction.direction == SwipeDirection.UpLeft)
        {
            _invokeCallback(_currentId, SwipeDirection.Left);
        }
    }

    void HandleLongPress(SwipeAction swipeAction)
    {
        //Debug.LogFormat("HandleLongPress: {0}", swipeAction);
    }
}
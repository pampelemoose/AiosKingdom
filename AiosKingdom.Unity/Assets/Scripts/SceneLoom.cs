using UnityEngine;
using System.Collections.Generic;
using Action = System.Action;

public class SceneLoom : MonoBehaviour
{
    public interface ILoom
    {
        void QueueOnMainThread(Action action);
    }

    private static NullLoom _nullLoom = new NullLoom();
    private static LoomDispatcher _loom;
    public static ILoom Loom
    {
        get
        {
            if (_loom != null)
            {
                return _loom as ILoom;
            }
            return _nullLoom as ILoom;
        }
    }

    void Awake()
    {
        _loom = new LoomDispatcher();
    }
    void OnDestroy()
    {
        _loom = null;
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            _loom.Update();
        }
    }

    private class NullLoom : ILoom
    {
        public void QueueOnMainThread(Action action) { }
    }
    private class LoomDispatcher : ILoom
    {
        private readonly List<Action> queuedActions = new List<Action>();
        private readonly List<Action> actionsToRun = new List<Action>();
        public void QueueOnMainThread(Action action)
        {
            lock (queuedActions)
            {
                queuedActions.Add(action);
            }
        }
        public void Update()
        {
            // Pop the actions from the synchronized list
            actionsToRun.Clear();
            lock (queuedActions)
            {
                actionsToRun.AddRange(queuedActions);
                queuedActions.Clear();
            }
            // Run each action
            for (int i = 0; i < actionsToRun.Count; ++i)
            {
                actionsToRun[i]();
            }
        }
    }

}

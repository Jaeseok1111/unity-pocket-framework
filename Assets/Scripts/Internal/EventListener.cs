using System.Collections.Generic;
using UnityEngine;

namespace Internal
{
    public struct SendEventSignal
    {
        public string Name;
    }

    public class EventListener
    {
        private readonly Dictionary<string, System.Action> _listeners = new();

        public void Listen(string name, System.Action listener)
        {
            _listeners.Add(name, listener);
        }

        public void Invoke(string name)
        {
            if (_listeners.ContainsKey(name) == false)
            {
                Debug.Log($"not found event. event name:{name}");
                return;
            }

            _listeners[name].Invoke();
        }
    }
}
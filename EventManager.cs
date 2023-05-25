using UnityEngine;
using System;
using System.Collections.Generic;

public enum EventName {
    OnTick,         // General - Combat
    OnHeal,         // Health
    OnDamage,
    OnSetHp,
    OnSetMaxHp,
    OnDeath,
    OnAttack,       // Attacking
    OnAttackTick,
}

public class EventManager : MonoBehaviour {
    // Dict holds events per gameObject instance
    static Dictionary<object, Dictionary<EventName, Delegate>> events;

    // public static EventManager Instance => _instance;
    static EventManager _instance;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }

        events = new Dictionary<object, Dictionary<EventName, Delegate>>();
    }

    public static void AddListener<T>(object ownerObj, EventName eventName, Action<T> listener) {
        Dictionary<EventName, Delegate> thisObjEvents;
        if (events.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.ContainsKey(eventName)) {
                // Delegate.Combine adds listeners - similar to event += listener
                thisObjEvents[eventName] = Delegate.Combine(thisObjEvents[eventName], listener);
            }
            else {
                thisObjEvents[eventName] = listener;
            }
        }
        else {
            thisObjEvents = new Dictionary<EventName, Delegate>();
            thisObjEvents[eventName] = listener;
            events[ownerObj] = thisObjEvents;
        }
    }
    public static void AddListener(object ownerObj, EventName eventName, Action listener) {
        Dictionary<EventName, Delegate> thisObjEvents;
        if (events.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.ContainsKey(eventName)) {
                // Delegate.Combine adds listeners - similar to event += listener
                thisObjEvents[eventName] = Delegate.Combine(thisObjEvents[eventName], listener);
            }
            else {
                thisObjEvents[eventName] = listener;
            }
        }
        else {
            thisObjEvents = new Dictionary<EventName, Delegate>();
            thisObjEvents[eventName] = listener;
            events[ownerObj] = thisObjEvents;
        }
    }

    public static void RemoveListener<T>(object ownerObj, EventName eventName, Action<T> listener) {
        Dictionary<EventName, Delegate> thisObjEvents;
        if (events.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.ContainsKey(eventName)) {
                // Delegate.Remove removes listeners - similar to event -= listener
                thisObjEvents[eventName] = Delegate.Remove(thisObjEvents[eventName], listener);
            }
        }
    }

    public static void TriggerEvent<T>(object ownerObj, EventName eventName, T eventData = default) {
        Dictionary<EventName, Delegate> thisObjEvents;
        if (events.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.TryGetValue(eventName, out Delegate eventAction)) {
                if (eventAction is Action<T>) {
                    (eventAction as Action<T>).Invoke(eventData);
                }
            }
        }
    }
    public static void TriggerEvent(object ownerObj, EventName eventName) {
        Dictionary<EventName, Delegate> thisObjEvents;
        if (events.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.TryGetValue(eventName, out Delegate eventAction)) {
                if (eventAction is Action) {
                    (eventAction as Action).Invoke();
                }
            }
        }
    }
}
using UnityEngine;
using System;
using System.Collections.Generic;

public enum EventID {
    PrimaryDown = 0,    // Input
    SecondaryDown = 1,
    EnterCombat = 2,    // General - Combat
    ExitCombat = 3,
    Heal,           // Health
    Damage,
    SetHp,
    SetMaxHp,
    Death,
    AttackReady,    // Attack
    AttackTick,
    SetAttack,
    AbilityReady,   // Ability
    AbilityTick
}

public class EventManager : MonoBehaviour {
    // Dict holds events per gameObject instance
    static Dictionary<object, Dictionary<EventID, Delegate>> eventsDict;

    // public static EventManager Instance => _instance;
    static EventManager _instance;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        }
        else {
            _instance = this;
        }

        eventsDict = new Dictionary<object, Dictionary<EventID, Delegate>>();
    }

    public static void Subscribe<T>(object ownerObj, EventID eventID, Action<T> listener) {
        Dictionary<EventID, Delegate> thisObjEvents;
        if (EventManager.eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.ContainsKey(eventID)) {
                // Delegate.Combine adds listeners - similar to event += listener
                thisObjEvents[eventID] = Delegate.Combine(thisObjEvents[eventID], listener);
            }
            else {
                thisObjEvents[eventID] = listener;
            }
        }
        else {
            thisObjEvents = new Dictionary<EventID, Delegate>();
            thisObjEvents[eventID] = listener;
            EventManager.eventsDict[ownerObj] = thisObjEvents;
        }
    }
    public static void Subscribe(object ownerObj, EventID eventID, Action listener) {
        Dictionary<EventID, Delegate> thisObjEvents;
        if (EventManager.eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.ContainsKey(eventID)) {
                // Delegate.Combine adds listeners - similar to event += listener
                thisObjEvents[eventID] = Delegate.Combine(thisObjEvents[eventID], listener);
            }
            else {
                thisObjEvents[eventID] = listener;
            }
        }
        else {
            thisObjEvents = new Dictionary<EventID, Delegate>();
            thisObjEvents[eventID] = listener;
            EventManager.eventsDict[ownerObj] = thisObjEvents;
        }
    }

    public static void Unsubscribe<T>(object ownerObj, EventID eventID, Action<T> listener) {
        Dictionary<EventID, Delegate> thisObjEvents;
        if (EventManager.eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.ContainsKey(eventID)) {
                // Delegate.Remove removes listeners - similar to event -= listener
                thisObjEvents[eventID] = Delegate.Remove(thisObjEvents[eventID], listener);
            }
        }
    }

    public static void Invoke<T>(object ownerObj, EventID eventID, T eventData = default) {
        Dictionary<EventID, Delegate> thisObjEvents;
        if (eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.TryGetValue(eventID, out Delegate eventAction)) {
                if (eventAction is Action<T>) {
                    (eventAction as Action<T>).Invoke(eventData);
                }
            }
        }
    }
    public static void Invoke(object ownerObj, EventID eventID) {
        Dictionary<EventID, Delegate> thisObjEvents;
        if (eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
            if (thisObjEvents.TryGetValue(eventID, out Delegate eventAction)) {
                if (eventAction is Action) {
                    (eventAction as Action).Invoke();
                }
            }
        }
    }
}
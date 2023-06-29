using UnityEngine;
using System;
using System.Collections.Generic;

public enum EventID {
    PrimaryDown = 0,    // Input
    SecondaryDown = 1,
    ModifyMoney = 10,   // General
    ModifyLife = 11,
    ModifyMaxLife = 12,
    WonGame = 30,
    LostGame = 31,
    EnterCombat = 101,
    ExitCombat = 102,
    StartWave = 103,
    EndWave = 104,
    WaveTick = 105,
    StartBattle = 106,
    WonBattle = 107,
    LostBattle = 108,
    SlotPlaced = 190,           // Slot
    SlotPickedUp = 191,
    Heal = 201,               // Health
    Damage = 202,
    SetHp = 203,
    SetMaxHp = 204,
    Death = 205,
    AnimalDied = 210,
    EnemyDied = 211,
    AttackReady = 220,        // Attack
    AttackTick = 221,
    SetAttack = 222,
    AbilityReady = 230,       // Ability
    AbilityTick = 231,
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
        if (eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
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
            eventsDict[ownerObj] = thisObjEvents;
        }
    }
    public static void Subscribe(object ownerObj, EventID eventID, Action listener) {
        Dictionary<EventID, Delegate> thisObjEvents;
        if (eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
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
            eventsDict[ownerObj] = thisObjEvents;
        }
    }

    public static void Unsubscribe<T>(object ownerObj, EventID eventID, Action<T> listener) {
        Dictionary<EventID, Delegate> thisObjEvents;
        if (eventsDict.TryGetValue(ownerObj, out thisObjEvents)) {
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
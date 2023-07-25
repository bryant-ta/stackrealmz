using UnityEngine;
using System;
using System.Collections.Generic;

public enum EventID {
    None = 0, // default value - should never be invoked
    PrimaryDown = 1,        // Input
    SecondaryDown = 2,
    TertiaryDown = 3,
    ModifyMoney = 10,       // General
    ModifyLife = 11,
    ModifyMaxLife = 12,
    ModifyMana = 13,
    ModifyMaxMana = 14,
    WonGame = 30,
    LostGame = 31,
    CraftDone = 50,
    EnterCombat = 101,
    ExitCombat = 102,
    StartWave = 103,
    EndWave = 104,
    WaveTick = 105,
    StartBattle = 106,
    WonBattle = 107,
    LostBattle = 108,
    SlotPlaced = 190,       // Slot
    SlotPickedUp = 191,
    Heal = 201,             // Health
    Damage = 202,
    SetHp = 203,
    SetMaxHp = 204,
    Death = 205,
    AnimalDied = 210,
    EnemyDied = 211,
    AttackReady = 220,      // Attack
    AttackTick = 221,
    SetAttack = 222,
    SetSpeed = 223,
    AbilityReady = 230,     // Ability
    AbilityTick = 231,
    SetManaCost = 240,      // Mana
    CardPlayed = 300,       // Card Effect Conditions
}

public class EventManager : MonoBehaviour {
    // Dict holds events per gameObject instance
    static Dictionary<object, Dictionary<EventID, Delegate>> eventsDict;
    static Dictionary<object, Dictionary<EventID, Delegate>> oneParamEventsDict;

    // public static EventManager Instance => _instance;
    static EventManager _instance;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        eventsDict = new Dictionary<object, Dictionary<EventID, Delegate>>();
        oneParamEventsDict = new Dictionary<object, Dictionary<EventID, Delegate>>();
    }

    public static void Subscribe(object ownerObj, EventID eventID, Action listener) {
        Dictionary<EventID, Delegate> ownerObjEvents;
        if (eventsDict.TryGetValue(ownerObj, out ownerObjEvents)) {
            if (ownerObjEvents.ContainsKey(eventID)) {
                // Delegate.Combine adds listeners - similar to event += listener
                ownerObjEvents[eventID] = Delegate.Combine(ownerObjEvents[eventID], listener);
            } else {
                ownerObjEvents[eventID] = listener;
            }
        } else {
            ownerObjEvents = new Dictionary<EventID, Delegate>();
            ownerObjEvents[eventID] = listener;
            eventsDict[ownerObj] = ownerObjEvents;
        }
    } 
    public static void Subscribe<T>(object ownerObj, EventID eventID, Action<T> listener) {
        Dictionary<EventID, Delegate> ownerObjEvents;
        if (oneParamEventsDict.TryGetValue(ownerObj, out ownerObjEvents)) {
            if (ownerObjEvents.ContainsKey(eventID)) {
                // Delegate.Combine adds listeners - similar to event += listener
                ownerObjEvents[eventID] = Delegate.Combine(ownerObjEvents[eventID], listener);
            } else {
                ownerObjEvents[eventID] = listener;
            }
        } else {
            ownerObjEvents = new Dictionary<EventID, Delegate>();
            ownerObjEvents[eventID] = listener;
            oneParamEventsDict[ownerObj] = ownerObjEvents;
        }
    }

    // public static void Unsubscribe<T>(object ownerObj, EventID eventID, Action<T> listener) {
    //     Dictionary<EventID, Delegate> ownerObjEvents;
    //     if (eventsDict.TryGetValue(ownerObj, out ownerObjEvents)) {
    //         if (ownerObjEvents.ContainsKey(eventID)) {
    //             // Delegate.Remove removes listeners - similar to event -= listener
    //             ownerObjEvents[eventID] = Delegate.Remove(ownerObjEvents[eventID], listener);
    //         }
    //     }
    // }

    public static void Invoke(object ownerObj, EventID eventID) {
        Dictionary<EventID, Delegate> ownerObjEvents;
        if (eventsDict.TryGetValue(ownerObj, out ownerObjEvents)) {
            if (ownerObjEvents.TryGetValue(eventID, out Delegate eventAction)) {
                if (eventAction is Action) {
                    (eventAction as Action).Invoke();
                }
            }
        }
    }
    public static void Invoke<T>(object ownerObj, EventID eventID, T eventData = default) {
        Dictionary<EventID, Delegate> ownerObjEvents;
        if (oneParamEventsDict.TryGetValue(ownerObj, out ownerObjEvents)) {
            if (ownerObjEvents.TryGetValue(eventID, out Delegate eventAction)) {
                if (eventAction is Action<T>) {
                    (eventAction as Action<T>).Invoke(eventData);
                }
            }
        }
    }
}
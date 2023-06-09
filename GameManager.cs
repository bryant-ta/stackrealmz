using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// GameManager singleton handles day cycle
// Load Order: priority in script load order (otherwise _instance might not be set when used)
public class GameManager : MonoBehaviour {
    public static GameManager Instance => _instance;
    static GameManager _instance;

    public static Canvas WorldCanvas { get; private set; }
    [SerializeField] Canvas _worldCanvas;

    public List<Card> cards;
    public List<Food> foods;
    public List<Animal> animals;

    // Time Vars
    public static int TimeScale = 1;
    public int dayDuration = 1;
    public int nightDuration = 1;
    public Image timeBarFill;

    // Money Vars
    public int Money { get; private set; }

    // Events
    public static UnityEvent onDayEnd = new UnityEvent();
    public static UnityEvent onNightEnd = new UnityEvent();

    //Debug
    public bool doEating;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }

        WorldCanvas = _worldCanvas;
    }

    void Start() {
        StartCoroutine(GameLoop());

        // Debug
        ModifyMoney(3);
    }

    IEnumerator GameLoop() {
        while (true) {
            yield return StartCoroutine(TimeCycle(dayDuration, onDayEnd));
            yield return StartCoroutine(TimeCycle(nightDuration, onNightEnd));
        }
    }

    IEnumerator TimeCycle(float duration, UnityEvent endEvent) {
        while (timeBarFill.fillAmount < 1) {
            timeBarFill.fillAmount += (float) TimeScale / duration * Time.deltaTime;
            yield return null;
        }

        timeBarFill.fillAmount = 0f;
        endEvent.Invoke();

        // if (doEating) {
        //     foreach (Animal v in animals) {
        //         if (foods.Count == 0) {
        //             // TODO: You lost function
        //             print("YOU LOST");
        //             break;
        //         }
        //
        //         // foods[0].Eat();
        //     }
        // }
    }

    // Usage in inspector by time buttons
    public static void SetTimeSpeed(int n) {
        if (n >= 0) {
            TimeScale = n;
        }
    }

    public bool ModifyMoney(int value) {
        int newMoney = Money + value;
        if (newMoney < 0) {
            return false;
        }

        Money = newMoney;
        EventManager.Invoke(gameObject, EventID.ModifyMoney, Money);
        
        return true;
    }
}
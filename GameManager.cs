using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// GameManager singleton handles day cycle
// Load Order: priority in script load order (otherwise _instance might not be set when used)
public class GameManager : MonoBehaviour {
    public static GameManager Instance => _instance;
    static GameManager _instance;

    public Canvas WorldCanvas => _worldCanvas;
    [SerializeField] Canvas _worldCanvas;

    public List<Card> cards;
    public List<Food> foods;
    public List<Animal> animals;
    public List<Animal> enemies;

    // Time Vars
    public int TimeScale => _timeScale;
    [SerializeField] int _timeScale;
    [SerializeField] int dayDuration = 1;
    [SerializeField] int nightDuration = 1;
    [SerializeField] float curTime;
    
    // Life Vars
    public Health playerLife;

    // Mana Vars
    public int Mana => curMana;
    [SerializeField] int curMana; 
    [SerializeField] int maxMana;
    
    // Money Vars
    public int Money => _money;
    [SerializeField] int _money;

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
    }

    void Start() {
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.StartBattle, EnableManaGen);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.WonBattle, DisableManaGen);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.LostBattle, LostGame);
        
        StartCoroutine(GameLoop());

        // Debug
        ModifyMoney(10);
        ModifyMana(5);
    }

    IEnumerator GameLoop() {
        while (true) {
            yield return StartCoroutine(TimeCycle(dayDuration, onDayEnd));
            yield return StartCoroutine(TimeCycle(nightDuration, onNightEnd));
        }
    }

    IEnumerator TimeCycle(float duration, UnityEvent endEvent) {
        while (curTime < duration) {
            curTime += Time.deltaTime * _timeScale;
            UIManager.Instance.UpdateTimeProgressBar(curTime / duration);
            yield return null;
        }

        curTime = 0;
        UIManager.Instance.UpdateTimeProgressBar(curTime / duration);
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

    void WonGame() {
        
    }
    
    void LostGame() {
        print("YOU LOST THE GAME");
        EventManager.Invoke(gameObject, EventID.LostGame);
    }
    
    // Usage in inspector by time buttons
    public void SetTimeSpeed(int n) {
        if (n >= 0) {
            _timeScale = n;
        }
    }
    
    // Mana funcs
    void EnableManaGen() { CombatClock.onTick.AddListener(IncrementMana); }
    void DisableManaGen() { 
        CombatClock.onTick.RemoveListener(IncrementMana); 
        ModifyMana(-curMana);
    }
    void FreezeManaGen() { CombatClock.onTick.RemoveListener(IncrementMana);}   // used to stop gen but not reset mana
    public void ModifyMana(int value) {
        int newMana = curMana + value;
        if (newMana < 0) {
            curMana = 0;
        } else if (newMana > maxMana) {
            curMana = maxMana;
        } else {
            curMana = newMana;
        }

        ManaArgs args = new ManaArgs() {curMana = this.curMana, maxMana = this.maxMana};
        EventManager.Invoke(gameObject, EventID.ModifyMana, args);
    }
    public void IncrementMana() { ModifyMana(1);}
    public void ModifyMaxMana(int value) {
        int newMaxMana = maxMana + value;
        if (newMaxMana < 1) {
            maxMana = 1;
        } else {
            maxMana = newMaxMana;
        }

        ManaArgs args = new ManaArgs() {curMana = this.curMana, maxMana = this.maxMana};
        EventManager.Invoke(gameObject, EventID.ModifyMaxMana, args);
    }

    public bool ModifyMoney(int value) {
        int newMoney = _money + value;
        if (newMoney < 0) {
            return false;
        }

        _money = newMoney;
        EventManager.Invoke(gameObject, EventID.ModifyMoney, _money);
        
        return true;
    }
}

public struct ManaArgs {
    public int curMana;
    public int maxMana;
}
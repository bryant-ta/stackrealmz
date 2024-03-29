using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    public int day = 1;
    
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
    
    // Modifiers
    public Modifiers playerMods;
    public Modifiers enemyMods;

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
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.StartBattle, EndDay);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.WonBattle, EndNight);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.LostBattle, LostGame);
        
        StartCoroutine(GameLoop());
        
        EnableManaGen();

        // Debug
        ModifyMoney(25);
        ModifyMana(10);
    }

    IEnumerator GameLoop() {
        while (true) {
            yield return StartCoroutine(TimeCycle(dayDuration, EventID.EndDay));
            yield return StartCoroutine(TimeCycle(nightDuration, EventID.EndNight));
            
            day++;
            UIManager.Instance.UpdateDayText(day);
        }
    }

    IEnumerator TimeCycle(float duration, EventID endEvent) {
        while (curTime < duration) {
            curTime += Time.deltaTime * _timeScale;
            UIManager.Instance.UpdateTimeProgressBar(curTime / duration);
            yield return null;
        }

        curTime = 0;
        UIManager.Instance.UpdateTimeProgressBar(curTime / duration);
        
        EventManager.Invoke(gameObject, endEvent);
    }
    void EndDay() {
        curTime = dayDuration;
    }
    void EndNight() {
        curTime = nightDuration;
    }

    public void IncreaseCurTime(float n) { curTime += n; }

    public void WonGame() {
        EventManager.Invoke(gameObject, EventID.WonGame);
    }
    
    void LostGame() {
        EventManager.Invoke(gameObject, EventID.LostGame);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
    
    // Usage in inspector by time buttons
    public void SetTimeSpeed(int n) {
        if (n >= 0) {
            _timeScale = n;
        }
    }
    
    /**********************   Mana Funcs   *********************/
    
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

    /**********************   Money Funcs   *********************/

    public bool ModifyMoney(int value) {
        int newMoney = _money + value;
        if (newMoney < 0) {
            return false;
        }

        _money = newMoney;
        EventManager.Invoke(gameObject, EventID.ModifyMoney, new DeltaArgs {newValue = _money, deltaValue = value});

        return true;
    }
}

public struct ManaArgs {
    public int curMana;
    public int maxMana;
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour {
    public static WaveManager Instance => _instance;
    static WaveManager _instance;

    public List<SO_Battle> battleDatas;
    public static List<SO_Battle> _battleDatas = new List<SO_Battle>();

    public int curWaveNum;
    public Battle battle;

    [SerializeField] List<CombatSlot> spawnSlots = new List<CombatSlot>();
    Queue<SO_Animal> spawnQueue = new Queue<SO_Animal>();
    [SerializeField] List<CombatSlot> playSlots = new List<CombatSlot>();

    CombatTicker waveTicker;

    public bool InBattle => inBattle;
    bool inBattle;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        waveTicker = new CombatTicker(gameObject, EventID.WaveTick, EventID.EndWave, 1, false);
        waveTicker.Pause();
        EventManager.Subscribe(gameObject, EventID.EndWave, NextWave);

        EventManager.Subscribe(gameObject, EventID.EnemyDied, DoCheckAllEnemiesDead);
        EventManager.Subscribe(GameManager.Instance.gameObject, EventID.Death, LostBattle);
        
        EventManager.Subscribe(GameManager.Instance.gameObject, EventID.EndDay, StartBattle);
        
        LoadBattles();
        
        // Debug
        // StartBattle();
    }
    
    void LoadBattles() {
        string[] assetPaths = AssetDatabase.FindAssets($"t:{typeof(SO_Battle).Name}", new string[] { Constants.BattlesDataPath });

        _battleDatas.Clear();
        
        // Load inspector created assets
        foreach (SO_Battle battleData in battleDatas) {
            _battleDatas.Add(battleData);
        }
        
        // Load file Assets
        for (int i = 0; i < assetPaths.Length; i++) {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
            SO_Battle battleData = AssetDatabase.LoadAssetAtPath<SO_Battle>(assetPath);

            _battleDatas.Add(battleData);
        }
    }

    public void StartBattle() {
        // Load battle data for current day
        if (GameManager.Instance.day <= _battleDatas.Count) {
            battle.waves = _battleDatas[GameManager.Instance.day - 1].waves;
        } else {
            Debug.Log("You Win!?");
            return;
        }

        inBattle = true;
        foreach (CombatSlot slot in playSlots) {
            slot.canPlace = true;
        }

        EventManager.Invoke(gameObject, EventID.StartBattle);

        NextWave();
        StartCoroutine(SpawnLoop());
    }

    public void WonBattle() {
        GameManager.Instance.ModifyMoney(battle.CalculateReward());
        
        CleanUp();
        EventManager.Invoke(gameObject, EventID.WonBattle);
    }
    public void LostBattle() {
        CleanUp();
        EventManager.Invoke(gameObject, EventID.LostBattle);
    }
    void CleanUp() {
        inBattle = false;
        foreach (CombatSlot slot in playSlots) {
            slot.canPlace = false;
        }

        battle.waves = null;
        curWaveNum = 0;
        spawnQueue.Clear();

        // Gather all player animals on combat grid into one stack
        if (spawnSlots.Count > 0) {
            ExecutionManager.Instance.ExecuteReturnEffect(spawnSlots[0].SlotGrid.slotGrid.Cast<CombatSlot>().ToList());
        } else { 
            Debug.LogError("No spawn slots registered");
            return;
        }
    }

    // NextWave increments curWaveNum and starts the next wave. Called from waveTicker end.
    void NextWave() {
        if (curWaveNum < battle.waves.Count) {
            curWaveNum += 1;
            StartWave(battle.waves[curWaveNum - 1]);
            
            EventManager.Invoke(gameObject, EventID.StartWave, new WaveArgs {
                curWave = battle.waves[curWaveNum - 1], 
                wavesLeft = battle.waves.Count - curWaveNum
            });
        }
    }

    void StartWave(Wave wave) {
        // Queue enemies to spawn
        foreach (SO_Animal aSO in wave.enemies) {
            spawnQueue.Enqueue(aSO);
        }

        // Setup waveTicker
        if (wave.tickDuration > 0) {
            waveTicker.Reset();
            waveTicker.SetEndTick(wave.tickDuration);
            waveTicker.Start();
        }
    }

    IEnumerator SpawnLoop() {
        while (inBattle) {
            if (spawnQueue.Count > 0) {
                CombatSlot spawnSlot = spawnSlots[Random.Range(0, spawnSlots.Count)];
                if (spawnSlot.IsEmpty()) {
                    spawnSlot.SpawnCard(spawnQueue.Dequeue());
                }
            }

            yield return null;
        }
    }

    void DoCheckAllEnemiesDead() { StartCoroutine(CheckAllEnemiesDead()); }
    IEnumerator CheckAllEnemiesDead() {
        // Killed all enemies in last wave
        yield return null; // wait for Animal to deregister from GameManager
        if (inBattle && GameManager.Instance.enemies.Count == 0 && curWaveNum == battle.waves.Count) {
            WonBattle();
        }
    }
}

[Serializable]
public class Battle {
    public List<Wave> waves;

    public int CalculateReward() {
        int total = 0;
        foreach (Wave wave in waves) {
            foreach (SO_Animal aSO in wave.enemies) {
                total += aSO.value;
            }
        }

        return total;
    }
}

[Serializable]
public struct Wave {
    public int tickDuration;
    public List<SO_Animal> enemies;
}

public struct WaveArgs {
    public int wavesLeft;
    public Wave curWave;
}


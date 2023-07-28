using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour {
    public static WaveManager Instance => _instance;
    static WaveManager _instance;

    public SO_Battle battleData;

    public int curWaveNum;
    public Battle battle;

    [SerializeField] List<CombatSlot> spawnSlots = new List<CombatSlot>();
    Queue<SO_Animal> spawnQueue = new Queue<SO_Animal>();
    [SerializeField] List<CombatSlot> playSlots = new List<CombatSlot>();

    public Transform cleanUpDepositPoint;

    CombatTicker waveTicker;
    bool inBattle;

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        battle.waves = battleData.waves;

        waveTicker = new CombatTicker(gameObject, EventID.WaveTick, EventID.EndWave, 10, false);
        waveTicker.Pause();
        EventManager.Subscribe(gameObject, EventID.EndWave, NextWave);

        EventManager.Subscribe(gameObject, EventID.EnemyDied, DoCheckAllEnemiesDead);
        EventManager.Subscribe(GameManager.Instance.gameObject, EventID.Death, LostBattle);
        
        // Debug
        StartBattle();
    }

    public void StartBattle() {
        inBattle = true;
        foreach (CombatSlot slot in playSlots) {
            slot.canPlace = true;
        }

        EventManager.Invoke(gameObject, EventID.StartBattle);

        NextWave();
        StartCoroutine(SpawnLoop());
    }

    public void WonBattle() {
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

        if (spawnSlots.Count == 0) {
            Debug.LogError("No spawn slots registered");
            return;
        }

        // Gather all player animals on combat grid into one stack
        SlotGrid combatGrid = spawnSlots[0].SlotGrid;
        Stack gatheredStack = null;
        bool first = true;
        foreach (Slot slot in combatGrid.slotGrid) {
            Card card = slot.Card;
            if (card) {
                if (GameManager.Instance.animals.Contains(card)) {
                    if (first) {
                        gatheredStack = card.mStack;
                        slot.PickUp();
                        first = false;
                        continue;
                    }

                    card.mStack.PlaceAll(gatheredStack);
                    StartCoroutine(Utils.MoveCardToPoint(card, gatheredStack.CalculateStackPosition(card)));
                    slot.PickUp();
                } else {
                    Transform stackTransform = slot.PickUp(card);
                    Destroy(stackTransform.gameObject);
                }
            }
        }

        if (gatheredStack) {
            StartCoroutine(Utils.MoveStackToPoint(gatheredStack, cleanUpDepositPoint.position));
        }
    }

    // NextWave increments curWaveNum and starts the next wave. Called from waveTicker end.
    void NextWave() {
        if (curWaveNum < battle.waves.Count) {
            curWaveNum += 1;
            StartWave(battle.waves[curWaveNum - 1]);
        } else { // No more waves, last wave timer finished... battle won
            curWaveNum = 0;
            WonBattle();
        }
    }

    void StartWave(Wave wave) {
        // Queue enemies to spawn
        foreach (SO_Animal aSO in wave.enemies) {
            spawnQueue.Enqueue(aSO);
        }

        // Setup waveTicker
        waveTicker.Reset();
        waveTicker.SetEndTick(wave.tickDuration);
        waveTicker.Start();
    }

    IEnumerator SpawnLoop() {
        while (inBattle) {
            if (spawnQueue.Count > 0) {
                CombatSlot spawnSlot = spawnSlots[Random.Range(0, spawnSlots.Count)];
                if (spawnSlot.IsEmpty()) {
                    StartCoroutine(SpawnEnemy(spawnQueue.Dequeue(), spawnSlot));
                }
            }

            yield return null;
        }
    }

    IEnumerator SpawnEnemy(SO_Animal aSO, Slot spawnSlot) {
        Stack s = CardFactory.CreateEnemy(aSO);
        yield return null; // required for Animal to be fully setup/events registered before slot placement
        spawnSlot.PlaceAndMove(s);
    }

    void DoCheckAllEnemiesDead() { StartCoroutine(CheckAllEnemiesDead()); }
    IEnumerator CheckAllEnemiesDead() {
        // Killed all enemies in last wave
        yield return null; // wait for Animal to deregister from GameManger
        if (inBattle && GameManager.Instance.enemies.Count == 0 && curWaveNum == battle.waves.Count) {
            WonBattle();
        }
    }
}

[Serializable]
public struct Battle {
    public List<Wave> waves;
}

[Serializable]
public struct Wave {
    public List<SO_Animal> enemies;
    public int tickDuration;
}
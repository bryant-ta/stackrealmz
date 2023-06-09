using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour {
    public int curWaveNum;
    public Battle battle;
    
    public List<CombatSlot> spawnSlots = new List<CombatSlot>();
    Queue<SO_Animal> spawnQueue = new Queue<SO_Animal>();

    CombatTicker waveTicker;
    bool inCombat;

    void Start() {
        waveTicker = new CombatTicker(gameObject, EventID.WaveTick, EventID.EndWave, 10, false);
        waveTicker.Pause();
        EventManager.Subscribe(gameObject, EventID.EndWave, NextWave);
    }

    public void StartBattle() {
        StartWave(battle.waves[curWaveNum]);
        inCombat = true;
        EventManager.Invoke(gameObject, EventID.StartBattle);

        StartCoroutine(SpawnLoop());
    }

    public void EndBattle() {
        inCombat = false;
        EventManager.Invoke(gameObject, EventID.EndBattle);
    }

    // NextWave increments curWaveNum and starts the next wave. Called from waveTicker end.
    void NextWave() {
        if (curWaveNum < battle.waves.Count - 1) {
            curWaveNum += 1;
            StartWave(battle.waves[curWaveNum]);
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
        while (inCombat) {
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
        yield return null;      // required for Animal to be fully setup/events registered before slot placement
        spawnSlot.PlaceAndMove(s);
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

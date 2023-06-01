using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    [SerializeField] public Stat atkDmg;
    [SerializeField] public Stat atkSpd;
    [SerializeField] public Stat ablPwr;
    [SerializeField] public Stat ablCd;

    IAttack attack;
    CombatTicker attackTicker;
    IAbility ability;
    CombatTicker abilityTicker;

    void Start() {
        Setup(animalData.name, animalData.value, animalData.image);
        atkDmg = new Stat(animalData.atkDmg);
        atkSpd = new Stat(animalData.atkSpd);
        ablCd = new Stat(animalData.ablCd);
        ablPwr = new Stat(animalData.ablPwr);

        attack = AttackTypeLookUp.LookUp[animalData.atkType];
        ability = AbilityTypeLookUp.LookUp[animalData.abilityType];
        
        // Listeners
        EventManager.Subscribe(gameObject, EventID.Death, Death);
        
        // Tickers
        attackTicker = new CombatTicker(gameObject, EventID.AttackTick, EventID.AttackReady, atkSpd.Value);
        attackTicker.Pause();   // paused until put into combat slot
        EventManager.Subscribe(gameObject, EventID.AttackReady, Attack);

        abilityTicker = new CombatTicker(gameObject, EventID.AbilityTick, EventID.AbilityReady, ablCd.Value, false);
        abilityTicker.Pause();   // paused until put into combat slot
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, Ability);
    }

    void Attack() {
        int terrainModifier = 1;
        if (mSlot.terrain == animalData.terrainPref) { terrainModifier = 2; }   // TEMP: temp value

        int dmg = atkDmg.Value * terrainModifier;
        
        print("attacking with power " + dmg);
        attack.Attack(mSlot, dmg);
    }

    public void Ability() {
        if (mSlot && abilityTicker.Ready()) {
            abilityTicker.Reset();
            ability.Use(mSlot, ablPwr.Value);
        }
    }

    void Death() {
        print("Ahhh I ded");
        mStack.Extract(this);
        // Destroy(gameObject);
    }
    
    public void StartCombatState() {
        attackTicker.Start();
        abilityTicker.Start();
        EventManager.Invoke(gameObject, EventID.EnterCombat);
    }
    public void EndCombatState() {
        attackTicker.Stop();
        abilityTicker.Stop();
        EventManager.Invoke(gameObject, EventID.ExitCombat);
    }

    void OnEnable() { GameManager.Instance.animals.Add(this); }
    void OnDisable() {
        GameManager.Instance.animals.Remove(this);
        attackTicker.Stop();
        abilityTicker.Stop();
    }

    // Old but keeping for solution ideas:
    //
    // This seems silly... having to make a func for every ticker just to trigger in EventManager
    // consider alternatives like using string instead of enum for EventManager or letting AnimalUI ref Animal always
    // and public CombatTickers (but this means no EventManager exposing? maybe not good bc other things might want to know
    // when this attacks...)
    // void AttackTick(int tick) {
    //     EventManager.TriggerEvent(gameObject, EventName.OnAttackTick, tick);
    // }
}
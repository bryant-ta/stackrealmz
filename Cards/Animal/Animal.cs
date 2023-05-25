using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    public int atkDmg;
    public int atkSpd;
    public int consumption;

    IAttack attack;
    CombatTicker attackTicker;
    IAbility ability;

    void Start() {
        Setup(animalData.name, animalData.value, animalData.image);
        atkDmg = animalData.atkDmg;
        atkSpd = animalData.atkSpd;
        consumption = animalData.consumption;

        attack = AttackTypeLookUp.LookUp[animalData.atkType];
        ability = AbilityTypeLookUp.LookUp[animalData.abilityType];
        
        // Listeners
        EventManager.AddListener(gameObject, EventName.OnDeath, Death);
        
        // Tickers
        attackTicker = new CombatTicker(gameObject, EventName.OnAttackTick, EventName.OnAttack, atkSpd);
        attackTicker.Pause();   // paused until put into combat slot
        EventManager.AddListener(gameObject, EventName.OnAttack, Attack);
    }

    // TODO: This seems silly... having to make a func for every ticker just to trigger in EventManager
    // consider alternatives like using string instead of enum for EventManager or letting AnimalUI ref Animal always
    // and public CombatTickers (but this means no EventManager exposing? maybe not good bc other things might want to know
    // when this attacks...)
    // void AttackTick(int tick) {
    //     EventManager.TriggerEvent(gameObject, EventName.OnAttackTick, tick);
    // }

    void Attack() {
        print("attacking");
        attack.Attack(mSlot, atkDmg);
    }

    public void Ability() {
        if (mSlot) {
            ability.Use(mSlot, 1);  // TODO: testing value 
        }
    }

    void Death() {
        print("Ahhh I ded");
        mStack.Extract(this);
        // Destroy(gameObject);
    }
    
    public void StartCombatState() {
        attackTicker.Start();
    }
    public void EndCombatState() {
        attackTicker.Reset();
        attackTicker.Pause();
    }

    void OnEnable() { GameManager.Instance.animals.Add(this); }
    void OnDisable() { GameManager.Instance.animals.Remove(this); }
}
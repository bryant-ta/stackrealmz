using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    public int atkDmg;
    public float atkSpd;
    public int consumption;

    void Start() {
        Setup(animalData.name, animalData.value, animalData.image);
        atkDmg = animalData.atkDmg;
        atkSpd = animalData.atkSpd;
        consumption = animalData.consumption;

        GetComponent<Health>().onDeath.AddListener(Death);
    }

    public void StartAttack() {
        StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop() {
        float timer = 0f;
        while (mSlot) {     // keep trying attack while in slot (prob need to change this)
            if (timer > atkSpd) {
                Attack();
                timer = 0;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    void Attack() {
        Slot targetSlot = mSlot.Forward;
        
        if (targetSlot) {
            targetSlot.Card.GetComponent<Health>().DoDamage(atkDmg);
        }
    }

    void Death() {
        print("Ahhh I ded");
        mStack.Extract(this);
        // Destroy(gameObject);
    }

    void OnEnable() { GameManager.Instance.animals.Add(this); }

    void OnDisable() { GameManager.Instance.animals.Remove(this); }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    public int consumption;

    void Start() {
        Setup(animalData.name, animalData.value, animalData.image);
        consumption = animalData.consumption;
        
        GetComponent<Health>().onDeath.AddListener(Death);
    }

    void Attack() {
        // TODO: figure out board structure code first
    }

    void Death() {
        print("Ahhh I ded");
        mStack.Extract(this);
        // Destroy(gameObject);
    }
    
    void OnEnable() { GameManager.Instance.animals.Add(this); }

    void OnDisable() {
        GameManager.Instance.animals.Remove(this);
    }
}

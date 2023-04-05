using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Card {
    public SO_Villager villagerData;

    public int consumption;
    
    void Start() {
        Setup(villagerData.name, villagerData.value, villagerData.image);
        consumption = villagerData.consumption;
    }
    
    void OnEnable() { GameManager.Instance.villagers.Add(this); }
    void OnDisable() { GameManager.Instance.villagers.Remove(this); }
}

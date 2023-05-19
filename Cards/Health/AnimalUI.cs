using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimalUI : MonoBehaviour {
    public Animal mAnimal;
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;

    void Start() {
        if (mAnimal != null) {
            nameText.text = mAnimal.animalData.name;
            hpText.text = mAnimal.animalData.hp.ToString();
            attackText.text = mAnimal.animalData.atkDmg.ToString();
        }
    }

    public void UpdateHPText(int n) {
        hpText.text = n.ToString();
    }
    
    public void UpdateAttackText(int n) {
        attackText.text = n.ToString();
    }
}

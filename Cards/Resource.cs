using System;
using UnityEngine;

public class Resource : Card
{
    public SO_Card cardData;
    
    void Start()
    {
        Setup(cardData.name, cardData.value, cardData.image);
    }
    
    void OnEnable() {
        GameManager.Instance.cards.Add(this);
    }
    void OnDisable() {
        GameManager.Instance.cards.Remove(this);
    }
}

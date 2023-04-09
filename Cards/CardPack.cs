using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardPack : Card {
    public SO_CardPack cardPackData;

    public int numCards;
    public List<Drop> dropTable;

    public void Start() {
        Setup(cardPackData.name, cardPackData.value, cardPackData.image);
        numCards = cardPackData.numCards;
        dropTable = cardPackData.dropTable;
    }

    // Open card pack, rolling and creating all card drops at once
    public void Open() {
        for (int i = 0; i < numCards; i++) {
            SO_Card cSO = CardFactory.RollDrop(dropTable);
            if (cSO == null) {
                continue;
            }
            
            GameObject cardObj = CardFactory.CreateCard(cSO);
            cardObj.transform.position = Utils.GenerateCircleVector(i, numCards, Constants.CardCreationRadius, transform.position);
        }
        
        Destroy(gameObject);
    }
}
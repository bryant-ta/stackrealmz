using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardPack : Card {
    public SO_CardPack cardPackData;

    public int numCards;
    public List<Drop> lootTable;

    public void Start() {
        Setup(cardPackData.name, cardPackData.value, cardPackData.image);
        numCards = cardPackData.numCards;
        lootTable = cardPackData.lootTable;
    }

    // Open card pack, rolling and creating all card drops at once
    public void Open() {
        for (int i = 0; i < numCards; i++) {
            GameObject cardObj = CardFactory.CreateCard(RollDrop());

            // Generate evenly spaced 2D vectors around object center
            float radius = 2f;
            float angle = i * Mathf.PI * 2f / numCards;
            cardObj.transform.position = transform.position + (new Vector3(Mathf.Cos(angle) * radius, transform.position.y, Mathf.Sin(angle) * radius));
        }
        
        Destroy(gameObject);
    }

    SO_Card RollDrop() {
        // Roll eligible drops
        int roll = Random.Range(1, 101);
        List<Drop> possibleDrops = new List<Drop>();
        foreach (Drop drop in lootTable) {
            if (roll <= drop.percentage) {
                possibleDrops.Add(drop);
            }
        }

        // Choose most rare drop
        if (possibleDrops.Count > 0) {
            Drop ret = possibleDrops[0];
            List<Drop> tiedDrops = new List<Drop>();
            for (int i = 1; i < possibleDrops.Count; i++) {
                if (possibleDrops[i].percentage == ret.percentage) {
                    tiedDrops.Add(possibleDrops[i]);
                }

                if (possibleDrops[i].percentage < ret.percentage) {
                    ret = possibleDrops[i];
                    tiedDrops.Clear();
                    tiedDrops.Add(ret);
                }
            }

            // Randomly choose drops with tied drop chance
            if (tiedDrops.Count > 1) {
                ret = tiedDrops[Random.Range(0, tiedDrops.Count)];
            }

            return ret.cSO;
        }

        // Did not roll any drops
        return null;
    }

    public void Create(GameObject obj, Vector3 location, int n) {
        for (int i = 0; i < n; i++) {
            float radius = n;
            float angle = i * Mathf.PI * 2f / radius;
            Vector3 newPos = transform.position +
                             (new Vector3(Mathf.Cos(angle) * radius, -2, Mathf.Sin(angle) * radius));
            Instantiate(obj, newPos, Quaternion.Euler(0, 0, 0));
        }
    }
}
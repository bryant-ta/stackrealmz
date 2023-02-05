using System.Collections.Generic;
using UnityEngine;

public class CardFactory : MonoBehaviour {
    [SerializeField] GameObject baseCard;

    Dictionary<string, SO_Card> cardDic;

    public GameObject CreateCard(string name, Vector3 spawnPos) {
        SO_Card c = cardDic["name"];

        foreach ((string key, SO_Card value) in cardDic) {
            
        }
        
        
        if (c )adsfasdfdsaf
        
        GameObject o = Instantiate(baseCard, spawnPos, Quaternion.identity);
        return o;
    }
}

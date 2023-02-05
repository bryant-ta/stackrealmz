using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// GameManager singleton handles day cycle
public class GameManager : MonoBehaviour {
    public static GameManager Instance {
        get {
            if (_instance == null) {
                Instance = FindObjectOfType<GameManager>().GetComponent<GameManager>();
            }

            return _instance;
        }
        private set => _instance = value;
    }
    static GameManager _instance;
    
    public List<Card> cards;
    public List<Card> foods;

    

    void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            _instance = this; 
        } 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameManager singleton handles day cycle
// Requires GameManager to have priority in script load order (otherwise _instance might not be set when used)
public class GameManager : MonoBehaviour {
    public static GameManager Instance => _instance;
    static GameManager _instance;

    public static Canvas WorldCanvas { get; private set; }
    [SerializeField] Canvas _worldCanvas;

    public List<Card> cards;
    public List<Card> foods;

    void Awake() 
    { 
        if (_instance != null && _instance != this) 
        { 
            Destroy(gameObject); 
        } 
        else 
        { 
            _instance = this; 
        }

        WorldCanvas = _worldCanvas;
    }

    IEnumerator DayCycle() {
        while (true) {
            // timer here
        }
    }
}

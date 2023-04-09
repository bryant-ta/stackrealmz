using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GameManager singleton handles day cycle
// Requires GameManager to have priority in script load order (otherwise _instance might not be set when used)
public class GameManager : MonoBehaviour {
    public static GameManager Instance => _instance;
    static GameManager _instance;

    public static Canvas WorldCanvas { get; private set; }
    [SerializeField] Canvas _worldCanvas;

    public List<Card> cards;
    public List<Food> foods;
    public List<Villager> villagers;

    // Day Vars
    public int dayDuration = 1;
    public Image dayProgressFill;
    
    // Time Vars
    public static int TimeSpeed = 1;
    
    //Debug
    public bool doDay;

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

        if (doDay) {
            StartCoroutine(DayCycle());
        }
    }
    
    IEnumerator DayCycle() {
        while (true) {
            while (dayProgressFill.fillAmount < 1) {
                dayProgressFill.fillAmount += (float)TimeSpeed / dayDuration * Time.deltaTime;
                yield return null;
            }

            dayProgressFill.fillAmount = 0f;
            
            foreach (Villager v in villagers) {
                if (foods.Count == 0) {
                    // TODO: You lost function
                    print("YOU LOST");
                    break;
                }

                foods[0].Eat();
            }
        }
    }

    public static void SetTimeSpeed(int n) {
        if (n >= 0) {
            TimeSpeed = n;
        }
    }
}

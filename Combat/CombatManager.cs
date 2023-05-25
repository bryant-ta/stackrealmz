using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour {
    [SerializeField] int realTimeTickDuration;
    
    public static UnityEvent onTick = new UnityEvent();

    void Start() {
        if (realTimeTickDuration == 0) { realTimeTickDuration = 5; }
    }

    float timer;
    void Update() {
        if (timer >= realTimeTickDuration) {
            timer = 0;
            onTick.Invoke();
        }

        timer += Time.deltaTime;
    }
}

using UnityEngine;
using UnityEngine.Events;

public class CombatClock : MonoBehaviour {
    [SerializeField] int realTimeTickDuration = 1;
    
    public static UnityEvent onTick;

    void Awake() {
        onTick = new UnityEvent();
        if (realTimeTickDuration == 0) { realTimeTickDuration = 5; }    // default value
    }

    float timer;
    void Update() {
        if (timer >= realTimeTickDuration) {
            timer = 0;
            onTick.Invoke();
        }

        timer += GameManager.Instance.TimeScale * Time.deltaTime;
    }
}

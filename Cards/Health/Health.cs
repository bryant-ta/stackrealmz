using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
    public int hp;

    public UnityEvent<int> onSetHP;
    public UnityEvent<int> onDamage;
    public UnityEvent onDeath;

    void Start() {
        if (TryGetComponent(out Animal a)) {
            SetHP(a.animalData.hp);
        }
    }

    public void SetHP(int n) {
        hp = n;
        onSetHP.Invoke(n);
    }

    public void DoDamage(int n) {
        SetHP(hp - n);
        onDamage.Invoke(n);
        if (hp <= 0) {
            onDeath.Invoke();
        }
    }
}

using UnityEngine;

public class Health : MonoBehaviour {
    public int hp;
    public int maxHp;

    void Start() {
        if (TryGetComponent(out Animal a)) {
            SetMaxHp(a.animalData.hp);
            SetHp(a.animalData.hp);
        } else {
            SetMaxHp(maxHp);
            SetHp(hp);
        }
    }

    public void Heal(int n) {
        int newHp = hp + n;
        if (newHp > maxHp) {
            newHp = maxHp;
        }
        EventManager.Invoke(gameObject, EventID.Heal, n);
        SetHp(newHp);
    }

    public void Damage(int n) {
        int newHp = hp - n;
        if (newHp <= 0) {
            EventManager.Invoke(gameObject, EventID.Death);
        }
        EventManager.Invoke(gameObject, EventID.Damage, n);
        SetHp(newHp);
    }

    public void SetHp(int n) {
        hp = n;
        EventManager.Invoke(gameObject, EventID.SetHp, n);
    }
    
    public void SetMaxHp(int n) {
        maxHp = n;
        EventManager.Invoke(gameObject, EventID.SetMaxHp, n);
    }
}

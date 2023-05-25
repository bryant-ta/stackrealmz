using UnityEngine;

public class Health : MonoBehaviour {
    public int maxHp;
    public int hp;

    void Start() {
        if (TryGetComponent(out Animal a)) {
            SetMaxHp(a.animalData.hp);
            SetHp(a.animalData.hp);
        }
    }

    public void Heal(int n) {
        int newHp = hp + n;
        if (newHp > maxHp) {
            newHp = maxHp;
        }
        EventManager.TriggerEvent(gameObject, EventName.OnHeal, n);
        SetHp(newHp);
    }

    public void Damage(int n) {
        int newHp = hp - n;
        if (newHp <= 0) {
            EventManager.TriggerEvent(gameObject, EventName.OnDeath);
        }
        EventManager.TriggerEvent(gameObject, EventName.OnDamage, n);
        SetHp(newHp);
    }

    public void SetHp(int n) {
        hp = n;
        EventManager.TriggerEvent(gameObject, EventName.OnSetHp, n);
    }
    
    public void SetMaxHp(int n) {
        maxHp = n;
        EventManager.TriggerEvent(gameObject, EventName.OnSetMaxHp, n);
    }
}

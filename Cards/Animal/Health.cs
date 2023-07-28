using UnityEngine;

public class Health : MonoBehaviour {
    public int hp;
    public int maxHp;
    public int armor;

    void Start() {
        if (TryGetComponent(out Animal a)) {
            SetMaxHp(a.animalData.hp);
        } else {
            SetMaxHp(maxHp);
        }
    }

    public void Damage(int value) {
        if (armor > 0 && value > 0) {
            ModifyArmor(-1);
        } else {
            ModifyHp(-value);
        }
    }
    
    // Positive input heals. Negative input damages.
    public void ModifyHp(int value) {
        int newHp = hp + value;
        if (newHp <= 0) {
            hp = 0;
        } else if (newHp > maxHp) {
            hp = maxHp;
        } else {
            hp = newHp;
        }
        
        if (value < 0) EventManager.Invoke(gameObject, EventID.Damage, hp);
        else if (value > 0) EventManager.Invoke(gameObject, EventID.Heal, hp);
        
        EventManager.Invoke(gameObject, EventID.SetHp, hp);
        if (hp <= 0) EventManager.Invoke(gameObject, EventID.Death);
    }

    public void ModifyMaxHp(int value) {
        int newMaxHp = maxHp + value;
        if (newMaxHp < 1) {
            maxHp = 1;
            SetHp(1);
        } else {
            maxHp = newMaxHp;
            ModifyHp(value);
        }
        
        EventManager.Invoke(gameObject, EventID.SetMaxHp, maxHp);
    }

    public void SetHp(int value) {
        hp = value;
        EventManager.Invoke(gameObject, EventID.SetHp, value);
        if (hp <= 0) EventManager.Invoke(gameObject, EventID.Death);
    }
    
    public void SetMaxHp(int value) {
        maxHp = value;
        ModifyHp(value);
        EventManager.Invoke(gameObject, EventID.SetMaxHp, value);
    }
    
    // Positive input raises armor. Negative input reduces armor.
    public void ModifyArmor(int value) {
        int newArmor = armor + value;
        if (newArmor <= 0) {
            armor = 0;
        } else {
            armor = newArmor;
        }
        
        EventManager.Invoke(gameObject, EventID.SetArmor, armor);
    }
}

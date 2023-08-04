using UnityEngine;

public class Health : MonoBehaviour {
    public int hp;
    public int maxHp;
    public int armor;
    public int poison;

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
            newHp = 0;
        } else if (newHp > maxHp) {
            newHp = maxHp;
        }
        
        if (value < 0) EventManager.Invoke(gameObject, EventID.Damage, newHp);
        else if (value > 0) EventManager.Invoke(gameObject, EventID.Heal, newHp);
        
        SetHp(newHp);
    }

    public void ModifyMaxHp(int value) {
        int newMaxHp = maxHp + value;
        if (newMaxHp < 1) {
            newMaxHp = 1;
        }
        
        SetMaxHp(newMaxHp);
    }
    
    public void SetHp(int value) {
        hp = value;
        
        EventManager.Invoke(gameObject, EventID.SetHp, value);
        if (hp <= 0) EventManager.Invoke(gameObject, EventID.Death);    // only invoke Death here
    }
    
    public void SetMaxHp(int value) {
        maxHp = value;
        if (value > 0) {
            ModifyHp(value);
        }

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
    
    // Positive input raises poison. Negative input reduces poison.
    public void ModifyPoison(int value) {
        int newPoison = poison + value;
        if (newPoison <= 0) {
            poison = 0;
        } else if (newPoison >= hp) {
            poison = hp;
            SetHp(0);
        } else {
            poison = newPoison;
        }
        
        EventManager.Invoke(gameObject, EventID.SetPoison, poison);
    }
}

using System.Collections.Generic;

// See AttackTypes.cs for how to use.
public enum EffectType {
    DamageEffect,
    ModifyAttackEffect,
    ModifyBaseAttackEffect,
}

public static class EffectTypeLookUp {
    public static Dictionary<EffectType, IEffect> LookUp = new Dictionary<EffectType, IEffect>() {
        {EffectType.DamageEffect, new DamageEffect()},
        {EffectType.ModifyAttackEffect, new ModifyAttackEffect()},
        {EffectType.ModifyBaseAttackEffect, new ModifyBaseAttackEffect()},
    };
}

/********************************************/

public interface IEffect {
    public void Execute(Animal animal, int val);
    public void Remove(Animal animal);
}

public class DamageEffect : IEffect {
    public void Execute(Animal animal, int val) {
        animal.GetComponent<Health>().Damage(val);
    }
    
    public void Remove(Animal animal) {
        return;
    }
}

public class ModifyAttackEffect : IEffect {
    int modifierTotal;
    
    public void Execute(Animal animal, int val) {
        animal.atkDmg.ChangeModifier(val);
        modifierTotal += val;
        EventManager.Invoke(animal.gameObject, EventID.SetAttack, animal.atkDmg.Value);
    }
    
    public void Remove(Animal animal) {
        animal.atkDmg.ChangeModifier(-modifierTotal);
        EventManager.Invoke(animal.gameObject, EventID.SetAttack, animal.atkDmg.Value);
    }
}

public class ModifyBaseAttackEffect : IEffect {
    int modifierTotal;
    
    public void Execute(Animal animal, int val) {
        animal.atkDmg.ChangeBaseValue(val);
        modifierTotal += val;
        EventManager.Invoke(animal.gameObject, EventID.SetAttack, animal.atkDmg.Value);
    }
    
    public void Remove(Animal animal) {
        animal.atkDmg.ChangeBaseValue(-modifierTotal);
        EventManager.Invoke(animal.gameObject, EventID.SetAttack, animal.atkDmg.Value);
    }
}
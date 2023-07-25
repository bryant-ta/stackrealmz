using System.Collections.Generic;

// See AttackTypes.cs for how to use.
public enum EffectType {
    None                   = 0,
    DamageEffect           = 1,
    HealEffect             = 2,
    ModifyMaxHealthEffect  = 3,
    ModifyManaEffect       = 4,
    ModifyBaseManaEffect   = 5,
    ModifyAttackEffect     = 6,
    ModifyBaseAttackEffect = 7,
    ModifySpeedEffect      = 8,
    ModifyBaseSpeedEffect  = 9,
    
}

public static class EffectTypeLookUp {
    public static Dictionary<EffectType, IEffect> LookUp = new Dictionary<EffectType, IEffect>() {
        {EffectType.None, null},
        {EffectType.DamageEffect, new DamageEffect()},
        {EffectType.HealEffect, new HealEffect()},
        {EffectType.ModifyMaxHealthEffect, new ModifyMaxHealthEffect()},
        {EffectType.ModifyManaEffect, new ModifyManaEffect()},
        {EffectType.ModifyBaseManaEffect, new ModifyBaseManaEffect()},
        {EffectType.ModifyAttackEffect, new ModifyAttackEffect()},
        {EffectType.ModifyBaseAttackEffect, new ModifyBaseAttackEffect()},
        {EffectType.ModifySpeedEffect, new ModifySpeedEffect()},
        {EffectType.ModifyBaseSpeedEffect, new ModifyBaseSpeedEffect()},
    };
}

/********************************************/

public interface IEffect {
    public void Execute(Animal animal, int val);
    public void Remove(Animal animal);
}

public class DamageEffect : IEffect {
    public void Execute(Animal animal, int val) {
        animal.health.ModifyHp(-val); // invokes event
    }

    public void Remove(Animal animal) { return; }
}
public class HealEffect : IEffect {
    public void Execute(Animal animal, int val) {
        animal.health.ModifyHp(val); // invokes event
    }

    public void Remove(Animal animal) {
        return;
        // animal.health.ModifyHp(-modifierTotal);
        // EventManager.Invoke(animal.gameObject, EventID.Heal, animal.health.hp);
    }
}
public class ModifyMaxHealthEffect : IEffect {
    int modifierTotal;
    public void Execute(Animal animal, int val) { animal.health.ModifyMaxHp(val); }
    public void Remove(Animal animal) { return; }
}
public class ModifyManaEffect : IEffect {
    int modifierTotal;

    public void Execute(Animal animal, int val) {
        animal.manaCost.ChangeModifier(val);
        modifierTotal += val;
        EventManager.Invoke(animal.gameObject, EventID.SetManaCost, animal.manaCost.Value);
    }

    public void Remove(Animal animal) {
        animal.manaCost.ChangeModifier(-modifierTotal);
        EventManager.Invoke(animal.gameObject, EventID.SetManaCost, animal.manaCost.Value);
    }
}
public class ModifyBaseManaEffect : IEffect {
    int modifierTotal;

    public void Execute(Animal animal, int val) {
        animal.manaCost.ChangeBaseValue(val);
        modifierTotal += val;
        EventManager.Invoke(animal.gameObject, EventID.SetManaCost, animal.manaCost.Value);
    }

    public void Remove(Animal animal) {
        animal.manaCost.ChangeBaseValue(-modifierTotal);
        EventManager.Invoke(animal.gameObject, EventID.SetManaCost, animal.manaCost.Value);
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
public class ModifySpeedEffect : IEffect {
    int modifierTotal;

    public void Execute(Animal animal, int val) {
        animal.speed.ChangeModifier(val);
        modifierTotal += val;
        EventManager.Invoke(animal.gameObject, EventID.SetSpeed, animal.speed.Value);
    }

    public void Remove(Animal animal) {
        animal.speed.ChangeModifier(-modifierTotal);
        EventManager.Invoke(animal.gameObject, EventID.SetSpeed, animal.speed.Value);
    }
}
public class ModifyBaseSpeedEffect : IEffect {
    int modifierTotal;

    public void Execute(Animal animal, int val) {
        animal.speed.ChangeBaseValue(val);
        modifierTotal += val;
        EventManager.Invoke(animal.gameObject, EventID.SetSpeed, animal.speed.Value);
    }

    public void Remove(Animal animal) {
        animal.speed.ChangeBaseValue(-modifierTotal);
        EventManager.Invoke(animal.gameObject, EventID.SetSpeed, animal.speed.Value);
    }
}

public class SummonEffect : IEffect {
    int modifierTotal;

    public void Execute(Animal animal, int val) {
        animal.speed.ChangeBaseValue(val);
        modifierTotal += val;
        EventManager.Invoke(animal.gameObject, EventID.SetSpeed, animal.speed.Value);
    }

    public void Remove(Animal animal) {
        animal.speed.ChangeBaseValue(-modifierTotal);
        EventManager.Invoke(animal.gameObject, EventID.SetSpeed, animal.speed.Value);
    }
}
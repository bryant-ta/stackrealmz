using System;
using System.Collections.Generic;
using UnityEngine;

// See AttackTypes.cs for how to use.
public enum EffectType {
    None = 0,
    DamageEffect = 1,
    HealEffect = 2,
    ModifyMaxHealthEffect = 3,
    ModifyManaEffect = 4,
    ModifyBaseManaEffect = 5,
    ModifyAttackEffect = 6,
    ModifyBaseAttackEffect = 7,
    ModifySpeedEffect = 8,
    ModifyBaseSpeedEffect = 9,
    ModifyArmorEffect = 10,
    SummonEffect = 20, // executed in EffectManager
    SpikeyEffect = 21,
    PoisonEffect = 22,
    PushEffect = 40,   // use with Standard TargetType
    PullEffect = 41,   // use with Far TargetType
    // SwapEffect = 42, // cant do yet, needs two combatslot args for swapping
}

public static class EffectTypeLookUp
{
    public static Dictionary<EffectType, Func<IEffect>> LookUp = new Dictionary<EffectType, Func<IEffect>>()
    {
        { EffectType.None, () => null },
        { EffectType.DamageEffect, () => new DamageEffect() },
        { EffectType.HealEffect, () => new HealEffect() },
        { EffectType.ModifyMaxHealthEffect, () => new ModifyMaxHealthEffect() },
        { EffectType.ModifyManaEffect, () => new ModifyManaEffect() },
        { EffectType.ModifyBaseManaEffect, () => new ModifyBaseManaEffect() },
        { EffectType.ModifyAttackEffect, () => new ModifyAttackEffect() },
        { EffectType.ModifyBaseAttackEffect, () => new ModifyBaseAttackEffect() },
        { EffectType.ModifySpeedEffect, () => new ModifySpeedEffect() },
        { EffectType.ModifyBaseSpeedEffect, () => new ModifyBaseSpeedEffect() },
        { EffectType.ModifyArmorEffect, () => new ModifyArmorEffect() },
        { EffectType.SummonEffect, () => null },
        { EffectType.SpikeyEffect, () => new SpikeyEffect() },
        { EffectType.PoisonEffect, () => new PoisonEffect() },
        { EffectType.PushEffect, () => new PushEffect() },
        { EffectType.PullEffect, () => new PullEffect() },
    };

    public static IEffect CreateEffect(EffectType type)
    {
        return LookUp[type]?.Invoke();
    }
}


public struct EffectArgs {
    public int val;
}

/********************************************/

public interface IEffect {
    public void Apply(CombatSlot target, EffectArgs args);
    public void Remove(CombatSlot target);
}

public class DamageEffect : IEffect {
    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.health.Damage(args.val); // invokes event
    }

    public void Remove(CombatSlot target) { return; }
}
public class HealEffect : IEffect {
    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.health.ModifyHp(args.val); // invokes event
    }

    public void Remove(CombatSlot target) {
        return;
        // animal.health.ModifyHp(-modifierTotal);
        // EventManager.Invoke(animal.gameObject, EventID.Heal, animal.health.hp);
    }
}
public class ModifyMaxHealthEffect : IEffect {
    int modifierTotal;
    public void Apply(CombatSlot target, EffectArgs args) { target.Animal.health.ModifyMaxHp(args.val); }
    public void Remove(CombatSlot target) { return; }
}
public class ModifyManaEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.manaCost.ChangeModifier(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetManaCost, target.Animal.manaCost.Value);
    }

    public void Remove(CombatSlot target) {
        target.Animal.manaCost.ChangeModifier(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetManaCost, target.Animal.manaCost.Value);
    }
}
public class ModifyBaseManaEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.manaCost.ChangeBaseValue(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetManaCost, target.Animal.manaCost.Value);
    }

    public void Remove(CombatSlot target) {
        target.Animal.manaCost.ChangeBaseValue(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetManaCost, target.Animal.manaCost.Value);
    }
}
public class ModifyAttackEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.atkDmg.ChangeModifier(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetAttack, target.Animal.atkDmg.Value);
    }

    public void Remove(CombatSlot target) {
        target.Animal.atkDmg.ChangeModifier(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetAttack, target.Animal.atkDmg.Value);
    }
}
public class ModifyBaseAttackEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.atkDmg.ChangeBaseValue(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetAttack, target.Animal.atkDmg.Value);
    }

    public void Remove(CombatSlot target) {
        target.Animal.atkDmg.ChangeBaseValue(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetAttack, target.Animal.atkDmg.Value);
    }
}
public class ModifySpeedEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.speed.ChangeModifier(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetSpeed, target.Animal.speed.Value);
    }

    public void Remove(CombatSlot target) {
        target.Animal.speed.ChangeModifier(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetSpeed, target.Animal.speed.Value);
    }
}
public class ModifyBaseSpeedEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.speed.ChangeBaseValue(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetSpeed, target.Animal.speed.Value);
    }

    public void Remove(CombatSlot target) {
        target.Animal.speed.ChangeBaseValue(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(target.Animal.gameObject, EventID.SetSpeed, target.Animal.speed.Value);
    }
}

public class ModifyArmorEffect : IEffect {
    public void Apply(CombatSlot target, EffectArgs args) { target.Animal.health.ModifyArmor(args.val); }

    public void Remove(CombatSlot target) { return; }
}

// public class SummonEffect : IEffect {
//     public void Execute(CombatSlot slot, EffectArgs args) {
//     }
//
//     public void Remove(CombatSlot slot) {
//     }
// }

public class SpikeyEffect : IEffect {
    Animal mAnimal;
    public void Apply(CombatSlot target, EffectArgs args) {
        mAnimal = target.Animal;
        EventManager.Subscribe<int>(target.Animal.gameObject, EventID.Damage, Spikey);
    }

    public void Remove(CombatSlot target) { }

    void Spikey(int val) { EventManager.Invoke(mAnimal.gameObject, EventID.AttackReady); }
}

public class PoisonEffect : IEffect {
    public void Apply(CombatSlot target, EffectArgs args) {
        target.Animal.health.ModifyPoison(args.val);
    }

    public void Remove(CombatSlot target) {
        target.Animal.health.ModifyPoison(-target.Animal.health.poison);
    }
}

public class PushEffect : IEffect {
    public void Apply(CombatSlot target, EffectArgs args) { // slot = enemy to push
        if (!target.IsEmpty()) {
            CombatSlot pushToSlot = target.SlotGrid.SelectSlotRelative(target, target.Animal.isEnemy, new Vector2Int(-1, 0)) as CombatSlot;
            if (pushToSlot) {
                target.SwapWithCombatSlot(pushToSlot);
            }
        }
    }

    public void Remove(CombatSlot target) {
        return;
    }
}
public class PullEffect : IEffect {
    public void Apply(CombatSlot target, EffectArgs args) { // slot = enemy to pull
        if (!target.IsEmpty()) {
            CombatSlot pushToSlot = target.SlotGrid.SelectSlotRelative(target, target.Animal.isEnemy, new Vector2Int(1, 0)) as CombatSlot;
            if (pushToSlot) {
                target.SwapWithCombatSlot(pushToSlot);
            }
        }
    }

    public void Remove(CombatSlot target) { return; }
}



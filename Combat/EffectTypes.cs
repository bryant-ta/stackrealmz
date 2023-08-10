using System;
using System.Collections.Generic;
using UnityEngine;

// See AttackTypes.cs for how to use.
public enum EffectType {
    None             = 0,
    Damage           = 1,
    Heal             = 2,
    ModifyMaxHealth  = 3,
    ModifyMana       = 4,
    ModifyBaseMana   = 5,
    ModifyAttack     = 6,
    ModifyBaseAttack = 7,
    ModifySpeed      = 8,
    ModifyBaseSpeed  = 9,
    ModifyArmor      = 10,
    Return           = 11,
    Summon           = 20, // executed in ExecutionManager
    Spikey           = 21,
    Poison           = 22,
    Hidden           = 23,
    Vanish           = 24,
    Push             = 40, // use with Standard TargetType
    Pull             = 41, // use with Far TargetType
    Rooted           = 42,
    // SwapEffect = 42, // cant do yet, needs two combatslot args for swapping
}

public static class EffectTypeLookUp
{
    static Dictionary<EffectType, Func<IEffect>> LookUp = new Dictionary<EffectType, Func<IEffect>>()
    {
        { EffectType.None, () => null },
        { EffectType.Damage, () => new DamageEffect() },
        { EffectType.Heal, () => new HealEffect() },
        { EffectType.ModifyMaxHealth, () => new ModifyMaxHealthEffect() },
        { EffectType.ModifyMana, () => new ModifyManaEffect() },
        { EffectType.ModifyBaseMana, () => new ModifyBaseManaEffect() },
        { EffectType.ModifyAttack, () => new ModifyAttackEffect() },
        { EffectType.ModifyBaseAttack, () => new ModifyBaseAttackEffect() },
        { EffectType.ModifySpeed, () => new ModifySpeedEffect() },
        { EffectType.ModifyBaseSpeed, () => new ModifyBaseSpeedEffect() },
        { EffectType.ModifyArmor, () => new ModifyArmorEffect() },
        { EffectType.Return, () => null },
        { EffectType.Summon, () => null },
        { EffectType.Spikey, () => new SpikeyEffect() },
        { EffectType.Poison, () => new PoisonEffect() },
        { EffectType.Hidden, () => new HiddenEffect() },
        { EffectType.Vanish, () => new VanishEffect() },
        { EffectType.Push, () => new PushEffect() },
        { EffectType.Pull, () => new PullEffect() },
        { EffectType.Rooted, () => new RootedEffect() },
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
    public void Apply(CombatSlot targetSlot, EffectArgs args);
    public void Remove(CombatSlot targetSlot);
}

public class DamageEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.health.Damage(args.val); // invokes event
    }

    public void Remove(CombatSlot targetSlot) { return; }
}
public class HealEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.health.ModifyHp(args.val); // invokes event
    }

    public void Remove(CombatSlot targetSlot) {
        return;
        // animal.health.ModifyHp(-modifierTotal);
        // EventManager.Invoke(animal.gameObject, EventID.Heal, animal.health.hp);
    }
}
public class ModifyMaxHealthEffect : IEffect {
    int modifierTotal;
    public void Apply(CombatSlot targetSlot, EffectArgs args) { targetSlot.Animal.health.ModifyMaxHp(args.val); }
    public void Remove(CombatSlot targetSlot) { return; }
}
public class ModifyManaEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.manaCost.ChangeModifier(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetManaCost, targetSlot.Animal.manaCost.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.manaCost.ChangeModifier(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetManaCost, targetSlot.Animal.manaCost.Value);
    }
}
public class ModifyBaseManaEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.manaCost.ChangeBaseValue(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetManaCost, targetSlot.Animal.manaCost.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.manaCost.ChangeBaseValue(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetManaCost, targetSlot.Animal.manaCost.Value);
    }
}
public class ModifyAttackEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.atk.ChangeModifier(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetAttack, targetSlot.Animal.atk.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.atk.ChangeModifier(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetAttack, targetSlot.Animal.atk.Value);
    }
}
public class ModifyBaseAttackEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.atk.ChangeBaseValue(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetAttack, targetSlot.Animal.atk.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.atk.ChangeBaseValue(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetAttack, targetSlot.Animal.atk.Value);
    }
}
public class ModifySpeedEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.speed.ChangeModifier(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.speed.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.speed.ChangeModifier(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.speed.Value);
    }
}
public class ModifyBaseSpeedEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.speed.ChangeBaseValue(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.speed.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.speed.ChangeBaseValue(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.speed.Value);
    }
}

public class ModifyArmorEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) { targetSlot.Animal.health.ModifyArmor(args.val); }

    public void Remove(CombatSlot targetSlot) { return; }
}

// public class Summon : IEffect {
//     public void Execute(CombatSlot slot, EffectArgs args) {
//     }
//
//     public void Remove(CombatSlot slot) {
//     }
// }

public class SpikeyEffect : IEffect {
    Animal mAnimal;
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        mAnimal = targetSlot.Animal;
        EventManager.Subscribe<int>(targetSlot.Animal.gameObject, EventID.Damage, Spikey);
    }

    public void Remove(CombatSlot targetSlot) { }

    void Spikey(int val) { EventManager.Invoke(mAnimal.gameObject, EventID.AttackReady); }
}

public class PoisonEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.health.ModifyPoison(args.val);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.health.ModifyPoison(-targetSlot.Animal.health.poison);
    }
}

public class HiddenEffect : IEffect {
    CombatSlot mTargetSlot;
    
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        mTargetSlot = targetSlot;
        
        EventManager.Subscribe<int>(targetSlot.Animal.gameObject, EventID.Damage, DoRemove);
        EventManager.Subscribe(targetSlot.Animal.gameObject, EventID.Attack, DoRemoveFromAttack);
    }

    public void Remove(CombatSlot targetSlot) {
        Effect e = targetSlot.Animal.EffectCtrl.FindEffect(EffectType.Hidden);
        targetSlot.Animal.EffectCtrl.tempEffects.Remove(e);
    }

    void DoRemove(int n = 0) {
        if (n <= 0) return;
        Remove(mTargetSlot);
    }
    void DoRemoveFromAttack() {
        Remove(mTargetSlot);
    }
}

public class VanishEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) { return; }

    public void Remove(CombatSlot targetSlot) { return; }
}

public class PushEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) { // slot = enemy to push
        if (!targetSlot.IsEmpty()) {
            CombatSlot pushToSlot = targetSlot.SlotGrid.SelectSlotRelative(targetSlot, targetSlot.Animal.isEnemy, new Vector2Int(-1, 0)) as CombatSlot;
            if (pushToSlot) {
                targetSlot.SwapWithCombatSlot(pushToSlot);
            }
        }
    }

    public void Remove(CombatSlot targetSlot) {
        return;
    }
}
public class PullEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) { // slot = enemy to pull
        if (!targetSlot.IsEmpty()) {
            CombatSlot pushToSlot = targetSlot.SlotGrid.SelectSlotRelative(targetSlot, targetSlot.Animal.isEnemy, new Vector2Int(1, 0)) as CombatSlot;
            if (pushToSlot) {
                targetSlot.SwapWithCombatSlot(pushToSlot);
            }
        }
    }

    public void Remove(CombatSlot targetSlot) { return; }
}

public class RootedEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) { return; }

    public void Remove(CombatSlot targetSlot) { return; }
}



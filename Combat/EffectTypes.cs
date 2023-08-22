using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// See AttackTypes.cs for how to use.
public enum EffectType {
    None             = 0,
    ModifyManaCost   = 1,
    Damage           = 2,
    Heal             = 3,
    ModifyMaxHealth  = 4,
    ModifyAttack     = 5,
    ModifyBaseAttack = 6,
    ModifySpeed      = 7,
    ModifyBaseSpeed  = 8,
    ModifyArmor      = 9,
    Return           = 10,
    Summon           = 20, // executed in ExecutionManager
    Spikey           = 21,
    Poison           = 22,
    Hidden           = 23,
    Vanish           = 24,
    Solitary         = 25,
    Burn             = 26,
    ModifyBurn       = 27,
    Consume          = 28,
    Rooted           = 40,
    Push             = 41, // use with Standard TargetType
    Pull             = 42, // use with Far TargetType
    MoveRandom       = 43,
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
        { EffectType.ModifyManaCost, () => new ModifyManaCostEffect() },
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
        { EffectType.Solitary, () => new SolitaryEffect() },
        { EffectType.Burn, () => new BurnEffect() },
        { EffectType.ModifyBurn, () => new ModifyBurnEffect() },
        { EffectType.Consume, () => new ConsumeEffect() },
        { EffectType.Rooted, () => new RootedEffect() },
        { EffectType.Push, () => new PushEffect() },
        { EffectType.Pull, () => new PullEffect() },
        { EffectType.MoveRandom, () => new MoveRandomEffect() },
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
public class ModifyManaCostEffect : IEffect {
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
        targetSlot.Animal.spd.ChangeModifier(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.spd.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.spd.ChangeModifier(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.spd.Value);
    }
}
public class ModifyBaseSpeedEffect : IEffect {
    int modifierTotal;

    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.spd.ChangeBaseValue(args.val);
        modifierTotal += args.val;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.spd.Value);
    }

    public void Remove(CombatSlot targetSlot) {
        targetSlot.Animal.spd.ChangeBaseValue(-modifierTotal);
        modifierTotal = 0;
        EventManager.Invoke(targetSlot.Animal.gameObject, EventID.SetSpeed, targetSlot.Animal.spd.Value);
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

public class SolitaryEffect : IEffect {
    CombatSlot originSlot;
    IEffect atkMod;
    IEffect hpMod;
    
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        List<CombatSlot> targets = TargetTypes.GetTargets(new TargetArgs() {
            targetType = TargetType.Adjacent,
            originSlot = targetSlot,
            targetSlotState = TargetSlotState.NonEmpty,
            targetSameTeam = true,
            numTargetTimes = 1,
        });

        if (targets.Count > 0) { return; }

        atkMod = EffectTypeLookUp.CreateEffect(EffectType.ModifyAttack);
        atkMod.Apply(targetSlot, args);
        hpMod = EffectTypeLookUp.CreateEffect(EffectType.ModifyMaxHealth);
        hpMod.Apply(targetSlot, args);
        originSlot = targetSlot;
    }

    public void Remove(CombatSlot targetSlot) {
        atkMod.Remove(originSlot);
        hpMod.Remove(originSlot);
    }
}

public class BurnEffect : IEffect { // similar to DamageEffect, but allows global modifiers to apply distinctively
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        targetSlot.Animal.health.Damage(args.val + GameManager.Instance.playerMods.burnDmg); // invokes event
    }

    public void Remove(CombatSlot targetSlot) { return; }
}

public class ModifyBurnEffect : IEffect {
    int modifierTotal;
    
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        GameManager.Instance.playerMods.burnDmg += args.val;
        modifierTotal += args.val;
    }

    public void Remove(CombatSlot targetSlot) {
        GameManager.Instance.playerMods.burnDmg -= modifierTotal;
        modifierTotal = 0;
    }
}

public class ConsumeEffect : IEffect {
    List<IEffect> atkMods = new List<IEffect>();
    List<IEffect> hpMods = new List<IEffect>();
    
    public void Apply(CombatSlot targetSlot, EffectArgs args) { // targetSlot is consumer
        // find target
        List<CombatSlot> t = TargetTypes.GetTargets(new TargetArgs() {
            targetType = TargetType.RandomAdjacent,
            originSlot = targetSlot,
            targetSlotState = TargetSlotState.NonEmpty,
            targetSameTeam = true,
            numTargetTimes = 1,
        });
        if (t.Count != 1) return; // nothing adjacent to consume
        
        // absorb atk and hp
        IEffect atkMod = EffectTypeLookUp.CreateEffect(EffectType.ModifyAttack);
        IEffect hpMod = EffectTypeLookUp.CreateEffect(EffectType.ModifyMaxHealth);
        Animal consumedAnimal = t[0].Animal;
        
        atkMod.Apply(targetSlot, new EffectArgs() { val = consumedAnimal.atk.Value} );
        hpMod.Apply(targetSlot, new EffectArgs() { val = consumedAnimal.health.hp} );
        
        atkMods.Add(atkMod);
        atkMods.Add(hpMod);
        
        // absorb attack effects
        if (consumedAnimal.cardText.condition == EventID.Attack) {
            foreach (Effect effect in consumedAnimal.cardText.effects) {
                targetSlot.Animal.cardText.effects.Add(new Effect(effect));
            }
        }
    }

    public void Remove(CombatSlot targetSlot) {
        foreach (IEffect atkMod in atkMods) {
            atkMod.Remove(targetSlot);
        }
        foreach (IEffect hpMod in hpMods) {
            hpMod.Remove(targetSlot);
        }
    }
}

public class RootedEffect : IEffect {
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

public class MoveRandomEffect : IEffect {
    public void Apply(CombatSlot targetSlot, EffectArgs args) {
        // find potential slots to move target to
        List<CombatSlot> selectedSlot = TargetTypes.GetTargets(new TargetArgs() {
            targetType = TargetType.Random,
            originSlot = targetSlot,
            targetSlotState = TargetSlotState.Empty,
            targetSameTeam = true,
            numTargetTimes = 1,
        });
        if (selectedSlot.Count != 1) return; // no available slots to move to
        
        // move target to random chosen slot
        targetSlot.SwapWithCombatSlot(selectedSlot[0]);
    }

    public void Remove(CombatSlot targetSlot) { return; }
}


using System.Collections.Generic;

// This enum-dictionary hack used to allow choosing an IAttack implementation from inspector of SO_Animal scriptable object.
// - Should be easily manageable, but will not be needed if moving to external (non-SO) method of storing card data.
// 
// To Use:
// 1. Add IAttack implementation below
// 2. Add IAttack implementation name to AttackType enum
// 3. Add <AttackType, IAttack> lookup to dictionary
// 4. (done once) In Animal.Start(), use lookup to assign IAttack 
// 5. (done once) In SO_Animal, add AttackType enum
public enum AttackType {
    Standard,
    None,
    Double,
    Hidden,
    Burn,
    Sunder,
    Piercing,
}

public static class AttackTypeLookUp {
    public static Dictionary<AttackType, IAttack> LookUp = new Dictionary<AttackType, IAttack>() {
        {AttackType.Standard, new StandardAttack()},
        {AttackType.None, new NoneAttack()},
        {AttackType.Double, new DoubleAttack()},
        {AttackType.Hidden, new HiddenAttack()},
        {AttackType.Burn, new BurnAttack()},
        {AttackType.Sunder, new SunderAttack()},
        {AttackType.Piercing, new PiercingAttack()},
    };
}

/********************************************/

public interface IAttack {
    // Attack returns true if Attack found valid targets (false if hit no targets, such as at field boundaries)
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg);
}

public class StandardAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg) {
        List<CombatSlot> targets = TargetTypes.GetTargets(new TargetArgs() {
            targetType = targetType,
            originSlot = originSlot,
            targetSlotState = TargetSlotState.NonEmpty,
            numTargetTimes = 1,
        });

        // found targets, apply damage
        foreach (CombatSlot c in targets) {
            c.Animal.health.Damage(dmg);
        }
        if (targets.Count > 0) {
            return true;
        }
        
        // nothing in row to hit, enemy can hit player
        if (originSlot.Animal.isEnemy) {
            GameManager.Instance.playerLife.ModifyHp(-dmg);
            return true;
        }
        
        return false;
    }
}

public class NoneAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg) {
        return true;
    }
}

public class DoubleAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg) {
        bool ret1 = AttackTypeLookUp.LookUp[AttackType.Standard].Attack(targetType, originSlot, dmg);
        if (!ret1) return false;
        
        AttackTypeLookUp.LookUp[AttackType.Standard].Attack(targetType, originSlot, dmg);

        return true;    // always return true after second attack since first must have hit
    }
}

public class HiddenAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg) {
        if (originSlot.Animal.EffectCtrl.FindEffect(EffectType.Hidden) != null) {
            return AttackTypeLookUp.LookUp[AttackType.Double].Attack(targetType, originSlot, dmg);
        } else {
            return AttackTypeLookUp.LookUp[AttackType.Standard].Attack(targetType, originSlot, dmg);
        }
    }
}

public class BurnAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg) {
        List<CombatSlot> targetSlots = TargetTypes.GetTargets(new TargetArgs() {
            targetType = targetType,
            originSlot = originSlot,
            targetSlotState = TargetSlotState.NonEmpty,
            numTargetTimes = 1,
        });

        if (targetSlots.Count > 0) {
            foreach (CombatSlot c in targetSlots) {
                Effect e = EffectPresetLookup.effectPresets["Burn"];
                e.source = originSlot.Animal;
                e.remainingDuration = dmg;
                
                c.Animal.EffectCtrl.AddEffect(e);
            }
            return true;
        }

        // cannot burn player

        return false;
    }
}

public class SunderAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg) {
        List<CombatSlot> targets = TargetTypes.GetTargets(new TargetArgs() {
            targetType = targetType,
            originSlot = originSlot,
            targetSlotState = TargetSlotState.NonEmpty,
            numTargetTimes = 1,
        });

        // found targets, apply damage through armor
        foreach (CombatSlot c in targets) {
            c.Animal.health.ModifyHp(-dmg);
        }
        if (targets.Count > 0) {
            return true;
        }
        
        // nothing in row to hit, enemy can hit player
        if (originSlot.Animal.isEnemy) {
            GameManager.Instance.playerLife.ModifyHp(-dmg);
            return true;
        }
        
        return false;
    }
}

public class PiercingAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg) {
        List<CombatSlot> targets = TargetTypes.GetTargets(new TargetArgs() {
            targetType = targetType,
            originSlot = originSlot,
            targetSlotState = TargetSlotState.NonEmpty,
            numTargetTimes = 1,
        });

        // found targets, apply damage
        foreach (CombatSlot c in targets) {
            int realDmg = c.Animal.health.Damage(dmg); // returns as negative if damage was dealt

            if (-realDmg < dmg && realDmg != 0) {
                List<CombatSlot> targetBehind = TargetTypes.GetTargets(new TargetArgs() {
                    targetType = TargetType.Backward,
                    originSlot = c,
                    targetSlotState = TargetSlotState.NonEmpty,
                    targetSameTeam = true,
                    numTargetTimes = 1,
                });

                if (targetBehind.Count > 0) {
                    targetBehind[0].Animal.health.Damage(dmg - (-realDmg));
                    return true;
                }
        
                // nothing left in row to hit, enemy can hit player
                if (originSlot.Animal.isEnemy) {
                    GameManager.Instance.playerLife.ModifyHp(-dmg);
                    return true;
                }
            }
        }
        if (targets.Count > 0) {
            return true;
        }
        
        // nothing in row to hit, enemy can hit player
        if (originSlot.Animal.isEnemy) {
            GameManager.Instance.playerLife.ModifyHp(-dmg);
            return true;
        }
        
        return false;
    }
}
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
    Burn,
}

public static class AttackTypeLookUp {
    public static Dictionary<AttackType, IAttack> LookUp = new Dictionary<AttackType, IAttack>() {
        {AttackType.Standard, new StandardAttack()},
        {AttackType.None, new NoneAttack()},
        {AttackType.Burn, new BurnAttack()},
    };
}

/********************************************/

public interface IAttack {
    // Attack returns true if Attack found valid targets (false if hit no targets, such as at field boundaries)
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg, bool enemyCalled = false);
}

public class StandardAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg, bool enemyCalled) {
        List<Animal> targets = TargetTypes.GetTargets(targetType, originSlot);

        // found targets, apply damage
        if (targets.Count > 0) {
            foreach (Animal a in targets) {
                a.health.Damage(dmg);
            }
            return true;
        }
        
        // nothing in row to hit, enemy can hit player
        if (enemyCalled) {
            GameManager.Instance.playerLife.ModifyHp(-dmg);
            return true;
        }
        
        return false;
    }
}

public class NoneAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg, bool enemyCalled) {
        return true;
    }
}

public class BurnAttack : IAttack {
    public bool Attack(TargetType targetType, CombatSlot originSlot, int dmg, bool enemyCalled) {
        List<Animal> targets = TargetTypes.GetTargets(targetType, originSlot);

        // TODO: make library of effects and pull Burn effect, then modify dmg
        if (targets.Count > 0) {
            foreach (Animal a in targets) {
                Effect e = new Effect("Burn", EffectType.DamageEffect, EffectPermanence.Duration, dmg, 2);
                a.GetComponent<EffectController>().AddEffect(e);
            }
            return true;
        }

        // cannot burn player

        return false;
    }
}
using System.Collections.Generic;

// See AttackTypes.cs for how to use.
public enum AbilityType {
    GainHp,
    BoostAttack,
    Poison,
}

public static class AbilityTypeLookUp {
    public static Dictionary<AbilityType, IAbility> LookUp = new Dictionary<AbilityType, IAbility>() {
        {AbilityType.GainHp, new GainHp()},
        {AbilityType.BoostAttack, new ModifyAttack()},
        {AbilityType.Poison, new Poison()},
    };
}

/********************************************/

public interface IAbility {
    public void Use(Slot originSlot, int val, bool flip = false);
}

public class GainHp : IAbility {
    public void Use(Slot originSlot, int val, bool flip) {
        originSlot.Card.GetComponent<Health>().Heal(val);
    }
}

public class ModifyAttack : IAbility {
    public void Use(Slot originSlot, int val, bool flip) {
        Effect e = new Effect("ModifyAttack", val, 10, false, EffectType.ModifyAttackEffect);
        
        originSlot.Card.GetComponent<EffectController>().AddEffect(e);
    }
}

public class Poison : IAbility {
    public void Use(Slot originSlot, int val, bool flip) {
        Slot targetSlot = originSlot.SlotGrid.Forward(originSlot, flip);
        Effect e = new Effect("Poison", val, 3, true, EffectType.DamageEffect);
        
        targetSlot.Card.GetComponent<EffectController>().AddEffect(e);
    }
}
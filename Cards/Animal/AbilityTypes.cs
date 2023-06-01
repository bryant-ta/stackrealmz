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
    public void Use(Slot mSlot, int val);
}

public class GainHp : IAbility {
    public void Use(Slot mSlot, int val) {
        mSlot.Card.GetComponent<Health>().Heal(val);
    }
}

public class ModifyAttack : IAbility {
    public void Use(Slot mSlot, int val) {
        Effect e = new Effect("ModifyAttack", val, 10, false, EffectType.ModifyAttackEffect);
        
        mSlot.Card.GetComponent<EffectController>().AddEffect(e);
    }
}

public class Poison : IAbility {
    public void Use(Slot mSlot, int val) {
        Slot targetSlot = mSlot.SlotGrid.Forward(mSlot);
        Effect e = new Effect("Poison", val, 3, true, EffectType.DamageEffect);
        
        targetSlot.Card.GetComponent<EffectController>().AddEffect(e);
    }
}
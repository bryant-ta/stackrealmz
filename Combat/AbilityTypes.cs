using System.Collections.Generic;

// See AttackTypes.cs for how to use.
public enum AbilityType {
    None,
    GainHp,
    BoostAttack,
    Poison,
}

public static class AbilityTypeLookUp {
    public static Dictionary<AbilityType, IAbility> LookUp = new Dictionary<AbilityType, IAbility>() {
        {AbilityType.None, new NoneAbility()},
        {AbilityType.GainHp, new GainHpAbility()},
        {AbilityType.BoostAttack, new ModifyAttackAbility()},
        {AbilityType.Poison, new PoisonAbility()},
    };
}

/********************************************/

public interface IAbility {
    public void Use(Slot originSlot, int val, bool flip = false);
}

public class NoneAbility : IAbility {
    public void Use(Slot originSlot, int val, bool flip) {
        return;
    }
}

public class GainHpAbility : IAbility {
    public void Use(Slot originSlot, int val, bool flip) {
        originSlot.Card.GetComponent<Health>().ModifyHp(val);
    }
}

public class ModifyAttackAbility : IAbility {
    public void Use(Slot originSlot, int val, bool flip) {
        Effect e = new Effect("ModifyAttack", EffectType.ModifyAttack, EffectPermanence.Temporary, val, 10);
        
        originSlot.Card.GetComponent<EffectController>().AddEffect(e);
    }
}

public class PoisonAbility : IAbility {
    public void Use(Slot originSlot, int val, bool flip) {
        Slot targetSlot = originSlot.SlotGrid.Forward(originSlot, flip);
        Effect e = new Effect("Poison", EffectType.Damage, EffectPermanence.Duration, val, 3);
        
        targetSlot.Card.GetComponent<EffectController>().AddEffect(e);
    }
}
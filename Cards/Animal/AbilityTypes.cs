using System.Collections.Generic;
using UnityEngine;

// See AttackTypes.cs for how to use.
public enum AbilityType {
    GainHp
}

public static class AbilityTypeLookUp {
    public static Dictionary<AbilityType, IAbility> LookUp = new Dictionary<AbilityType, IAbility>() {
        {AbilityType.GainHp, new GainHp()}
    };
}

/********************************************/

public interface IAbility {
    void Use(Slot mSlot, int value);
}

public class GainHp : IAbility
{
    public void Use(Slot mSlot, int hp) {
        mSlot.Card.GetComponent<Health>().Heal(hp);
    }
}
using System.Collections.Generic;
using UnityEngine;

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
    Sweep
}

public static class AttackTypeLookUp {
    public static Dictionary<AttackType, IAttack> LookUp = new Dictionary<AttackType, IAttack>() {
        {AttackType.Standard, new StandardAttack()},
        {AttackType.Sweep, new SweepAttack()}
    };
}

/********************************************/

public interface IAttack {
    void Attack(Slot mSlot, int dmg);
}

public class StandardAttack : IAttack
{
    public void Attack(Slot mSlot, int dmg) {
        Slot targetSlot = mSlot.SlotGrid.Forward(mSlot);
        
        if (targetSlot && targetSlot.Card) {
            targetSlot.Card.GetComponent<Health>().Damage(dmg);
        }
    }
}

public class SweepAttack : IAttack
{
    public void Attack(Slot mSlot, int dmg) {
        List<Slot> targetSlots = new List<Slot>();
        for (int x = -1; x <= 1; x++) {
            Slot s = mSlot.SlotGrid.SelectSlot(mSlot, new Vector2Int(x, 1));
            if (s) { targetSlots.Add(s); }
        }

        foreach (Slot s in targetSlots) {
            s.Card.GetComponent<Health>().Damage(dmg);
        }
    }
}

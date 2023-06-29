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
    // Attack returns true if Attack found valid targets (false if hit no targets, such as at field boundaries)
    public bool Attack(Slot originSlot, int dmg, bool flip = false);
}

public class StandardAttack : IAttack
{
    public bool Attack(Slot originSlot, int dmg, bool flip) {
        Slot targetSlot = originSlot.SlotGrid.Forward(originSlot, flip);
        
        if (targetSlot) {
            if (targetSlot.Card) {
                targetSlot.Card.GetComponent<Health>().Damage(dmg);
                return true;
            } else if (targetSlot.x == 0) {
                GameManager.Instance.playerLife.Damage(dmg);
                return true;
            }
        }

        return false;
    }
}

public class SweepAttack : IAttack
{
    public bool Attack(Slot originSlot, int dmg, bool flip) {
        List<Slot> targetSlots = new List<Slot>();
        for (int y = -1; y <= 1; y++) {
            Slot targetSlot = originSlot.SlotGrid.SelectSlot(originSlot, flip, new Vector2Int(1, y));
            if (targetSlot) { targetSlots.Add(targetSlot); }
        }

        bool didHit = false;
        foreach (Slot targetSlot in targetSlots) {
            if (targetSlot.Card) {
                targetSlot.Card.GetComponent<Health>().Damage(dmg);
                didHit = true;
            } else if (targetSlot.x == 0) {
                GameManager.Instance.playerLife.Damage(dmg);
                didHit = true;
            }
        }

        return didHit;
    }
}

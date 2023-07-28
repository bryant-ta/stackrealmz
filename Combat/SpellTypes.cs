using System.Collections.Generic;
using UnityEngine;

public enum SpellType {
    None = 0,
    Effect = 1,
    Move = 2,
}

public static class SpellTypeLookUp {
    public static Dictionary<SpellType, ISpell> LookUp = new Dictionary<SpellType, ISpell>() {
        {SpellType.None, new NoneSpell()},
        {SpellType.Effect, new EffectSpell()},
        {SpellType.Move, new MoveSpell()},
    };
}

/********************************************/

public interface ISpell {
    public void Execute(List<CombatSlot> targetSlots, CardText cardText);
}

public class NoneSpell : ISpell {
    public void Execute(List<CombatSlot> targetSlots, CardText cardText) { return; }
}

public class EffectSpell : ISpell {
    public void Execute(List<CombatSlot> targetSlots, CardText cardText) {
        foreach (CombatSlot slot in targetSlots) {
            EffectOrder eo = new EffectOrder() {
                originSlot = slot,
                cardText = cardText,
            };
            EffectManager.Instance.AddEffectOrder(eo);
        }
    }
}

public class MoveSpell : ISpell {
    public void Execute(List<CombatSlot> targetSlots, CardText cardText) {
        if (targetSlots.Count != 2) {
            Debug.LogError("Cannot execute MoveSpell without exactly 2 targets");
            return;
        }
        
        targetSlots[0].SwapWithCombatSlot(targetSlots[1]);
    }
}
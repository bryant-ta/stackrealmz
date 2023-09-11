using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatSlot : Slot {
    public Animal Animal { get => animal; private set { animal = value; } }
    [SerializeField] protected Animal animal;

    public bool isEnemySlot;
    public int executionPriority;
    public Terrain terrain;
    
    public List<Effect> activeAuraEffects = new List<Effect>();

    public override bool PlaceAndMove(Stack stack, bool isPlayerCalled = false) {
        // Play Conditions
        if (!stack.GetTopCard().TryGetComponent(out Animal a)) return false;
        if (isPlayerCalled && GameManager.Instance.Mana < a.manaCost.Value) return false;
        
        // Play Condition + Move result
        if (!base.PlaceAndMove(stack, isPlayerCalled)) return false;

        // Cost result
        if (isPlayerCalled) {
            GameManager.Instance.ModifyMana(-a.manaCost.Value);
        }

        animal = a;
        
        // Play Effect
        if (!animal.isInCombat) {
            animal.StartCombatState();
            animal.Play();
        }
        
        // Apply Auras Effects
        foreach (Effect e in activeAuraEffects) {
            animal.EffectCtrl.AddEffect(e);
        }
        
        EventManager.Invoke(animal.gameObject, EventID.CardPlaced);

        return true;
    }
    
    public override Transform PickUpHeld(bool isPlayerCalled = false, bool endCombatState = false, bool doEventInvoke = true) {
        if (!PickUpCondition(isPlayerCalled)) {
            // Remove Auras Effects
            foreach (Effect e in activeAuraEffects) {
                animal.EffectCtrl.RemoveEffect(e);
            }
        }

        Transform ret = base.PickUpHeld(isPlayerCalled);
        if (ret == null) return null;

        if (endCombatState) {
            animal.EndCombatState();
        }

        animal = null;

        return ret;
    }

    public override Stack SpawnCard(SO_Card cardData) {
        if (!IsEmpty()) return null;
        
        Stack s = CardFactory.CreateStack(transform.position, cardData);
        if (!s.GetTopCard().TryGetComponent(out Animal a)) {
            Debug.LogError("SpawnCard: could not spawn Animal in CombatSlot");
            return null;
        }

        if (isEnemySlot) {
            a.isEnemy = true;
        }

        a.Start();
        PlaceAndMove(s);
        
        return s;
    }

    void MoveHeldToCombatSlot(CombatSlot slot) {
        Animal temp = animal;
        
        Transform ret = PickUpHeld();
        if (ret == null) return;

        slot.PlaceAndMove(temp.mStack);
    }

    // SwapWithCombatSlot swaps two CombatSlots' Animals. If one slot does not have an Animal, this will
    // act as a move instead.
    public bool SwapWithCombatSlot(CombatSlot slot) {
        if ((animal && animal.EffectCtrl.FindEffect(EffectType.Rooted) != null) || (slot.Animal && slot.Animal.EffectCtrl.FindEffect(EffectType.Rooted) != null)) {
            Debug.Log("Cannot move Rooted card!");
            return false;
        }

        Animal temp = animal;
        
        Transform ret = PickUpHeld();
        if (ret == null) return false;

        slot.MoveHeldToCombatSlot(this);
        
        slot.PlaceAndMove(temp.mStack);
        return true;
    }

    // Manipulation funcs for aura result effects to apply to held Animal (aura effect sourced from neighbor, modifies this)
    public void AddActiveAuraEffect(Effect e) {
        if (Animal) {
            Animal.EffectCtrl.AddEffect(e);
        }
        
        activeAuraEffects.Add(e);
    }
    public void RemoveActiveAuraEffect(Effect auraEffect) {
        // Remove active aura effect from animal in slot
        if (Animal) {
            foreach (Effect activeAuraEffect in activeAuraEffects) {
                if (activeAuraEffect.IsEqual(auraEffect)) Animal.EffectCtrl.RemoveEffect(activeAuraEffect);
            }
        }

        // Remove active aura effect from apply list
        activeAuraEffects = activeAuraEffects.Where(effect => !effect.IsEqual(auraEffect)).ToList();
    }
}

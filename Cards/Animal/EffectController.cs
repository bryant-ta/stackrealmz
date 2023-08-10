using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// EffectController executes all effects for the Animal it is attached to
[RequireComponent(typeof(Animal))]
public class EffectController : MonoBehaviour {
    public List<Effect> permEffects = new List<Effect>();
    public List<Effect> tempEffects = new List<Effect>();
    public List<Effect> durationEffects = new List<Effect>();
    public List<Effect> auraEffects = new List<Effect>();

    public List<CombatSlot> auraTargets = new List<CombatSlot>();

    Animal mAnimal;

    void Start() {
        mAnimal = GetComponent<Animal>();

        EventManager.Subscribe(gameObject, EventID.ExitCombat, ResetEffects);
        EventManager.Subscribe(mAnimal.gameObject, EventID.CardPickedUp, RemoveAuraEffectFromTargets); // aura source moved away, remove aura from all neighbors
    }
    
    

    // AddEffect handles input effect based on its effect permanence
    //   note: used enum instead of derived classes (PermEffect, TempEffect...) bc ExecuteEffect would need to get
    //   more complex to handle Effects with remainingDuration - not worth it
    public void AddEffect(Effect e) {
        e = new Effect(e);
        switch (e.effectPermanence) {
            case EffectPermanence.Permanent:
                AddPermEffect(e);
                break;
            case EffectPermanence.Temporary:
                AddTempEffect(e);
                break;
            case EffectPermanence.Duration:
                AddDurationEffect(e);
                break;
            default:
                Debug.LogError("Unhandled effect permanence type.");
                return;
        }
    }

    // AddPermEffect handles permanent effects (execute once, never removed)
    void AddPermEffect(Effect e) {
        if (e.remainingDuration > 0) {
            Debug.LogError("AddPermEffect() requires effect with no duration");
            return;
        }

        e.effectFunc.Apply(mAnimal.mCombatSlot, new EffectArgs() {val = e.baseValue});
        
        // Don't add effect that is pure one-time damage
        if (e.name != "Damage") permEffects.Add(e);
    }
    // AddTempEffect handles one time effects (execute once, removeable)            Note: does not support execute once, remove after duration - but what effect even needs that??
    void AddTempEffect(Effect e) {
        if (e.remainingDuration > 0) {
            Debug.LogError("AddTempEffect() requires effect with no duration");
            return;
        }

        e.effectFunc.Apply(mAnimal.mCombatSlot, new EffectArgs() {val = e.baseValue});
        tempEffects.Add(e);
    }
    // AddDurationEffect handles over time effects (execute multiple times over duration, removeable after duration)
    // - remainingDuration = -1 : execute multiple times, do not remove from duration
    void AddDurationEffect(Effect e) {
        if (e.remainingDuration == 0) {
            Debug.LogError("AddDurationEffect() requires effect with duration");
            return;
        }

        durationEffects.Add(e);
    }

    public bool RemoveEffect(Effect e) {
        switch (e.effectPermanence) {
            case EffectPermanence.Temporary:
                foreach (Effect effect in tempEffects) {
                    if (effect.IsEqual(e)) effect.effectFunc.Remove(mAnimal.mCombatSlot);
                }
                tempEffects.Remove(e);
                return true;
            case EffectPermanence.Duration:
                foreach (Effect effect in durationEffects) {
                    if (effect.IsEqual(e)) effect.effectFunc.Remove(mAnimal.mCombatSlot);
                }
                durationEffects.Remove(e);
                return true;
            case EffectPermanence.Aura:
                Debug.LogError("Cannot remove Aura source effect");
                return false;
            case EffectPermanence.Permanent:
                Debug.LogError("Cannot remove Permanent effect");
                return false;
            default:
                Debug.LogError("Unhandled effect permanence type.");
                return false;
        }
    }
    public bool RemoveEffect(EffectType effectType) {
        Effect e = FindEffect(effectType);
        if (e == null) return false;
        
        return RemoveEffect(e);
    }
    
    public void ResetEffects() {
        // TODO: possible inconsistent execution order for remove effects since it's not executed by ExecutionManager... leaving for now
        foreach (Effect e in tempEffects) {
            e.effectFunc.Remove(mAnimal.mCombatSlot);
        }
        tempEffects.Clear();

        foreach (Effect e in durationEffects) {
            e.effectFunc.Remove(mAnimal.mCombatSlot);
        }
        durationEffects.Clear();
    }

    public Effect FindEffect(EffectType effectType) {
        List<Effect> allEffects = permEffects.Concat(tempEffects).Concat(durationEffects).Concat((auraEffects))
            .ToList();
        foreach (Effect e in allEffects) {
            if (e.effectType == effectType) return e;
        }
        
        return null;
    }

    public string ActiveEffectsToString() {
        string ret = "";
        foreach (Effect e in durationEffects) {
            ret += string.Format("{0}: {1} ({2} turns left)\n", e.name, e.baseValue, e.remainingDuration);
        }

        foreach (Effect e in tempEffects) {
            ret += string.Format("{0}: {1}\n", e.name, e.baseValue);
        }

        foreach (Effect e in permEffects) {
            ret += string.Format("{0}: {1}\n", e.name, e.baseValue);
        }

        return ret;
    }
    
    /*************************   Aura Effects   *************************/

    public void AddAuraEffect(Effect auraEffect) {
        if (!auraEffects.Contains(auraEffect)) {
            auraEffects.Add(auraEffect);
        }

        Effect auraRealEffect = new Effect(auraEffect.name, auraEffect.effectType, EffectPermanence.Temporary, auraEffect.baseValue, 0, auraEffect.source);
        
        auraTargets.Clear();
        auraTargets = TargetTypes.GetTargets(new TargetArgs() {
            targetType = mAnimal.cardText.targetArgs.targetType,
            originSlot = mAnimal.mCombatSlot,
            targetSlotState = TargetSlotState.Any,
            targetSameTeam = true,
            targetGroup = mAnimal.cardText.targetArgs.targetGroup,
            numTargetTimes = mAnimal.cardText.targetArgs.numTargetTimes,
        });
        
        foreach (CombatSlot auraTargetSlot in auraTargets) {
            auraTargetSlot.AddActiveAuraEffect(auraRealEffect);
        }
    }

    void RemoveAuraEffectFromTargets() {
        foreach (CombatSlot auraTarget in auraTargets) {
            foreach (Effect e in auraEffects) {
                auraTarget.RemoveActiveAuraEffect(e);
            }
        }
    }
}
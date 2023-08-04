using System.Collections.Generic;
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

    public void AddAuraEffect(Effect auraEffect) {
        if (!auraEffects.Contains(auraEffect)) {
            auraEffects.Add(auraEffect);
        }

        Effect auraRealEffect = new Effect(auraEffect.name, auraEffect.effectType, EffectPermanence.Permanent, auraEffect.baseValue, 0, auraEffect.source);
        
        auraTargets.Clear();
        auraTargets = TargetTypes.GetTargets(mAnimal.cardText.auraTargetType, mAnimal.mCombatSlot, mAnimal.cardText.targetGroup);
        
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

    // AddPermEffect handles any permanent effects (execute once, never removed from duration)
    public void AddPermEffect(Effect e) {
        if (e.remainingDuration > 0) {
            Debug.LogError("AddPermEffect() requires effect with no duration");
            return;
        }

        e.effectFunc.Apply(mAnimal.mCombatSlot, new EffectArgs() {val = e.baseValue});
        permEffects.Add(e);
    }
    // AddTempEffect handles any one time effects (execute once, removeable after duration)
    public void AddTempEffect(Effect e) {
        if (e.remainingDuration == 0) {
            Debug.LogError("AddTempEffect() requires effect with duration");
            return;
        }

        e.effectFunc.Apply(mAnimal.mCombatSlot, new EffectArgs() {val = e.baseValue});
        tempEffects.Add(e);
    }
    // AddDurationEffect handles any over time effects (execute multiple times over duration, removeable after duration)
    public void AddDurationEffect(Effect e) {
        if (e.remainingDuration == 0) {
            Debug.LogError("AddDurationEffect() requires effect with duration");
            return;
        }

        durationEffects.Add(e);
    }

    public bool RemoveEffect(Effect e) {
        switch (e.effectPermanence) {
            case EffectPermanence.Permanent:
                foreach (Effect permEffect in permEffects) {
                    if (permEffect.IsEqual(e)) permEffect.effectFunc.Remove(mAnimal.mCombatSlot);
                }
                return true;
            case EffectPermanence.Temporary:
                foreach (Effect tempEffect in tempEffects) {
                    if (tempEffect.IsEqual(e)) tempEffect.effectFunc.Remove(mAnimal.mCombatSlot);
                }
                return true;
            case EffectPermanence.Duration:
                foreach (Effect durationEffect in durationEffects) {
                    if (durationEffect.IsEqual(e)) durationEffect.effectFunc.Remove(mAnimal.mCombatSlot);
                }
                return true;
            case EffectPermanence.Aura:
                Debug.LogError("Cannot remove Aura source effect");
                return false;
            default:
                Debug.LogError("Unhandled effect permanence type.");
                return false;
        }
    }
    public void ResetEffects() {
        // TODO: possible inconsistent execution order for remove effects since it's not executed by EffectManager... leaving for now
        foreach (Effect e in tempEffects) {
            e.effectFunc.Remove(mAnimal.mCombatSlot);
        }

        tempEffects.Clear();

        foreach (Effect e in durationEffects) {
            e.effectFunc.Remove(mAnimal.mCombatSlot);
        }

        durationEffects.Clear();
    }

    public string ActiveEffectsToString() {
        string ret = "";
        foreach (Effect e in durationEffects) {
            ret += string.Format("{0}: {1} ({2} turns left)\n", e.name, e.baseValue, e.remainingDuration);
        }

        foreach (Effect e in tempEffects) {
            ret += string.Format("{0}: {1} ({2} turns left)\n", e.name, e.baseValue, e.remainingDuration);
        }

        foreach (Effect e in permEffects) {
            ret += string.Format("{0}: {1}\n", e.name, e.baseValue);
        }

        return ret;
    }
}
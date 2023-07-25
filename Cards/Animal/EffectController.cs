using System;
using System.Collections.Generic;
using UnityEngine;

// EffectController manages all active effects on the Animal it is attached to
[RequireComponent(typeof(Animal))]
public class EffectController : MonoBehaviour {
    public readonly List<Effect> permEffects = new List<Effect>();
    public readonly List<Effect> tempEffects = new List<Effect>();
    public readonly List<Effect> durationEffects = new List<Effect>();

    Animal mAnimal;

    void Start() {
        mAnimal = GetComponent<Animal>(); 
        
        EventManager.Subscribe(gameObject, EventID.ExitCombat, ResetEffects);
    }

    // AddEffect handles input effect based on its effect permanence
    //   note: used enum instead of derived classes (PermEffect, TempEffect...) bc ExecuteEffect would need to get
    //   more complex to handle Effects with remainingDuration - not worth it
    public void AddEffect(Effect e) {
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
        }
    }

    // AddPermEffect handles any permanent effects (execute once, never removed)
    public void AddPermEffect(Effect e) {
        if (e.remainingDuration > 0) {
            Debug.LogError("AddPermEffect() cannot be used with non-permanent effect");
            return;
        }

        e.effectFunc.Execute(mAnimal, e.baseValue);
        permEffects.Add(e);
    }
    // AddTempEffect handles any one time effects (execute once, removeable)
    public void AddTempEffect(Effect e) {
        if (e.remainingDuration == 0) {
            Debug.LogError("AddTempEffect() cannot be used with permanent effect");
            return;
        }
        
        e.effectFunc.Execute(mAnimal, e.baseValue);
        tempEffects.Add(e);
    }
    // AddDurationEffect handles any over time effects (execute multiple times over duration, removeable)
    public void AddDurationEffect(Effect e) {
        if (e.remainingDuration == 0) {
            Debug.LogError("AddDurationEffect() cannot be used with permanent effect");
            return;
        }
        
        durationEffects.Add(e);
    }

    public bool RemoveEffect(Effect e) {
        e.effectFunc.Remove(mAnimal);

        return tempEffects.Remove(e) || durationEffects.Remove(e);
    }
    public void ResetEffects() {                // TODO: possible inconsistent execution order for remove effects since it's not executed by EffectManager... leaving for now
        foreach (Effect e in tempEffects) {
            e.effectFunc.Remove(mAnimal);
        }
        tempEffects.Clear();
        
        foreach (Effect e in durationEffects) {
            e.effectFunc.Remove(mAnimal);
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
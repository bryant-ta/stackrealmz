using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]      // NEED THIS
public class Effect {
    public string name;
    public EffectType effectType;
    public EffectPermanence effectPermanence;
    
    public int baseValue;
    public int remainingDuration;
    [HideInInspector] public Card source;
    
    public SO_Card summonData;
    
    public IEffect effectFunc;

    public Effect(string name, EffectType effectType, EffectPermanence effectPermanence, int baseValue, int remainingDuration, Card source = null, SO_Card summonData = null) {
        this.name = name;
        this.effectType = effectType;
        this.effectPermanence = effectPermanence;
        this.baseValue = baseValue;
        this.remainingDuration = remainingDuration;
        this.source = source;

        this.summonData = summonData;
        
        effectFunc = EffectTypeLookUp.CreateEffect(effectType);
    }

    public Effect(Effect e) : this(e.name, e.effectType, e.effectPermanence, e.baseValue, e.remainingDuration, e.source, e.summonData) { }

    // Returns true if this Effect has same name and source as input
    public bool IsEqual(Effect e) {
        return name == e.name && source == e.source;
    }
    
    // Returns true if this Effect is the real aura effect of the input aura effect (is the effect placed on neighbors)
    public bool IsAuraRealEffect(Effect auraEffect) {
        return name == auraEffect.name && effectPermanence == EffectPermanence.Temporary &&
               auraEffect.effectPermanence == EffectPermanence.Aura && source == auraEffect.source;
    }
}

public static class EffectPresetLookup {
    public static Dictionary<string, Effect> effectPresets = new Dictionary<string, Effect>() {
        {"Burn", new Effect("Burn", EffectType.Burn, EffectPermanence.Duration, 1, 2)},
    };
}

public enum EffectPermanence {
    Permanent = 0,
    Temporary = 1,
    Duration = 2,
    Aura = 4,
}
using System;
using System.Collections.Generic;

[Serializable]      // NEED THIS
public class Effect {
    public string name;
    public EffectType effectType;
    public EffectPermanence effectPermanence;

    public int baseValue;
    public int remainingDuration;

    public IEffect effectFunc;

    public Effect(string name, EffectType effectType, EffectPermanence effectPermanence, int baseValue, int remainingDuration) {
        this.name = name;
        this.baseValue = baseValue;
        this.effectPermanence = effectPermanence;
        this.remainingDuration = remainingDuration;

        effectFunc = EffectTypeLookUp.LookUp[effectType];
    }
}

public static class EffectPresetLookup {
    public static Dictionary<string, Effect> effectPresets = new Dictionary<string, Effect>() {
        {"Burn", new Effect("Burn", EffectType.DamageEffect, EffectPermanence.Duration, 1, 1)},
        {"Poison", new Effect("Poison", EffectType.DamageEffect, EffectPermanence.Duration, 1, 1)},
    };
}

public enum EffectPermanence {
    Permanent = 0,
    Temporary = 1,
    Duration = 2,
}
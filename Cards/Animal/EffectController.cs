using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animal))]
public class EffectController : MonoBehaviour
{
    List<Effect> curEffects = new List<Effect>();
    List<Effect> oneTimeCurEffects = new List<Effect>();

    Animal mAnimal;

    void Start() {
        mAnimal = GetComponent<Animal>();
        
        CombatManager.onTick.AddListener(ExecuteEffects);
    }

    void ExecuteEffects() {
        foreach (Effect e in curEffects.ToList()) {
            print(e.name + " " + e.remainingDuration);
            if (e.overTime) {
                e.effect.Execute(mAnimal, e.baseValue);
            }

            e.remainingDuration--;
            if (e.remainingDuration == 0) {
                e.effect.Remove(mAnimal);
                curEffects.Remove(e);
            }
        }
    }

    // AddEffect handles any effects with durations
    public void AddEffect(Effect e) {
        curEffects.Add(e);

        if (!e.overTime) {
            e.effect.Execute(mAnimal, e.baseValue);
        }
    }

    // AddEffectPerm executes a permanent effect on mAnimal
    public void AddEffectPerm(Effect e) {
        if (e.remainingDuration > 0 || e.overTime) { Debug.LogError("AddEffectPerm(): trying to add non-permanent effect");}
        print(e.effect);
        print(EffectTypeLookUp.LookUp[EffectType.ModifyBaseAttackEffect]);
        e.effect.Execute(mAnimal, e.baseValue);
    }
}

[Serializable]
public class Effect {
    [SerializeField] public string name;
    [SerializeField] public int baseValue;
    [SerializeField] public int remainingDuration;
    [SerializeField] public bool overTime;

    [SerializeField] public EffectType effectType;
    public IEffect effect;

    public Effect(string name, int baseValue, int remainingDuration, bool overTime, EffectType effectType) {
        this.name = name;
        this.baseValue = baseValue;
        this.remainingDuration = remainingDuration;
        this.overTime = overTime;

        effect = EffectTypeLookUp.LookUp[effectType];
    }
}

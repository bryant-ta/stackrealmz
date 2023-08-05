using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Cards/SO_Animal")]
public class SO_Animal : SO_Card {
    public int manaCost;
    public int hp;
    public int atkDmg;
    public int speed;
    public int ablPwr;
    public int ablCd;

    public Attack attack;
    public CardText cardText;
    
    public AbilityType abilityType;
    public Group group;

    // public Terrain terrainPref;
}

public enum Group {
    None,
    Forge,
    Artillery,
    Plant,
    Evergreen,
}

[Serializable]
public struct Attack {
    public AttackType attackType;
    public IAttack attackFunc;
    public TargetType targetType;
}

[Serializable]
public class CardText {
    public string text;
    public EventID condition;
    public Effect effect;
    public TargetArgs targetArgs;
    public int numTargetTimes = 1;          // used for Random, Spells

    public CardText(CardText cardText) {
        condition = cardText.condition;
        effect = new Effect(cardText.effect);
        
        targetArgs.targetType = cardText.targetArgs.targetType;
        targetArgs.originSlot = cardText.targetArgs.originSlot;
        targetArgs.targetSlotState = cardText.targetArgs.targetSlotState;
        targetArgs.targetSameTeam = cardText.targetArgs.targetSameTeam;
        targetArgs.targetGroup = cardText.targetArgs.targetGroup;
        
        numTargetTimes = cardText.numTargetTimes;
        text = cardText.text;
    }
    
    public CardText(Effect effect) { this.effect = effect; }
}
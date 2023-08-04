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
    public EventID condition;
    public Effect effect;
    public TargetType targetType;
    public TargetType auraTargetType;
    public int numTargetTimes = 1;          // used for Random, Spells
    public Group targetGroup;       // optional
    public string text;

    public CardText(CardText cardText) {
        condition = cardText.condition;
        effect = new Effect(cardText.effect);
        targetType = cardText.targetType;
        auraTargetType = cardText.auraTargetType;
        numTargetTimes = cardText.numTargetTimes;
        targetGroup = cardText.targetGroup;
        text = cardText.text;
    }
    
    public CardText(Effect effect) { this.effect = effect; }
}
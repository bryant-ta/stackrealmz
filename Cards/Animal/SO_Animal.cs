using System;
using UnityEngine;

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
    public int numTargets = 1;          // used for Random, Spells
    public Group targetGroup;       // optional
    public string text;
}
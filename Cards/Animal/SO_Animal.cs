using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Animal")]
public class SO_Animal : SO_Card {
    public int manaCost;
    public int hp;
    public int atkDmg;
    public int atkSpd;
    public int ablPwr;
    public int ablCd;

    public AttackType atkType;
    public AbilityType abilityType;

    public string text;
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
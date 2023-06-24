using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Animal")]
public class SO_Animal : SO_Card {
    public int hp;
    public int atkDmg;
    public int atkSpd;
    public int ablPwr;
    public int ablCd;

    public AttackType atkType;
    public AbilityType ablType;
    
    public Terrain terrainPref;
}
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Animal")]
public class SO_Animal : SO_Card {
    public int hp;
    public int atkDmg;
    public float atkSpd;

    public Terrain terrainType;
    
    public int consumption;
}
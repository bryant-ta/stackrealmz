using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Interactions;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "Cards/Animal")]
public class SO_Animal : SO_Card {
    public int hp;
    public int atkDmg;
    public float atkSpd;

    public Terrain terrainType;
    
    public int consumption;

    
    // public AttackEvent AttackAction;
    //
    // public void Attack() {
    //     AttackAction.Invoke();
    // }


    [SerializeField] private Object attackTypeAsset;
    public IAttack attackType;
    public void SetAttackTypeAsset(Object asset)
    {
        attackTypeAsset = asset;
        attackType = asset as IAttack;
    }

    public void Attack()
    {
        attackType = attackTypeAsset as IAttack;
        if (attackType != null)
        {
            attackType.Attack();
        }
    }

    // public AttackHandler Attack;
}

public interface IAttack {
    void Attack();
}

// public class StandardAttack : IAttack {
//     public void Attack() { }
//     
// }

[Serializable]
public class AttackEvent : UnityEvent {
    
}

// public abstract class AttackHandler : MonoBehaviour, IAttack {
//     public IAttack attack;
//     public abstract void Attack();
// }
//
// public class StandardAttack : AttackHandler {
//     public override void Attack() {
//         
//     }
// }
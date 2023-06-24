using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    [SerializeField] public Stat atkDmg;
    [SerializeField] public Stat atkSpd;
    [SerializeField] public Stat ablPwr;
    [SerializeField] public Stat ablCd;

    IAttack attack;
    CombatTicker attackTicker;
    IAbility ability;
    CombatTicker abilityTicker;

    public bool isInCombat;

    void Start() {
        Setup(animalData.name, animalData.value, animalData.image);
        atkDmg = new Stat(animalData.atkDmg);
        atkSpd = new Stat(animalData.atkSpd);
        ablCd = new Stat(animalData.ablCd);
        ablPwr = new Stat(animalData.ablPwr);

        attack = AttackTypeLookUp.LookUp[animalData.atkType];
        ability = AbilityTypeLookUp.LookUp[animalData.ablType];
        
        EventManager.Subscribe(gameObject, EventID.Death, () => StartCoroutine(Death()));
        EventManager.Subscribe(gameObject, EventID.AttackReady, () => StartCoroutine(Attack()));
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, Ability);
    }

    IEnumerator Attack() {
        CombatSlot curCombatSlot = mSlot as CombatSlot;
        if (!curCombatSlot) {
            Debug.LogErrorFormat("Current slot of %s is not type CombatSlot. Attack failed.", name);
            yield break;
        }

        int terrainModifier = 1;
        if (curCombatSlot.terrain == animalData.terrainPref) { terrainModifier = 2; }   // TEMP: temp value
        int dmg = atkDmg.Value * terrainModifier;

        if (!attack.Attack(mSlot, dmg, isEnemy)) {
            // Tries claiming target move slot, then wait for claims to resolve from other cards
            // Interesting mechanic that rose from this: card that didnt get to move loses a turn!
            CombatSlot targetMoveSlot = curCombatSlot.SlotGrid.Forward(curCombatSlot, isEnemy) as CombatSlot;
            if (targetMoveSlot && targetMoveSlot.RegisterMovementClaim(this)) {
                yield return null;
            }

            if (Step(targetMoveSlot)) {       // If did not attack anything, try move forward one space
                attackTicker.forceEndTick = false;
                attack.Attack(mSlot, dmg, isEnemy);
            } else {            // If could not move, set ticker to full to try attack or move next tick
                attackTicker.SetCurTick(attackTicker.EndTick);
            }
        }
    }

    bool Step(CombatSlot targetMoveSlot) {
        if (targetMoveSlot && targetMoveSlot.CheckMovementClaim(this) && targetMoveSlot.PlaceAndMove(mStack)) {
            return true;
        }
        return false;
    }

    public void Ability() {
        if (mSlot && abilityTicker.Ready()) {
            abilityTicker.Reset();
            abilityTicker.Start();
            ability.Use(mSlot, ablPwr.Value);
        }
    }

    // Death delays a death cleanup until the next frame, to allow finishing code execution
    IEnumerator Death() {
        yield return null;
        
        mSlot.PickUp();     // Note: calls child class PickUp (i.e. CombatSlot.Pickup) if mSlot is actually child class
        if (isEnemy) {
            EventManager.Invoke(WaveManager.Instance.gameObject, EventID.EnemyDied);
        }
        Destroy(gameObject);
    }
    
    public void StartCombatState() {
        // TODO: ensure atkSpd/ablCd correctly updates tickers
        isInCombat = true;
        
        attackTicker = new CombatTicker(gameObject, EventID.AttackTick, EventID.AttackReady, atkSpd.Value);
        abilityTicker = new CombatTicker(gameObject, EventID.AbilityTick, EventID.AbilityReady, ablCd.Value, false);

        EventManager.Invoke(gameObject, EventID.EnterCombat);
    }
    public void EndCombatState() {
        isInCombat = false;
        
        attackTicker.Stop();
        abilityTicker.Stop();
        
        EventManager.Invoke(gameObject, EventID.ExitCombat);
    }

    void OnEnable() {
        StartCoroutine(RegisterAnimal());
    }
    void OnDisable() {
        if (isEnemy) GameManager.Instance.enemies.Remove(this);
        else GameManager.Instance.animals.Remove(this);
        
        if (attackTicker != null) attackTicker.Stop();
        if (abilityTicker != null) abilityTicker.Stop();
    }

    IEnumerator RegisterAnimal() {
        yield return null;      // required for isEnemy to be set during CardFactory.CreateAnimal()
        if (isEnemy) GameManager.Instance.enemies.Add(this);
        else GameManager.Instance.animals.Add(this);
    }

    // Old but keeping for solution ideas:
    //
    // This seems silly... having to make a func for every ticker just to trigger in EventManager
    // consider alternatives like using string instead of enum for EventManager or letting AnimalUI ref Animal always
    // and public CombatTickers (but this means no EventManager exposing? maybe not good bc other things might want to know
    // when this attacks...)
    // void AttackTick(int tick) {
    //     EventManager.TriggerEvent(gameObject, EventName.OnAttackTick, tick);
    // }
}
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    [SerializeField] public Stat atkDmg;
    [SerializeField] public Stat atkSpd;
    // [SerializeField] public Stat ablPwr;
    // [SerializeField] public Stat ablCd;

    IAttack attack;
    CombatTicker attackTicker;
    IAbility ability;
    CombatTicker abilityTicker;

    public bool isEnemy;
    public bool isInCombat;

    new void Start() {
        Setup(animalData);
        atkDmg = new Stat(animalData.atkDmg);
        atkSpd = new Stat(animalData.atkSpd);
        // ablCd = new Stat(animalData.ablCd);
        // ablPwr = new Stat(animalData.ablPwr);

        attack = AttackTypeLookUp.LookUp[animalData.atkType];
        ability = AbilityTypeLookUp.LookUp[animalData.abilityType];
        
        EventManager.Subscribe(gameObject, EventID.Death, Death);
        EventManager.Subscribe(gameObject, EventID.AttackReady, Attack);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, Ability);
    }

    void Attack() {
        int terrainModifier = 1;
        CombatSlot curCombatSlot = mSlot as CombatSlot;
        if (!curCombatSlot) {
            Debug.LogErrorFormat("Current slot of %s is not type CombatSlot. Attack failed.", name);
            return;
        }

        // if (curCombatSlot.terrain == animalData.terrainPref) { terrainModifier = 2; }   // TEMP: temp value
        int dmg = atkDmg.Value * terrainModifier;

        if (!attack.Attack(mSlot, dmg, isEnemy)) {
            attackTicker.Start();   // hit nothing
        } else {
            attackTicker.Reset();   // hit something, reset timer
            attackTicker.Start();
        }
    }

    void Step() {
        CombatSlot curCombatSlot = mSlot as CombatSlot;
        CombatSlot targetMoveSlot = curCombatSlot.SlotGrid.Forward(curCombatSlot, isEnemy) as CombatSlot;
        if (targetMoveSlot) {
            targetMoveSlot.PlaceAndMove(mStack);
        }
    }

    void Ability() {
        // if (isInCombat && mSlot && abilityTicker.Ready()) {
        //     abilityTicker.Reset();
        //     abilityTicker.Start();
        //     ability.Use(mSlot, ablPwr.Value);
        // }
    }

    void Death() {
        print("Ahhh I ded");

        StartCoroutine(DestroyNextFrame());
    }

    IEnumerator DestroyNextFrame() {
        yield return null;
        
        mSlot.PickUp();
        if (isEnemy) {
            EventManager.Invoke(WaveManager.Instance.gameObject, EventID.EnemyDied);
        }
        
        Destroy(gameObject);
        mStack.ExtractWithoutCraft(this);
    }
    
    public void StartCombatState() {
        // TODO: ensure atkSpd/ablCd correctly updates tickers
        isInCombat = true;
        
        attackTicker = new CombatTicker(gameObject, EventID.AttackTick, EventID.AttackReady, atkSpd.Value, false);
        // abilityTicker = new CombatTicker(gameObject, EventID.AbilityTick, EventID.AbilityReady, ablCd.Value, false);

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
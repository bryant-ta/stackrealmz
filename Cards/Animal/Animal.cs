using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    public Health health;
    public Stat manaCost;
    public Stat atk;
    public Stat spd;
    // public Stat ablPwr;
    // public Stat ablCd;

    public Attack attack;
    public CardText cardText;
    public Group group;

    CombatTicker attackTicker;
    IAbility ability;
    CombatTicker abilityTicker;

    public bool isEnemy;
    public bool isInCombat;

    public EffectController EffectCtrl => effectCtrl;
    EffectController effectCtrl;

    bool didStart = false;
    public new void Start() {
        if (didStart) return; // useful when need to manually call Start before other initialization code in same frame (e.g. StartCombatState)
        didStart = true;
        
        Setup(animalData);
        manaCost = new Stat(animalData.manaCost);
        atk = new Stat(animalData.atk);
        spd = new Stat(animalData.spd);
        // ablCd = new Stat(animalData.ablCd);
        // ablPwr = new Stat(animalData.ablPwr);

        cardText = new CardText(animalData.cardText);
        foreach (var e in cardText.effects) {
            e.effectFunc = EffectTypeLookUp.CreateEffect(e.effectType);
            e.source = this;
        }
        group = animalData.group;
        
        attack = animalData.attack;
        attack.attackFunc = AttackTypeLookUp.LookUp[attack.attackType];
        
        ability = AbilityTypeLookUp.LookUp[animalData.abilityType];

        effectCtrl = GetComponent<EffectController>();
        if (!effectCtrl) Debug.LogError("No EffectController found for this Animal");

        EventManager.Subscribe(gameObject, EventID.Death, Death);
        EventManager.Subscribe(gameObject, EventID.AttackReady, QueueAttack);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, Ability);

        EventManager.Subscribe(gameObject, cardText.condition, TriggerCardText);
    }

    public void Play() {
        EventManager.Invoke(gameObject, EventID.CardPlayed);
    }

    void TriggerCardText() {
        foreach (Effect e in cardText.effects) { // if EffectType.None exists in card effects, do not trigger any effects
            if (e.effectType == EffectType.None) return;
        }
        
        EventManager.Invoke(gameObject, cardText.condition, new EffectOrder() {
            originSlot = mCombatSlot,
            cardText = cardText,
        });
    }
    
    public void StartCombatState() {
        // TODO: ensure atkSpd/ablCd correctly updates tickers
        isInCombat = true;
        
        
        attackTicker = new CombatTicker(gameObject, EventID.AttackTick, EventID.AttackReady, spd.Value, false);
        // abilityTicker = new CombatTicker(gameObject, EventID.AbilityTick, EventID.AbilityReady, ablCd.Value, false);

        ExecutionManager.Instance.RegisterEffectOrder(this, cardText.condition);
        EventManager.Invoke(gameObject, EventID.EnterCombat);
    }
    public void EndCombatState() {
        isInCombat = false;
        
        attackTicker.Stop();
        // abilityTicker.Stop();
        
        EventManager.Invoke(gameObject, EventID.ExitCombat);
    }

    void QueueAttack() { ExecutionManager.Instance.QueueAttackOrder(new AttackOrder { priority = mCombatSlot.executionPriority, animal = this }); }
    public void Attack() {
        int terrainModifier = 1;
        CombatSlot curCombatSlot = mCombatSlot;
        if (!curCombatSlot) {
            Debug.LogErrorFormat("Current slot {0} is not type CombatSlot. SendAttack failed.", name);
            return;
        }

        // if (curCombatSlot.terrain == animalData.terrainPref) { terrainModifier = 2; }   // TEMP: temp value
        int dmg = atk.Value * terrainModifier;
            
        if (attack.attackFunc.Attack(attack.targetType, mCombatSlot, dmg)) {
            attackTicker.Reset(); // hit something, reset timer
            attackTicker.Start();
            
            EventManager.Invoke(gameObject, EventID.Attack);
        }
    }

    // void Step() {
    //     CombatSlot curCombatSlot = mCombatSlot;
    //     CombatSlot targetMoveSlot = curCombatSlot.SlotGrid.Forward(curCombatSlot, isEnemy) as CombatSlot;
    //     if (targetMoveSlot) {
    //         targetMoveSlot.PlaceAndMove(mStack);
    //     }
    // }

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
        
        if (mSlot) {
            mSlot.PickUpHeld();
        }
        if (isEnemy) {
            EventManager.Invoke(WaveManager.Instance.gameObject, EventID.EnemyDied);
        }
        if (mStack) {
            mStack.ExtractWithoutCraft(this);
        }

        Destroy(gameObject);
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
        yield return null;      // required for isEnemy to be set
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
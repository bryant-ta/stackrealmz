using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Animal : Card {
    public SO_Animal animalData;

    public Health health;
    public Stat manaCost;
    public Stat atkDmg;
    public Stat speed;
    // public Stat ablPwr;
    // ublic Stat ablCd;

    public Attack attack;
    public CardText cardText;

    CombatTicker attackTicker;
    IAbility ability;
    CombatTicker abilityTicker;

    public bool isEnemy;
    public bool isInCombat;

    public EffectController EffectCtrl => effectCtrl;
    EffectController effectCtrl;

    new void Start() {
        Setup(animalData);
        manaCost = new Stat(animalData.manaCost);
        atkDmg = new Stat(animalData.atkDmg);
        speed = new Stat(animalData.speed);
        // ablCd = new Stat(animalData.ablCd);
        // ablPwr = new Stat(animalData.ablPwr);

        cardText = animalData.cardText;
        cardText.effect.effectFunc = EffectTypeLookUp.LookUp[cardText.effect.effectType];

        attack = animalData.attack;
        attack.attackFunc = AttackTypeLookUp.LookUp[attack.attackType];
        
        ability = AbilityTypeLookUp.LookUp[animalData.abilityType];

        effectCtrl = GetComponent<EffectController>();
        if (!effectCtrl) Debug.LogError("No EffectController found for this Animal");
        
        EventManager.Subscribe(gameObject, EventID.Death, Death);
        EventManager.Subscribe(gameObject, EventID.AttackReady, Attack);
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, Ability);
        
        EventManager.Subscribe(gameObject, cardText.condition, TriggerCardText);
    }

    public void Play() {
        EffectManager.Instance.RegisterEffectOrder(this, cardText.condition);
        EventManager.Invoke(gameObject, EventID.CardPlayed);
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

        if (!attack.attackFunc.Attack(attack.targetType, mSlot as CombatSlot, dmg, isEnemy)) {
            attackTicker.Start();   // hit nothing
        } else {
            attackTicker.Reset();   // hit something, reset timer
            attackTicker.Start();
        }
    }

    void TriggerCardText() {
        EventManager.Invoke(gameObject, cardText.condition, new EffectOrder() {
            originSlot = mSlot as CombatSlot,
            cardText = cardText,
        });
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

        if (mSlot) {
            mSlot.PickUp();
        }

        if (isEnemy) {
            EventManager.Invoke(WaveManager.Instance.gameObject, EventID.EnemyDied);
        }
        
        mStack.ExtractWithoutCraft(this);
        Destroy(gameObject);
    }
    
    public void StartCombatState() {
        // TODO: ensure atkSpd/ablCd correctly updates tickers
        isInCombat = true;
        
        attackTicker = new CombatTicker(gameObject, EventID.AttackTick, EventID.AttackReady, speed.Value, false);
        // abilityTicker = new CombatTicker(gameObject, EventID.AbilityTick, EventID.AbilityReady, ablCd.Value, false);

        EventManager.Invoke(gameObject, EventID.EnterCombat);
    }
    public void EndCombatState() {
        isInCombat = false;
        
        attackTicker.Stop();
        // abilityTicker.Stop();
        
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
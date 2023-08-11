using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExecutionManager : MonoBehaviour {
    public static ExecutionManager Instance => _instance;
    static ExecutionManager _instance;

    public Transform returnPoint;

    [SerializeField] SlotGrid combatGrid;
    List<EffectOrder> effectOrders = new List<EffectOrder>();
    List<AttackOrder> attackOrders = new List<AttackOrder>();

    void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.StartBattle, EnableTriggerEffects);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.LostBattle, DisableTriggerEffects);
        EventManager.Subscribe(WaveManager.Instance.gameObject, EventID.WonBattle, DisableTriggerEffects);
        
        CombatClock.onTick.AddListener(DoTriggerAttackOrder);
    }

    void Update() {
        TriggerEffectOrders(); 
    }

    /****************   Effect Order Management   ****************/

    public void RegisterEffectOrder(Animal origin, EventID condition) {
        EventManager.Subscribe<EffectOrder>(origin.gameObject, condition, QueueEffectOrder);
    }

    public void QueueAttackOrder(AttackOrder ao) { attackOrders.Add(ao); }
    public void QueueEffectOrder(EffectOrder effectOrder) { effectOrders.Add(effectOrder); }
    
    void SortAttackOrders() { attackOrders.Sort((a, b) => a.priority.CompareTo(b.priority)); }
    // TODO: Move executeDurationEffects loop to here to do them in priority order
    void SortEffectOrders() { }
    
    /****************   Attack Execution   ****************/

    void DoTriggerAttackOrder() { StartCoroutine(TriggerAttackOrders()); }
    IEnumerator TriggerAttackOrders() {
        yield return null;
        
        SortAttackOrders();
        
        foreach (AttackOrder ao in attackOrders) {
            if (!ao.animal) {
                continue;
            }
            ao.animal.Attack();

            yield return new WaitForSeconds(0.25f);
        }
        
        attackOrders.Clear();
    }

    /****************   Effect Execution   ****************/

    void EnableTriggerEffects() {
        // CombatClock.onTick.AddListener(TriggerEffectOrders);
        CombatClock.onTick.AddListener(ExecuteDurationEffects);
    }
    void DisableTriggerEffects() {
        // CombatClock.onTick.RemoveListener(TriggerEffectOrders); 
        CombatClock.onTick.RemoveListener(ExecuteDurationEffects);
        effectOrders.Clear();
    }

    void TriggerEffectOrders() {
        foreach (EffectOrder eo in effectOrders) {
            // Get targets
            List<CombatSlot> targetSlots = new List<CombatSlot>();
            for (int i = 0; i < eo.cardText.targetArgs.numTargetTimes; i++) {
                List<CombatSlot> t = TargetTypes.GetTargets(new TargetArgs() {
                    targetType = eo.cardText.targetArgs.targetType,
                    originSlot = eo.originSlot,
                    targetSlotState = eo.cardText.targetArgs.targetSlotState,
                    targetSameTeam = eo.cardText.targetArgs.targetSameTeam,
                    targetGroup = eo.cardText.targetArgs.targetGroup,
                    numTargetTimes = eo.cardText.targetArgs.numTargetTimes,
                });
                if (t == null || t.Count == 0) continue;

                targetSlots = targetSlots.Concat(t).ToList();
            }

            // Apply effects
            Animal effectOrigin = eo.originSlot.Animal;
            foreach (Effect e in eo.cardText.effects) {
                if (e.effectType == EffectType.Summon) {
                    ExecuteSummonEffect(e, targetSlots);
                } else if (e.effectType == EffectType.Return) {
                    ExecuteReturnEffect(targetSlots);
                } else if (e.effectPermanence == EffectPermanence.Aura) {
                    effectOrigin.EffectCtrl.AddAuraEffect(e);
                } else {
                    for (int i = 0; i < targetSlots.Count; i++) {
                        if (!targetSlots[i].IsEmpty()) {
                            targetSlots[i].Animal.EffectCtrl.AddEffect(e);
                        }

                        print("activating effect: " + e.name);
                    }
                }
            }
        }

        effectOrders.Clear();
    }

    // ExecuteDurationEffects handles executing duration effects on cards in play.
    // Currently prioritizes top to bottom, left to right
    void ExecuteDurationEffects() {
        for (int x = 0; x < combatGrid.Width; x++) {
            for (int y = combatGrid.Height - 1; y >= 0; y--) {
                CombatSlot c = combatGrid.SelectSlot(new Vector2Int(x, y), false) as CombatSlot;
                if (!c || c.IsEmpty()) continue;

                foreach (Effect e in c.Animal.EffectCtrl.durationEffects.ToList()) {
                    print(e.name + " " + e.remainingDuration);
                    e.effectFunc.Apply(c.Animal.mSlot as CombatSlot, new EffectArgs() {val = e.baseValue});
                    
                    if (e.remainingDuration > 0) e.remainingDuration--;

                    if (e.remainingDuration == 0) c.Animal.EffectCtrl.RemoveEffect(e);
                }
            }
        }
    }

    // ExecuteSummonEffect handles spawning new cards in combat from summon effects.
    // - Spawn location is determined by TargetType
    // - Prioritizes spawn slots based on list order
    // - If there are no more valid spawn slots, then chooses a random adjacent empty slot. If there is none, nothing will be spawned.
    void ExecuteSummonEffect(Effect effect, List<CombatSlot> spawnSlots) {
        foreach (CombatSlot spawnSlot in spawnSlots) {
            Stack s = spawnSlot.SpawnCard(effect.summonData);
            if (!s) {
                Debug.LogError("ExecuteSummonEffect: tried spawning in non-empty slot");
            }
        }
    }
    
    // ExecuteReturnEffect handles removing cards from combat.
    public void ExecuteReturnEffect(List<CombatSlot> targetSlots) {
        // Gather all player animals on combat grid into one stack
        Stack gatheredStack = null;
        bool first = true;
        foreach (CombatSlot combatSlot in targetSlots) {
            Animal animal = combatSlot.Animal;
            if (animal) {
                if (GameManager.Instance.animals.Contains(animal)) {
                    Transform stackTransform = combatSlot.PickUpHeld(false, true);
                    
                    if (animal.EffectCtrl.FindEffect(EffectType.Vanish) != null) {
                        Destroy(stackTransform.gameObject);
                        continue;
                    }
                    
                    if (first) {
                        first = false;
                        gatheredStack = animal.mStack;
                        continue;
                    }

                    animal.mStack.PlaceAll(gatheredStack);
                    StartCoroutine(Utils.MoveCardToPoint(animal, gatheredStack.CalculateStackPosition(animal)));
                } else {
                    Transform stackTransform = combatSlot.PickUpHeld(false, true);
                    Destroy(stackTransform.gameObject);
                }
            }
        }

        if (gatheredStack) {
            StartCoroutine(Utils.MoveStackToPoint(gatheredStack, returnPoint.position));
        }
    }
}

public struct EffectOrder {
    public CombatSlot originSlot;
    public CardText cardText;
}

public struct AttackOrder {
    public int priority;
    public Animal animal;
}
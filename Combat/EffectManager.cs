using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager : MonoBehaviour {
    public static EffectManager Instance => _instance;
    static EffectManager _instance;

    [SerializeField] SlotGrid combatGrid;
    List<EffectOrder> effectOrders = new List<EffectOrder>();

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
    }

    void Update() {
        TriggerEffectOrders();
    }

    /****************   Effect Order Management   ****************/
    
    public void RegisterEffectOrder(Animal origin, EventID condition) {
        EventManager.Subscribe<EffectOrder>(origin.gameObject, condition, AddEffectOrder);
    }

    public void AddEffectOrder(EffectOrder effectOrder) {
        effectOrders.Add(effectOrder);
    }
    
    // TODO: Move executeDurationEffects loop to here to do them in priority order
    // TODO: sort based on origin location or assigned effect priority for consistent effect resolution
    void SortEffectOrders() {
        
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
            List<CombatSlot> targetSlots = new List<CombatSlot>();
            for (int i = 0; i < eo.cardText.numTargetTimes; i++) {
                List<CombatSlot> t = TargetTypes.GetTargets(eo.cardText.targetType, eo.originSlot, eo.cardText.targetGroup);
                if (t == null || t.Count == 0) continue;

                targetSlots = targetSlots.Concat(t).ToList();
            }

            // Effect Execution
            
            Effect e = eo.cardText.effect;
            Animal effectOrigin = eo.originSlot.Animal;
            if (e.effectType == EffectType.SummonEffect) {
                // add empty adjacent slots as backup spawn slots
                targetSlots = targetSlots.Concat(TargetTypes.GetTargets(TargetType.EmptyAdjacent, eo.originSlot)).ToList();
                ExecuteSummonEffect(eo.cardText.effect, targetSlots);
            } else if (e.effectPermanence == EffectPermanence.Aura) {
                effectOrigin.EffectCtrl.AddAuraEffect(e);
            } else {
                for (int i = 0; i < targetSlots.Count; i++) {
                    if (!targetSlots[i].IsEmpty()) {
                        targetSlots[i].Animal.EffectCtrl.AddEffect(e);
                    }

                    print("activating effect: " + eo.cardText.effect.name);
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
                    e.effectFunc.Apply(c.Animal.mSlot as CombatSlot, new EffectArgs() { val = e.baseValue });
                    e.remainingDuration--;

                    if (e.remainingDuration == 0) {
                        c.Animal.EffectCtrl. RemoveEffect(e);
                    }
                }
            }
        }
    }
    
    // ExecuteSummonEffects handles spawning new cards in combat from summon effects.
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
}

public struct EffectOrder {
    public CombatSlot originSlot;
    public CardText cardText;
}
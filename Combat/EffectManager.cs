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
            List<Animal> targets = new List<Animal>();
            for (int i = 0; i < eo.cardText.numTargets; i++) {
                List<Animal> t = TargetTypes.GetTargets(eo.cardText.targetType, eo.originSlot, eo.cardText.targetGroup);
                if (t == null || t.Count == 0) continue;

                targets = targets.Concat(t).ToList();
            }
            
            for (int i = 0; i < targets.Count; i++) {
                targets[i].EffectCtrl.AddEffect(eo.cardText.effect);
                print("activating effect: " + eo.cardText.effect.name);
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
                    e.effectFunc.Execute(c.Animal, e.baseValue);
                    e.remainingDuration--;

                    if (e.remainingDuration == 0) {
                        c.Animal.EffectCtrl. RemoveEffect(e);
                    }
                }
            }
        }
    }
}

public struct EffectOrder {
    public CombatSlot originSlot;
    public CardText cardText;
}
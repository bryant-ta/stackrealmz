 using System;
 using System.Collections.Generic;
 using UnityEditor;
 using UnityEngine;
 using UnityEngine.UI;

 public class RollManager : MonoBehaviour {
     public int rollPrice;
     public List<Drop> rollTableA = new List<Drop>();
     public List<Drop> rollTableB = new List<Drop>();
     
     public SlotGrid rollSlotGrid;
     public Button rollButtonA;
     public Button rollButtonB;

     void Start() {
         LoadRealmCards(Realm.Fire, rollTableA);
         // LoadRealmCards(Realm.Earth, rollTableB);
         
         foreach (Slot slot in rollSlotGrid.slotGrid) {
             EventManager.Subscribe<Slot>(slot.gameObject, EventID.SlotPickedUp, CleanUpCards);
         }
     }

     void LoadRealmCards(Realm realm, List<Drop> rollTable) {
         Drop commonPool = new Drop {
             cardDropsPool = new List<SO_Card>(),
             percentage = 100
         };
         Drop rarePool = new Drop {
             cardDropsPool = new List<SO_Card>(),
             percentage = 25
         };
         
         SO_Card[] cards = Resources.LoadAll<SO_Card>("Animals");
         foreach (SO_Card card in cards) {
             if (card != null && card.realm == realm) {
                 switch (card.rarity) {
                     case Rarity.Common:
                         commonPool.cardDropsPool.Add(card);
                         break;    
                     case Rarity.Rare:
                         rarePool.cardDropsPool.Add(card);
                         break;
                     default:
                         continue;
                 }
             }
         }

         // TODO: something more elegant for assigning percentage to rarity? then can make drops on the fly
         if (commonPool.cardDropsPool.Count > 0) rollTable.Add(commonPool);
         if (rarePool.cardDropsPool.Count > 0 ) rollTable.Add(rarePool);
     }

     void RollRealm(List<Drop> rollTable) {
         if (GameManager.Instance.ModifyMoney(-rollPrice)) {
             CleanUpCards();
             
             foreach (Slot slot in rollSlotGrid.slotGrid) {
                 SO_Card cSO = CardFactory.RollDrop(rollTable);

                 slot.SpawnCard(cSO);
             }
         }
     }

     // Clears card roll slots. Skips input slot if provided, used to ignore slot that was already taken from.
     void CleanUpCards(Slot skipSlot = null) {
         foreach (Slot slot in rollSlotGrid.slotGrid) {
             if (slot == skipSlot) continue;
             
             // do not reinvoked pickup event since that triggers this func
             Transform slotStackTransform = slot.PickUpHeld(false, false, false);
             if (slotStackTransform) {
                 Destroy(slotStackTransform.gameObject);
             }
         }
     }

     void OnEnable() {
         rollButtonA.onClick.AddListener(() => RollRealm(rollTableA));
         rollButtonB.onClick.AddListener(() => RollRealm(rollTableB));
     }
     void OnDisable() {
         rollButtonA.onClick.RemoveAllListeners();
         rollButtonB.onClick.RemoveAllListeners();
     }
 }

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
         LoadRealmCards(Realm.Earth, rollTableB);
         
         foreach (Slot slot in rollSlotGrid.slotGrid) {
             EventManager.Subscribe(slot.gameObject, EventID.SlotPickedUp, CleanUpCards);
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
         
         string[] assetPaths = AssetDatabase.FindAssets("t:SO_Animal", new[] { Constants.AnimalDataPath });
         foreach (string path in assetPaths) {
             string assetPath = AssetDatabase.GUIDToAssetPath(path);
             SO_Card card = AssetDatabase.LoadAssetAtPath<SO_Card>(assetPath);

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

                 Stack s = CardFactory.CreateStack(slot.transform.position, cSO);
                 slot.PlaceAndMove(s);
             }
         }
     }

     // Clears card roll slots. Returns true if a card was already taken from the roll grid.
     void CleanUpCards() {
         foreach (Slot slot in rollSlotGrid.slotGrid) {
             // do not reinvoked pickup event since that triggers this func
             Transform slotStackTransform = slot.PickUp(false, false);
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

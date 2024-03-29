using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stack : MonoBehaviour {
    [SerializeField] List<Card> stack = new List<Card>();

    [SerializeField] GameObject craftProgressBar;
    [SerializeField] bool isChanged;
    public bool isLocked;

    public int craftingSnapRange;

    void Start() {
        EventManager.Subscribe(gameObject, EventID.CraftDone, TryCraft);
    }

    public void Place(Card card) {
        AddCard(card);
        isChanged = true;
        TryCraft();
    }

    // null input treated as placing stack on nothing
    public void PlaceAll(Stack destStack) {
        if (destStack == null) {
            TryCraft();
            return;
        }

        MoveCardRangeTo(destStack, 0, stack.Count);
        destStack.isChanged = true;
        destStack.TryCraft();
    }
    
    public Transform PickUp(Card card) {
        isChanged = true;
        if (card == stack.First()) {
            return transform;
        } else {
            Transform newStack = SplitStack(card).transform;
            TryCraft();
            return newStack;
        }
    }

    public Transform ExtractWithoutCraft(Card card) {
        isChanged = true;
        RemoveCard(card);
        return card.transform;
    }

    // SplitStack returns a new stack (+object) containing all cards from input card to end of current stack
    // - Sets each cards mStack reference
    Transform SplitStack(Card card) {
        int splitIndex = stack.IndexOf(card);
        Stack newStack = CardFactory.CreateStack(stack[splitIndex].transform.position);

        MoveCardRangeTo(newStack, splitIndex, stack.Count - splitIndex);

        return newStack.transform;
    }

    // Tries crafting with current stack. This should always be only func called when crafting
    void TryCraft() {
        if (stack.Count > 1) {
            StartCoroutine(Craft());
        }
    }
    
    // Real TODO: This is shit, think of a better spell creation system or dont do at all
    // // TODO: determine if dictionary or using SpellMaterial component composition is better (this is simpler for simple system)
    // Dictionary<string, CardText> spellMaterialCardTexts = new Dictionary<string, CardText>();
    // Recipe CraftSpell(List<string> cardNames) {
    //     SO_Spell spellData = new SO_Spell();
    //     spellData.spellType = SpellType.Effect;
    //     
    //     Spell s = CardFactory.CreateBaseSpell(GetTopCard().transform.position);
    //     
    //     foreach (string cardName in cardNames) {
    //         if (spellMaterialCardTexts.TryGetValue(cardName, out CardText cardText)) { // Get spell element-effect
    //             CardText cardTextCopy = new CardText(cardText);
    //             foreach (Effect effect in cardTextCopy.effects) {
    //                 effect.source = s;
    //             }
    //         } 
    //     }
    //
    //     Recipe recipe = new Recipe();
    //     return recipe;
    // }

    // must be coroutine for pausing on DoCraftTime
    IEnumerator Craft() {
        yield return null;  // required for isChanged to register correctly and interrupt crafting
        isChanged = false;
        
        // TODO: TEMP: This is temp way to apply food affects to animals
        if (stack.Last() is Food f && stack.First() is Animal a && stack.Count == 2) {
            a.GetComponent<EffectController>().AddEffect(f.foodEffect);
            RemoveCard(f);
            Destroy(f.gameObject);
            yield break;
        }
        // END TEMP
        
        List<string> cardNames = GetCardsNamesInStack();
        
        // CraftSpell
        
        Recipe validRecipe = CardFactory.LookupRecipe(cardNames);
        if (validRecipe == null) { yield break; }
        
        int craftFinished = 0;
        // Delegate shortened syntax for returning a value from coroutine
        yield return StartCoroutine(DoCraftTime(validRecipe.craftTime, (ret) => {
            craftFinished = ret;
        }));

        if (craftFinished == 1 && !isChanged) {
            // Create recipe products
            if (validRecipe.randomProducts.Count > 0) {
                for (int i = 0; i < validRecipe.numRandomProducts; i++) {
                    SO_Card cSO = CardFactory.RollDrop(validRecipe.randomProducts);
                    if (cSO == null) {
                        continue;
                    }
                    CreateCraftedProduct(cSO, i, validRecipe.numRandomProducts);
                }
            } else {
                for (int i = 0; i < validRecipe.products.Count; i++) {
                    CreateCraftedProduct(validRecipe.products[i], i, validRecipe.products.Count);
                }
            }
            
            // Clean up used materials
            if (validRecipe.reusableMaterials.Length > 0) {     // Keep cards marked by recipe
                foreach (Card c in stack.ToList()) {
                    if (validRecipe.reusableMaterials.Contains(c.name)) {
                        continue;
                    }
                    
                    RemoveCard(c);
                    Destroy(c.gameObject);
                }
                RecalculateStackPositions();
                
                EventManager.Invoke(gameObject, EventID.CraftDone);
            } else {                                            // Used up all cards, just destroy whole stack
                // TODO: convert to event
                // Destroys all children too
                Destroy(gameObject);
            }
        }
    }

    void CreateCraftedProduct(SO_Card cSO, int i, int total) {
        if (cSO is SO_Sheet sSO) {
            Sheet sheet = CardFactory.CreateSheet(sSO);
            sheet.transform.position = Utils.GenerateCircleVector(i, total, Constants.CardCreationRadius, transform.position);
            return;
        }
        
        Transform topCardTrans = GetTopCard().transform;
        Collider[] nears = Physics.OverlapBox(topCardTrans.position, Vector3.one * craftingSnapRange, topCardTrans.rotation, 1<<LayerMask.NameToLayer("Card"));
        
        // Find nearest stack with top card matching the product
        float minDistance = int.MaxValue;
        Stack snapStack = null;
        foreach (Collider near in nears) {
            if (near.TryGetComponent(out Card nearCard)) {
                float d = Vector3.Distance(transform.position, nearCard.transform.position);
                if (nearCard.mStack != this && nearCard.mStack.GetTopCard() == nearCard &&
                    nearCard.GetComponent<MoveableCard>().isStackable && nearCard.name == cSO.name && d < minDistance) {
                    minDistance = d;
                    snapStack = nearCard.mStack;
                }
            }
        }
        
        // Create crafted card object
        Stack s = CardFactory.CreateStack(topCardTrans.position, cSO);
        
        // Move to nearest matching stack or create a new one
        if (snapStack) {
            Card newCard = s.GetTopCard();
            s.PlaceAll(snapStack);
            StartCoroutine(Utils.MoveCardToPoint(newCard, snapStack.CalculateStackPosition(newCard)));
            // tends to bug out for recipes that would automatically start crafting (when 2 of something is a recipe)
        } else {
            s.transform.position = Utils.GenerateCircleVector(i, total, Constants.CardCreationRadius, transform.position);
        }
    }

    IEnumerator DoCraftTime(float craftTime, Action<int> onCraftFinished) {
        if (craftTime == 0) {
            onCraftFinished(1);
            yield break;
        }
        
        // TODO: If this slow, make this pooled
        GameObject barObj = Instantiate(craftProgressBar, GameManager.Instance.WorldCanvas.gameObject.transform);
        barObj.transform.position =
            new Vector3(transform.position.x, 0, transform.position.z + 0.75f);

        Image craftProgressFill = barObj.transform.GetChild(0).GetComponent<Image>();
        // Fill bar over <craftTime> seconds. Interrupted by being pickedup and placed on
        while (craftProgressFill.fillAmount < 1 && !isChanged ) {
            craftProgressFill.fillAmount += (float) GameManager.Instance.TimeScale / craftTime * Time.deltaTime;
            yield return null;
        }

        Destroy(barObj);

        if (craftProgressFill.fillAmount >= 1) {
            onCraftFinished(1);
            yield break; // Above line runs delegate function, set craftFinished. Then return immediately
        }

        onCraftFinished(0);
    }

    void MoveCardRangeTo(Stack newStack, int startIndex, int count) {
        List<Card> cards = stack.GetRange(startIndex, count);   // Save copy for adding to newStack, RemoveCard() will delete these cards
        foreach (Card c in cards) {
            RemoveCard(c);
        }
        foreach (Card c in cards) {
            newStack.AddCard(c);
        }
    }
    void AddCard(Card c) {
        stack.Add(c);
        c.mStack = this;
        c.transform.parent = transform;
    }
    void RemoveCard(Card c) {
        stack.Remove(c);
        c.mStack = null;
        c.transform.parent = null;
        if (stack.Count == 0) {
            Destroy(gameObject);
        }
    }

    // GetTopCardObj returns the highest (i.e. has cards under) card in the stack
    public Card GetTopCard() {
        if (stack.Count > 0) return stack.Last();
        return null;
    }
    public List<T> GetComponentsInStack<T>() where T : Component {
        List<T> components = new List<T>();
        foreach (Card card in stack) {
            T component = card.GetComponent<T>();
            if (component != null) {
                components.Add(component);
            } else {
                Debug.LogErrorFormat("Component %s missing from card stack", typeof(T));
            }
        }
    
        return components;
    }
    public List<string> GetCardsNamesInStack() {
        List<string> cardNames = new List<string>();
        foreach (Card c in stack) {
            cardNames.Add(c.name);
        }

        return cardNames;
    }
    public Card GetCardByName(string cardName) {
        foreach (Card c in stack) {
            if (c.name == cardName) {
                return c;
            }
        }

        return null;
    }
    public List<Card> GetStack() {
        return new List<Card>(stack);
    }
    public int GetStackSize() {
        return stack.Count;
    }

    // CalculateStackPosition returns correct card position according to its stack index
    public Vector3 CalculateStackPosition(Card card) {
        int i = stack.IndexOf(card);
        Vector3 test = new Vector3(0, Constants.StackDepthOffset * i, -Constants.StackHeightOffset * i);
        return test;
    }
    // RecalculateStackPositions moves stack cards to their correct positions according to stack indexes
    public void RecalculateStackPositions() {
        foreach (Card c in stack) {
            c.transform.localPosition = CalculateStackPosition(c);
        }
    }
    
    public int TotalValue() {
        int total = 0;
        foreach (Card card in stack) {
            total += card.value;
        }
        return total;
    }
}

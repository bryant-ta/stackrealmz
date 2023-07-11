using System.Collections.Generic;

public class CardPack : Card {
    public SO_CardPack cardPackData;

    public int numCards;
    public List<Drop> dropTable;

    public void Start() {
        Setup(cardPackData);
        numCards = cardPackData.numCards;
        dropTable = cardPackData.dropTable;
        
        EventManager.Subscribe(gameObject, EventID.SecondaryDown, Open);
    }

    // Open card pack, rolling and creating all card drops at once
    void Open() {
        for (int i = 0; i < numCards; i++) {
            SO_Card cSO = CardFactory.RollDrop(dropTable);
            if (cSO == null) {
                continue;
            }
            
            Stack s = CardFactory.CreateStack(Utils.GenerateCircleVector(i, numCards, Constants.CardCreationRadius, transform.position), cSO);
        }
        
        Destroy(gameObject);
    }
}
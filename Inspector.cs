using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inspector : MonoBehaviour {
    public GameObject inspectorObj;
    public TextMeshProUGUI inspectorHeaderText;
    public TextMeshProUGUI inspectorBodyText;
    public GameObject worldBackground; // for hiding recipe viewer, should likley be drag plane
    public GameObject craftsRecipePanel;
    public Transform craftsGrid;

    public GameObject fullCardObj;
    Image fullCardBack;
    Image fullCardImage;
    TextMeshProUGUI fullCardNameText;
    TextMeshProUGUI fullCardManaCostText;
    TextMeshProUGUI fullCardHealthText;
    TextMeshProUGUI fullCardAttackText;
    TextMeshProUGUI fullCardSpeedText;
    TextMeshProUGUI fullCardCardText;

    void Awake() {
        fullCardBack = fullCardObj.transform.Find("Back").GetComponent<Image>();
        fullCardImage = fullCardObj.transform.Find("Image").GetComponent<Image>();
        fullCardNameText = fullCardObj.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        fullCardManaCostText = fullCardObj.transform.Find("ManaCostText").GetComponent<TextMeshProUGUI>();
        fullCardHealthText = fullCardObj.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();
        fullCardAttackText = fullCardObj.transform.Find("AttackText").GetComponent<TextMeshProUGUI>();
        fullCardSpeedText = fullCardObj.transform.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        fullCardCardText = fullCardObj.transform.Find("CardText").GetComponent<TextMeshProUGUI>();
    }

    void Start() {
        EventManager.Subscribe<Card>(gameObject, EventID.TertiaryDown, UpdateInspector);
        EventManager.Subscribe(worldBackground, EventID.TertiaryDown, HideInspector);
    }

    void UpdateInspector(Card targetCard) {
        inspectorObj.SetActive(true);

        string bodyText = "";
        if (targetCard is Animal a) {       
            // Show effects on Animal
            bodyText = a.EffectCtrl.ActiveEffectsToString(); 
            
            // Show full Animal card
            fullCardObj.SetActive(true);
            fullCardImage.sprite = a.animalData.image;
            fullCardNameText.text = a.animalData.name;
            fullCardManaCostText.text = a.animalData.manaCost.ToString();
            fullCardHealthText.text = a.animalData.hp.ToString();
            fullCardAttackText.text = a.animalData.atk.ToString();
            fullCardSpeedText.text = a.animalData.spd.ToString();
            fullCardCardText.text = a.animalData.cardText.text;
        } else { // Show target recipe
            List<Recipe> targetRecipes = CardFactory.LookupRecipesWithProduct(targetCard.cardData);
            foreach (Recipe r in targetRecipes) {
                bodyText += "> ";
                foreach (string s in r.materials) {
                    bodyText += s + ", ";
                }

                bodyText = bodyText.Remove(bodyText.Length - 2);
                bodyText += "\n";
            }
        }

        inspectorHeaderText.text = targetCard.name;
        inspectorBodyText.text = bodyText;
        
        // // TODO: determine if this is even needed, currently broken
        // // Crafts recipe
        // List<Recipe> craftsRecipes = CardFactory.LookupRecipesWithMaterial(targetCard);
        // foreach (Recipe r in craftsRecipes) {
        //     string craftsText = "> ";
        //     foreach (SO_Card s in r.products) {
        //         craftsText += s.name + ", ";
        //     }
        //
        //     craftsText = craftsText.Remove(craftsText.Length - 2);
        //     
        //     TextMeshProUGUI craftsPanelText = Instantiate(craftsRecipePanel, craftsGrid).GetComponent<TextMeshProUGUI>();
        //     craftsPanelText.text = craftsText;
        // }
    }
    void HideInspector() {
        inspectorObj.SetActive(false);
        fullCardObj.SetActive(false);
    }
}

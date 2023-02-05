using UnityEngine;

public class Food : Card
{
    public SO_Food foodData;

    [SerializeField] int saturation;
    
    void Start() {
        Setup(foodData.name, foodData.value, foodData.image);
        saturation = foodData.saturation;
    }

    void OnEnable() {
        GameManager.Instance.foods.Add(this);
    }
    void OnDisable() {
        if (GameManager.Instance != null) {
            GameManager.Instance.foods.Remove(this);
        }
    }
}

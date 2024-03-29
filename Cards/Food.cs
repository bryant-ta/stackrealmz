public class Food : Card {
    public SO_Food foodData;

    public Effect foodEffect;

    new void Start() {
        Setup(foodData);
        foodEffect = foodData.effect;
        foodEffect.effectFunc = EffectTypeLookUp.CreateEffect(foodEffect.effectType);
    }

    void OnEnable() { GameManager.Instance.foods.Add(this); }
    void OnDisable() { GameManager.Instance.foods.Remove(this); }
}
public class Food : Card {
    public SO_Food foodData;

    public int saturation;

    new void Start() {
        Setup(foodData.name, foodData.value, foodData.image);
        saturation = foodData.saturation;
    }

    public void Eat() {
        saturation -= 1;
        if (saturation == 0) {
            if (TryGetComponent(out Moveable moveable)) {
                moveable.PickUp();
            }
            Destroy(gameObject, 0.05f);
        }
    }

    void OnEnable() { GameManager.Instance.foods.Add(this); }
    void OnDisable() { GameManager.Instance.foods.Remove(this); }
}
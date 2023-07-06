public static class Constants {
    public static float StackHeightOffset = 0.2f;       // Card shift within stack
    public static float StackDepthOffset = 0.01f;       // Card depth difference within stack
    public static float CardCreationRadius = 1.2f;      // Distance cards are created after crafting, opening packs
    public static float CardDragSpeed = 30f;            // Speed card follows the mouse when held
    public static float CardMoveSpeed = 30f;            // Speed card snaps to cards, falls to ground

    public static float SheetYLayer = -0.01f;           // Object depth of sheets

    // Path Vars
    public static string CardDataPath = "Assets/SO/Cards/";
    public static string AnimalDataPath = "Assets/SO/Animals/";
    public static string RecipeDataPath = "Assets/SO/Recipes/";
    public static string PacksDataPath = "Assets/SO/CardPacks/";
    
    public static string TestDataPath = "Assets/SO/Test/";
}
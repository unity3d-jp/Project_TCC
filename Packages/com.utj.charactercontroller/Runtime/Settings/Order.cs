
namespace Unity.TinyCharacterController.Utility
{
    public static class Order
    {
        public const int GameObjectPool = -1000;
        public const int IndicatorRegister = -100;
        
        // ----------------------------------
        // early update
        // ----------------------------------

        // Prepare Process
        public const int PrepareEarlyUpdate = -1000;
        
        // ----------------------------------
        // update
        // ----------------------------------

        public const int Check = -200;
        public const int Gravity = -100;
        // 自己完結
        public const int Effect = -50;
        // Control
        public const int Control = 5;
        
        // ----------------------------------
        // 計算結果を反映 ( ExecuteOrder )
        // ----------------------------------

        public const int EarlyUpdateBrain = -101;   // InputSystemより前
        public const int UpdateBrain = 10;          // Brainの更新タイミング
        public const int PostUpdate = 100;          // Brainの後
        public const int UpdateIK = 50;             // Ikの更新タイミング
    }
}
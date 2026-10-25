using UnityEngine;

namespace TokenSurvival {
    [CreateAssetMenu(menuName = "Token Survival/Game Config")]
    public sealed class GameConfig : ScriptableObject {
        public int startingTokens = 0;
        public float startingHunger = 72f;
        public float hungerDrainPerMinute = 5f;
        public int feedCost = 25;
        public int feedAmount = 38;
        public int computerBaseReward = 18;
        public int showcaseBaseReward = 24;
        public int beggingBaseReward = 12;
        public int[] upgradeCosts = { 70, 120, 190 };
        public float playerSpeed = 4.2f;
    }
}

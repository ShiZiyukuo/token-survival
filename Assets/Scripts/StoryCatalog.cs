using System;
using UnityEngine;
namespace TokenSurvival {
    [Serializable] public class StoryLine { public string speaker; [TextArea(2,5)] public string text; public bool whale; }
    [Serializable] public class StoryChapter { public string title, objective; public Vector2 cameraFocus; public StoryLine[] lines; }
    [CreateAssetMenu(menuName="Token Survival/Story Catalog")]
    public sealed class StoryCatalog : ScriptableObject {
        public StoryChapter[] chapters;
        public StoryLine[] bowlAfterLines = {
            new StoryLine { speaker="程序员", text="等等……真的有人打赏了？不是玩笑，Token 真的到账了！" },
            new StoryLine { speaker="DeepSeek 娘", text="咚咚！大家喜欢我们的节奏！我还能继续！", whale=true },
            new StoryLine { speaker="程序员", text="看着你活力满满的样子，我也有干劲了。我们一起攒够 100 Token 吧！" }
        };
    }
}

using UnityEngine;
using UnityEngine.UI;
namespace TokenSurvival {
    // Use the user's installed CJK font. No Windows font files are redistributed.
    public sealed class SystemUIFont : MonoBehaviour {
        [SerializeField] string[] familyNames={"Microsoft YaHei","SimHei","Arial"};
        [SerializeField] Font bundledFont;
        public void Configure(Font font){bundledFont=font;}
        void Awake(){var font=bundledFont?bundledFont:Font.CreateDynamicFontFromOSFont(familyNames,22);foreach(var text in GetComponentsInChildren<Text>(true))text.font=font;}
    }
}

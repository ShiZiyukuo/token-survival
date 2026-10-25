using UnityEngine;
namespace TokenSurvival {
    public sealed class ScreenLayout : MonoBehaviour {
        [SerializeField] RectTransform safeRoot;
        [SerializeField] Camera worldCamera;
        Rect previous; int width,height;
        void Update(){if(previous==Screen.safeArea&&width==Screen.width&&height==Screen.height)return;previous=Screen.safeArea;width=Screen.width;height=Screen.height;if(width==0||height==0)return;
            safeRoot.anchorMin=new Vector2(previous.xMin/width,previous.yMin/height);safeRoot.anchorMax=new Vector2(previous.xMax/width,previous.yMax/height);safeRoot.offsetMin=safeRoot.offsetMax=Vector2.zero;
            float aspect=(float)width/height,design=16f/9;worldCamera.rect=aspect>design?new Rect((1-design/aspect)/2,0,design/aspect,1):new Rect(0,(1-aspect/design)/2,1,aspect/design);
        }
    }
}

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace TokenSurvival.EditorTools {
 public static class MiniUi {
  public static Button[] Create(Transform parent){var a=new Button[4];for(int i=0;i<4;i++){var g=new GameObject("CommunityKey_"+i,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image),typeof(Button));g.transform.SetParent(parent,false);var r=(RectTransform)g.transform;r.anchorMin=new Vector2(.18f+i*.17f,.2f);r.anchorMax=new Vector2(.31f+i*.17f,.31f);r.offsetMin=r.offsetMax=Vector2.zero;g.GetComponent<Image>().color=new Color(.08f,.3f,.5f,1);var t=new GameObject("Text",typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));t.transform.SetParent(g.transform,false);var tr=(RectTransform)t.transform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;var tx=t.GetComponent<Text>();tx.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");tx.fontSize=28;tx.alignment=TextAnchor.MiddleCenter;tx.color=Color.white;tx.text=new[]{"W","A","S","D"}[i];a[i]=g.GetComponent<Button>();g.SetActive(false);}return a;}
 }
}
#endif

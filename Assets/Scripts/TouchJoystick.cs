using UnityEngine;
using UnityEngine.EventSystems;
namespace TokenSurvival {
    public sealed class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
        public static Vector2 Axis {get;private set;}
        [SerializeField] RectTransform knob;
        int pointer=int.MinValue;
        public void OnPointerDown(PointerEventData e){if(pointer!=int.MinValue)return;pointer=e.pointerId;OnDrag(e);}
        public void OnDrag(PointerEventData e){if(e.pointerId!=pointer)return;var rect=(RectTransform)transform;RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,e.position,e.pressEventCamera,out var p);Axis=Vector2.ClampMagnitude(p/(rect.rect.width*.36f),1);knob.anchoredPosition=Axis*rect.rect.width*.36f;}
        public void OnPointerUp(PointerEventData e){if(e.pointerId!=pointer)return;pointer=int.MinValue;Axis=Vector2.zero;knob.anchoredPosition=Vector2.zero;}
        void OnDisable(){pointer=int.MinValue;ResetAxis();if(knob)knob.anchoredPosition=Vector2.zero;}
        public static void ResetAxis(){Axis=Vector2.zero;}
    }
}

using UnityEngine;
namespace TokenSurvival {
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class DepthSort : MonoBehaviour {
        [SerializeField] float feetOffset=-.65f;
        SpriteRenderer sprite;
        void Awake(){sprite=GetComponent<SpriteRenderer>();}
        void LateUpdate(){sprite.sortingOrder=400-Mathf.RoundToInt((transform.position.y+feetOffset)*50);}
    }
}

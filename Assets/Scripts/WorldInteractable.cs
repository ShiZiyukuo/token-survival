using UnityEngine;

namespace TokenSurvival {
    public enum StationType { Computer, Showcase, Begging, Feeding, Upgrade, Bed }
    [RequireComponent(typeof(Collider2D),typeof(DepthSort))]
    public sealed class WorldInteractable : MonoBehaviour {
        [SerializeField] StationType type;
        [SerializeField] string prompt;
        public string Prompt => prompt;
        public void Configure(StationType value, string label) { type=value; prompt=label; }
        public void Interact() { GameDirector.Instance.UseStation(type); }
        public void TouchInteract() { if(GameDirector.Instance)GameDirector.Instance.UseStation(type,true); }
    }
}

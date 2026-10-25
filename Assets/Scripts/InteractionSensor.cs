using System.Collections.Generic;
using UnityEngine;

namespace TokenSurvival {
    public sealed class InteractionSensor : MonoBehaviour {
        readonly List<WorldInteractable> nearby = new List<WorldInteractable>();
        PlayerController player;
        public WorldInteractable Nearest { get; private set; }
        void Awake() { player = GetComponentInParent<PlayerController>(); }
        void Update() {
            RefreshNearest();
            GameUI.Instance?.SetPrompt(Nearest ? "E  " + Nearest.Prompt : "");
            if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        }
        public bool TryInteract(){RefreshNearest();if(!Nearest||player.InputLocked||(StoryDirector.Instance&&StoryDirector.Instance.Blocking))return false;Nearest.Interact();return true;}
        public bool TryTouchInteract(){RefreshNearest();if(!Nearest)return false;Nearest.TouchInteract();return true;}
        void RefreshNearest(){WorldInteractable best=null;float bestD=float.MaxValue;var hits=Physics2D.OverlapCircleAll(transform.position,1.35f);for(int h=0;h<hits.Length;h++){var i=hits[h].GetComponentInParent<WorldInteractable>();if(i){float d=(i.transform.position-transform.position).sqrMagnitude;if(d<bestD){best=i;bestD=d;}}}foreach(var i in nearby)if(i){float d=(i.transform.position-transform.position).sqrMagnitude;if(d<bestD){best=i;bestD=d;}}Nearest=best;}
        void OnTriggerEnter2D(Collider2D c) { var i=c.GetComponent<WorldInteractable>(); if(i && !nearby.Contains(i)) nearby.Add(i); }
        void OnTriggerExit2D(Collider2D c) { var i=c.GetComponent<WorldInteractable>(); if(i) nearby.Remove(i); }
    }
}

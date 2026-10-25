using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace TokenSurvival {
    public sealed class MobileControls : MonoBehaviour {
        [SerializeField] GameObject walking, actions;
        [SerializeField] Button interact, cancel;
        [SerializeField] Button[] keys;
        [SerializeField] MinigameController minigame;
        [SerializeField] InteractionSensor sensor;
        public bool Enabled {get;private set;}
        void Awake(){Enabled=Application.isMobilePlatform||Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-forceTouch");if(!sensor)sensor=FindObjectOfType<InteractionSensor>();
            if(interact){var bridge=interact.gameObject.GetComponent<MobileInteractBridge>();if(!bridge)bridge=interact.gameObject.AddComponent<MobileInteractBridge>();bridge.owner=this;}if(cancel&&minigame)cancel.onClick.AddListener(minigame.Cancel);
            if(keys!=null)for(int i=0;i<keys.Length;i++){int n=i;if(keys[i])keys[i].onClick.AddListener(()=>Submit(n));}
        }
        void Update(){if(!walking||!actions||!interact||!minigame)return;bool open=!StoryDirector.Instance||!StoryDirector.Instance.Blocking;bool running=minigame.IsRunning;walking.SetActive(Enabled&&open&&!running);actions.SetActive(Enabled&&open);interact.gameObject.SetActive(Enabled&&open&&!running);interact.interactable=Enabled&&open;if(cancel)cancel.gameObject.SetActive(Enabled&&open&&running);
            if(keys!=null)for(int i=0;i<keys.Length;i++)if(keys[i])keys[i].gameObject.SetActive(Enabled&&open&&running);
            if(!actions.activeSelf||!running)return;var mode=minigame.CurrentMode;for(int i=0;i<keys.Length;i++){keys[i].gameObject.SetActive(mode==MinigameController.Mode.BowlBeat?i==0:mode==MinigameController.Mode.CodeSprint?i<3:true);keys[i].GetComponentInChildren<Text>().text=mode==MinigameController.Mode.BowlBeat?"敲 盆":mode==MinigameController.Mode.CodeSprint?"补丁 "+(i+1):new[]{"测试 W","网页 A","性能 S","需求 D"}[i];}
        }
        void Submit(int n){if(!minigame.IsRunning)return;if(minigame.CurrentMode==MinigameController.Mode.CodeSprint)minigame.SubmitPatch(n);else if(minigame.CurrentMode==MinigameController.Mode.ProjectPitch)minigame.SubmitPitch(new[]{KeyCode.W,KeyCode.A,KeyCode.S,KeyCode.D}[n]);else minigame.SubmitBeat();}
        public void TouchInteractNow(){if(!sensor)sensor=FindObjectOfType<InteractionSensor>();if(sensor&&!sensor.TryTouchInteract())GameUI.Instance?.Toast("请先靠近发光的交互物");}
    }
    public sealed class MobileInteractBridge : MonoBehaviour, IPointerClickHandler { public MobileControls owner; public void OnPointerClick(PointerEventData eventData){owner?.TouchInteractNow();} }
}

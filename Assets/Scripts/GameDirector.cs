using UnityEngine;
using System.Collections;

namespace TokenSurvival {
    public sealed class GameDirector : MonoBehaviour {
        public static GameDirector Instance { get; private set; }
        public int BowlImpactCount { get; private set; }
        [SerializeField] PlayerController player;
        [SerializeField] WhaleController whale;
        [SerializeField] MinigameController minigame;
        [SerializeField] AudioSource sfx;
        [SerializeField] AudioClip[] clips;
        [SerializeField] GameObject[] roomUpgrades;
        [SerializeField] Transform performanceSpot;
        [SerializeField] SpriteRenderer upgradePreview;
        void Awake(){ Instance=this; }
        void Start(){ GameState.Instance.Changed += RefreshUpgrades; RefreshUpgrades(); }
        public void UseStation(StationType type,bool fromTouch=false){
            if(minigame.IsRunning || (!fromTouch && StoryDirector.Instance && StoryDirector.Instance.Blocking)) return;
            if(StoryDirector.Instance && !StoryDirector.Instance.CanUseStation(type)){GameUI.Instance.Toast(StoryDirector.Instance.RequiredMessage);return;}
            switch(type){
                case StationType.Computer: StartGame(MinigameController.Mode.CodeSprint); break;
                case StationType.Showcase: StartGame(MinigameController.Mode.ProjectPitch); break;
                case StationType.Begging: if(whale.IsPerforming){GameUI.Instance.Toast("大肥鱼正在演出，稍等一下");return;}StartCoroutine(BeginPerformance());break;
                case StationType.Feeding:
                    if(whale.IsPerforming){GameUI.Instance.Toast("先让大肥鱼吃完、跳完这一段");return;}
                    if(GameState.Instance.Feed()){ GameUI.Instance.Toast("开饭！大肥鱼闻到香味了"); whale.PlayFeed(); }
                    else GameUI.Instance.Toast("Token 不够，先去接单或展示项目"); break;
                case StationType.Upgrade:
                    if(GameState.Instance.Upgrade()){ PlaySfx(3); GameUI.Instance.Toast("房间升级完成，收益加成提高"); RefreshUpgrades(); }
                    else GameUI.Instance.Toast("Token 不足或已经全部升级"); break;
                case StationType.Bed: GameState.Instance.RestAtBed();GameUI.Instance.Toast("DeepSeek 娘：未完待续……");if(StoryDirector.Instance)StoryDirector.Instance.OnBedVisited();break;
            }
        }
        void StartGame(MinigameController.Mode mode){ player.InputLocked=true; minigame.Begin(mode, OnGameFinished); }
        IEnumerator BeginPerformance(){player.InputLocked=true;GameUI.Instance.Toast("大肥鱼正在走向直播角");yield return whale.WalkToStage(performanceSpot.position);StartGame(MinigameController.Mode.BowlBeat);}
        void OnGameFinished(MinigameController.Mode mode, int score){
            player.InputLocked=false;
            if(mode==MinigameController.Mode.BowlBeat) whale.EndBowl();
            if(score<0){GameUI.Instance.Toast("已取消，未交付的任务不会产生收益");return;}
            int baseReward=mode==MinigameController.Mode.CodeSprint?GameState.Instance.Config.computerBaseReward:mode==MinigameController.Mode.ProjectPitch?GameState.Instance.Config.showcaseBaseReward:GameState.Instance.Config.beggingBaseReward;
            int reward=Mathf.RoundToInt(baseReward*(.45f+.11f*score)*(1f+.12f*GameState.Instance.ComfortLevel));
            GameState.Instance.Earn(reward, mode!=MinigameController.Mode.BowlBeat); PlaySfx(0); GameUI.Instance.Toast($"完成！准确度 {score}/5，获得 {reward} Token");
            GameState.Instance.CompleteActivity(mode);
        }
        void RefreshUpgrades(){if(upgradePreview)upgradePreview.enabled=GameState.Instance.ComfortLevel==0;for(int i=0;i<roomUpgrades.Length;i++)if(roomUpgrades[i])roomUpgrades[i].SetActive(i==GameState.Instance.ComfortLevel-1);}
        public void PlaySfx(int index){if(index>=0&&index<clips.Length&&clips[index]){sfx.PlayOneShot(clips[index]);if(index==4)BowlImpactCount++;}}
        public void BowlHit(){whale.PlayBowlHit();}
    }
}

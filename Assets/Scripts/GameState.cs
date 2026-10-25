using System;
using UnityEngine;

namespace TokenSurvival {
    public sealed class GameState : MonoBehaviour {
        public static GameState Instance { get; private set; }
        [SerializeField] GameConfig config;
        public GameConfig Config => config;
        public int Tokens { get; private set; }
        public float Hunger { get; private set; }
        public int ComfortLevel { get; private set; }
        public int JobsCompleted { get; private set; }
        public int StoryChapter { get; private set; }
        public int Feeds { get; private set; }
        public int Showcases { get; private set; }
        public int Performances { get; private set; }
        public bool BedVisited { get; private set; }
        public bool BowlAfterSeen { get; private set; }
        public event Action Changed;

        void Awake() {
            Instance = this;
            var save = SaveSystem.Load();
            Tokens = save.valid ? save.tokens : config.startingTokens;
            Hunger = save.valid ? save.hunger : config.startingHunger;
            ComfortLevel = save.valid ? save.comfort : 0;
            JobsCompleted = save.valid ? save.jobs : 0;
            StoryChapter = save.chapter; Feeds=save.feeds; Showcases=save.showcases; Performances=save.performances;
            BedVisited=save.bedVisited;
            BowlAfterSeen=save.bowlAfterSeen;
        }

        void Update() {
            if(!StoryDirector.Instance || !StoryDirector.Instance.Blocking) AdvanceTime(Time.deltaTime);
        }
        public void AdvanceTime(float seconds){
            float old = Hunger;
            Hunger = Mathf.Max(0, Hunger - config.hungerDrainPerMinute * Mathf.Max(0,seconds) / 60f);
            if (Mathf.FloorToInt(old) != Mathf.FloorToInt(Hunger)) Changed?.Invoke();
        }
        void OnApplicationQuit(){SaveSystem.Save(this);}

        public void Earn(int amount, bool countsAsJob = false) {
            Tokens += Mathf.Max(0, amount);
            if (countsAsJob) JobsCompleted++;
            Changed?.Invoke(); SaveSystem.Save(this);
        }

        public bool Spend(int amount) {
            if (Tokens < amount) return false;
            Tokens -= amount; Changed?.Invoke(); SaveSystem.Save(this); return true;
        }

        public bool Feed() {
            if (!Spend(config.feedCost)) return false;
            Hunger = Mathf.Min(100, Hunger + config.feedAmount);
            Feeds++;
            Changed?.Invoke(); SaveSystem.Save(this); return true;
        }

        public bool Upgrade() {
            if (ComfortLevel >= config.upgradeCosts.Length) return false;
            if (!Spend(config.upgradeCosts[ComfortLevel])) return false;
            ComfortLevel++; Changed?.Invoke(); SaveSystem.Save(this); return true;
        }
        public void CompleteActivity(MinigameController.Mode mode){if(mode==MinigameController.Mode.ProjectPitch)Showcases++;if(mode==MinigameController.Mode.BowlBeat)Performances++;Changed?.Invoke();SaveSystem.Save(this);}
        public void SetChapter(int chapter){StoryChapter=chapter;Changed?.Invoke();SaveSystem.Save(this);}
        public void RestAtBed(){BedVisited=true;Changed?.Invoke();SaveSystem.Save(this);}
        public void MarkBowlAfterSeen(){BowlAfterSeen=true;Changed?.Invoke();SaveSystem.Save(this);}
        public void NewGame(){Tokens=config.startingTokens;Hunger=config.startingHunger;ComfortLevel=JobsCompleted=StoryChapter=Feeds=Showcases=Performances=0;BedVisited=BowlAfterSeen=false;Changed?.Invoke();SaveSystem.Save(this);}
        void OnApplicationPause(bool paused){if(paused)SaveSystem.Save(this);}
    }
}

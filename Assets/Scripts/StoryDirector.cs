using System;
using UnityEngine;
using UnityEngine.UI;
namespace TokenSurvival {
    // Saved scene UI and character animators provide the presentation; no world is spawned here.
    public sealed class StoryDirector : MonoBehaviour {
        public static StoryDirector Instance {get;private set;}
        [SerializeField] StoryCatalog catalog;
        [SerializeField] GameObject menu, dialogue, pausePanel, confirmPanel;
        [SerializeField] Text chapterTitle, speaker, body, objective, progress, menuSubtitle;
        [SerializeField] Image portrait;
        [SerializeField] Sprite playerPortrait, whalePortrait;
        [SerializeField] Button newGame, continueGame, next, skip, pause, resume, home, sound, confirm, back;
        [SerializeField] GameObject endingChoice;
        [SerializeField] Button freeExplore, exitGame;
        [SerializeField] Camera sceneCamera;
        [SerializeField] MinigameController minigame;
        [SerializeField] AudioSource music;
        [SerializeField] PlayerController player;
        [SerializeField] WhaleController whale;
        int line, chapter; bool playing, talking, paused, smoke; float revealed, entered;
        bool bowlAfterDialogue;
        Vector3 cameraHome; float cameraSize; string fullText;
        public bool Blocking=>!playing||talking||paused;
        public bool Talking=>talking;
        public bool IsPaused=>paused;
        public string RequiredMessage { get { if(GameState.Instance.StoryChapter==1)return "第一章：请先到电脑前完成外包，其他交互尚未开放"; if((GameState.Instance.StoryChapter==1||GameState.Instance.StoryChapter==2)&&GameState.Instance.JobsCompleted>0&&GameState.Instance.Feeds==0)return "DeepSeek 娘饿了：先喂 25 Token，或者继续做外包"; if(GameState.Instance.StoryChapter==2&&GameState.Instance.Feeds>0&&GameState.Instance.Showcases==0)return "先把项目整理好，在展示台完成社区展示"; if(GameState.Instance.StoryChapter==3&&GameState.Instance.Showcases==0)return "先在展示台展示项目"; if(GameState.Instance.StoryChapter==4&&GameState.Instance.Performances==0)return "先去直播募集角，和大肥鱼一起敲盆"; if(GameState.Instance.StoryChapter==4&&GameState.Instance.Performances>0&&GameState.Instance.Tokens<100)return "打赏还不够，先继续做外包，攒到 100 Token"; if(GameState.Instance.StoryChapter==5&&GameState.Instance.Performances>0&&GameState.Instance.ComfortLevel==0)return "先花 70 Token 升级设备"; return ""; } }
        public int CurrentLine=>line;
        public bool CanUseStation(StationType type){int c=GameState.Instance.StoryChapter;if(smoke)return true;if(c==1)return type==StationType.Computer;if((c==1||c==2)&&GameState.Instance.JobsCompleted>0&&GameState.Instance.Feeds==0)return type==StationType.Feeding||type==StationType.Computer;if(c==2&&GameState.Instance.Feeds>0&&GameState.Instance.Showcases==0)return type==StationType.Showcase;if(c==3&&GameState.Instance.Showcases==0)return type==StationType.Showcase;if(c==4&&GameState.Instance.Performances==0)return type==StationType.Begging;if(c==4&&GameState.Instance.Performances>0&&GameState.Instance.Tokens<100)return type==StationType.Computer;if(c==5&&GameState.Instance.Performances>0&&GameState.Instance.ComfortLevel==0)return type==StationType.Upgrade;return true;}
        void Awake(){Instance=this;cameraHome=sceneCamera.transform.position;cameraSize=sceneCamera.orthographicSize;
            newGame.onClick.AddListener(RequestNewGame);continueGame.onClick.AddListener(ContinueGame);
            next.onClick.AddListener(Advance);skip.onClick.AddListener(SkipScene);pause.onClick.AddListener(TogglePause);resume.onClick.AddListener(TogglePause);home.onClick.AddListener(ReturnTitle);
            sound.onClick.AddListener(ToggleSound);confirm.onClick.AddListener(StartNewGame);back.onClick.AddListener(()=>confirmPanel.SetActive(false));
            if(freeExplore)freeExplore.onClick.AddListener(()=>endingChoice.SetActive(false));if(exitGame)exitGame.onClick.AddListener(()=>Application.Quit());
        }
        void Start(){smoke=Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-tokenSmoke");
            music.mute=PlayerPrefs.GetInt("TokenSurvivalMuted",0)==1;
            if(smoke){playing=true;menu.SetActive(false);dialogue.SetActive(false);pausePanel.SetActive(false);return;}
            ReturnTitle();
        }
        public void RequestNewGame(){if(SaveSystem.Load().valid)confirmPanel.SetActive(true);else StartNewGame();}
        public void StartNewGame(){GameState.Instance.NewGame();player.transform.position=new Vector3(0,-2.5f,0);whale.transform.position=new Vector3(1.2f,-2.1f,0);confirmPanel.SetActive(false);playing=true;paused=false;menu.SetActive(false);pausePanel.SetActive(false);Time.timeScale=1;BeginScene(0);}
        public void ContinueGame(){playing=true;paused=false;menu.SetActive(false);pausePanel.SetActive(false);Time.timeScale=1;if(GameState.Instance.StoryChapter==0)BeginScene(0);}
        public void ReturnTitle(){SaveSystem.Save(GameState.Instance);playing=false;talking=false;paused=false;dialogue.SetActive(false);pausePanel.SetActive(false);menu.SetActive(true);confirmPanel.SetActive(false);Time.timeScale=0;
            sceneCamera.transform.position=cameraHome;sceneCamera.orthographicSize=cameraSize;
            menuSubtitle.text="雨夜、欠费的 API，和一位吃 Token 的新室友。\n一段从修好第一个 Bug 开始的小小生活。";
        }
        public void TogglePause(){if(!playing||talking)return;paused=!paused;pausePanel.SetActive(paused);Time.timeScale=paused?0:1;if(paused)SaveSystem.Save(GameState.Instance);TouchJoystick.ResetAxis();}
        public void ToggleSound(){music.mute=!music.mute;AudioListener.volume=music.mute?0:1;PlayerPrefs.SetInt("TokenSurvivalMuted",music.mute?1:0);sound.GetComponentInChildren<Text>().text=music.mute?"声音：关闭":"声音：开启";}
        void Update(){
            // Keyboard fallback keeps the game playable even when the Canvas has no focused Button.
            if(!playing){if(Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.Space)){if(SaveSystem.Load().valid)ContinueGame();else StartNewGame();}return;}
            if(smoke)return;
            if(Input.GetKeyDown(KeyCode.Escape)&&!minigame.IsRunning)TogglePause();
            if(talking){revealed+=Time.unscaledDeltaTime*35;body.text=fullText.Substring(0,Mathf.Min(fullText.Length,(int)revealed));
                float t=Mathf.Clamp01((Time.unscaledTime-entered)/1.2f);t=t*t*(3-2*t);
                var focus=catalog.chapters[chapter].cameraFocus;sceneCamera.transform.position=Vector3.Lerp(cameraHome,new Vector3(focus.x,focus.y,-10),t);sceneCamera.orthographicSize=Mathf.Lerp(cameraSize,3.8f,t);
                portrait.rectTransform.localScale=Vector3.one*(1+.015f*Mathf.Sin(Time.unscaledTime*2));
                if(Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.Space))Advance();return;
            }
            if(!playing||paused)return;
            int c=GameState.Instance.StoryChapter;
            if(c==4&&GameState.Instance.Performances>0&&!GameState.Instance.BowlAfterSeen&&!minigame.IsRunning&&!whale.IsPerforming&&!player.InputLocked){BeginScene(3);bowlAfterDialogue=true;ShowLine();return;}
            if(c<catalog.chapters.Length){string goal=catalog.chapters[c].objective;objective.text=goal;GameUI.Instance.SetObjective(goal);progress.text="章节 "+c+" / 5";}
            else {objective.text="尾声之后 · 接单、养鱼，继续改善生活";GameUI.Instance.SetObjective("自由生活 · 继续养鱼与升级");progress.text="故事已完成";}
            bool complete=c==1?GameState.Instance.JobsCompleted>0:c==2?GameState.Instance.Feeds>0:c==3?GameState.Instance.Showcases>0:c==4?GameState.Instance.Performances>0&&GameState.Instance.Tokens>=100:c==5?GameState.Instance.ComfortLevel>0:false;
            if(complete&&!minigame.IsRunning&&!whale.IsPerforming&&!player.InputLocked)BeginScene(c);
        }
        void BeginScene(int index){bowlAfterDialogue=false;chapter=index;line=0;talking=true;dialogue.SetActive(true);entered=Time.unscaledTime;ShowLine();}
        void ShowLine(){var c=catalog.chapters[chapter];var l=bowlAfterDialogue?catalog.bowlAfterLines[line]:c.lines[line];chapterTitle.text=bowlAfterDialogue?"第四章 · 真的有人打赏":c.title;speaker.text=l.speaker;fullText=l.text;revealed=0;body.text="";portrait.sprite=l.whale?whalePortrait:playerPortrait;portrait.preserveAspect=true;}
        public void Advance(){if(!talking)return;if(revealed<fullText.Length){revealed=fullText.Length;body.text=fullText;return;}line++;if(line>=(bowlAfterDialogue?catalog.bowlAfterLines.Length:catalog.chapters[chapter].lines.Length))SkipScene();else ShowLine();}
        public void OnBedVisited(){if(GameState.Instance.StoryChapter>=6&&endingChoice)endingChoice.SetActive(true);}
        public void SkipScene(){if(!talking)return;talking=false;dialogue.SetActive(false);if(bowlAfterDialogue)GameState.Instance.MarkBowlAfterSeen();else GameState.Instance.SetChapter(chapter+1);sceneCamera.transform.position=cameraHome;sceneCamera.orthographicSize=cameraSize;if(chapter==2&&!bowlAfterDialogue)whale.PlayCheer();if(chapter==5){whale.Celebrate();GameUI.Instance.SetObjective("去床边休息，和 DeepSeek 娘说晚安");}GameUI.Instance.Toast(chapter==0?"第一章 · 到电脑前完成一份外包":"新章节已解锁 · 进度已保存");}
        void OnApplicationPause(bool value){if(value&&playing&&!talking&&!paused)TogglePause();}
        void OnDestroy(){Time.timeScale=1;if(Instance==this)Instance=null;}
    }
}

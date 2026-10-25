using System;
using System.Collections;
using System.IO;
using UnityEngine;
namespace TokenSurvival {
    public sealed class StoryFixValidation : MonoBehaviour {
        int failures; int runtimeErrors;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init(){if(Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-storySmoke"))new GameObject("StoryFixValidation").AddComponent<StoryFixValidation>();}
        void OnEnable(){Application.logMessageReceived+=Log;}
        void OnDisable(){Application.logMessageReceived-=Log;}
        void Log(string m,string stack,LogType t){if(t==LogType.Error||t==LogType.Exception)runtimeErrors++;}
        void Check(bool value,string label){if(!value)failures++;Debug.Log("[StoryFix] "+(value?"PASS ":"FAIL ")+label);}
        IEnumerator Start(){
            yield return new WaitForSecondsRealtime(1);
            var story=StoryDirector.Instance;var state=GameState.Instance;var mini=FindObjectOfType<MinigameController>(true);
            story.StartNewGame();story.SkipScene();yield return null;
            Check(state.StoryChapter==1,"First dialogue ends in outsourcing chapter");
            foreach(StationType t in Enum.GetValues(typeof(StationType)))Check(story.CanUseStation(t)==(t==StationType.Computer),"Chapter 1 interaction gate: "+t);
            int tokens=state.Tokens;GameDirector.Instance.UseStation(StationType.Feeding);GameDirector.Instance.UseStation(StationType.Begging);GameDirector.Instance.UseStation(StationType.Upgrade);GameDirector.Instance.UseStation(StationType.Showcase);GameDirector.Instance.UseStation(StationType.Bed);
            Check(state.Tokens==tokens&&!mini.IsRunning&&!state.BedVisited&&state.Feeds==0,"Locked stations have no side effects");
            GameDirector.Instance.UseStation(StationType.Computer);Check(mini.IsRunning,"Computer still opens an outsourcing task");mini.Cancel();
            state.SetChapter(4);state.CompleteActivity(MinigameController.Mode.BowlBeat);yield return null;yield return null;
            Check(story.Talking,"Post-bowl dialogue begins immediately before 100 Token");
            story.SkipScene();Check(state.BowlAfterSeen&&state.StoryChapter==4,"Post-bowl dialogue records once without skipping chapter");
            yield return null;Check(!story.Talking,"Post-bowl dialogue does not replay");
            state.Earn(100);yield return null;Check(story.Talking,"100 Token unlocks chapter 5 after post-bowl dialogue");story.SkipScene();Check(state.StoryChapter==5,"Chapter 5 unlock persists");
            Check(runtimeErrors==0,"No runtime Console errors");
            Directory.CreateDirectory(Path.Combine(Application.dataPath,"../Validation"));File.WriteAllText(Path.Combine(Application.dataPath,"../Validation/story-fix-result.txt"),"STORY_FIX_RESULT failures="+failures+" runtimeErrors="+runtimeErrors);
            Debug.Log("STORY_FIX_RESULT failures="+failures+" runtimeErrors="+runtimeErrors);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(failures+runtimeErrors==0?0:1);
#else
            Application.Quit(failures+runtimeErrors==0?0:1);
#endif
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TokenSurvival {
    public sealed class RuntimeValidation : MonoBehaviour {
        readonly List<string> checks=new List<string>();
        int errors, runtimeErrors;
        PlayerController player; MinigameController mini; WhaleController whale;
        string output;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init(){if(Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-tokenSmoke"))new GameObject("ValidationRunner_OnlyInSelfTest").AddComponent<RuntimeValidation>();}
        void OnEnable(){Application.logMessageReceived+=OnLog;}
        void OnDisable(){Application.logMessageReceived-=OnLog;}
        void OnLog(string message,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception){errors++;runtimeErrors++;checks.Add("RUNTIME_ERROR: "+message);}}
        IEnumerator Start(){
            output=Path.GetFullPath(Path.Combine(Application.dataPath,"../Validation"));Directory.CreateDirectory(output);
            yield return new WaitForSeconds(1);
            player=FindObjectOfType<PlayerController>();mini=FindObjectOfType<MinigameController>(true);whale=FindObjectOfType<WhaleController>();
            Check(player&&mini&&whale,"Configured scene references available");
            player.ValidationMode=true;
            yield return Capture("01-room");
            Vector3 start=player.transform.position;yield return Move(Vector2.left,1.4f);
            Check(player.transform.position.x<start.x-1,"Player walks using Rigidbody2D");
            Check(player.transform.position.x>-3.3f,"Bed collision stops movement");
            yield return Move(Vector2.up,1f);yield return Move(Vector2.left,.58f);yield return Move(Vector2.up,.30f);
            var sensor=player.GetComponentInChildren<InteractionSensor>();yield return null;
            Check(sensor.Nearest&&sensor.Nearest.Prompt.Contains("开发"),"Computer detected by proximity trigger");
            int tokens=GameState.Instance.Tokens;
            Check(sensor.TryInteract()&&mini.IsRunning,"Proximity interaction opens coding contract");
            yield return Capture("02-coding");
            int[] answers={0,1,0,2,1};for(int i=0;i<5;i++){mini.SubmitPatch(answers[i]);yield return new WaitForSeconds(.9f);}
            Check(!mini.IsRunning&&!player.InputLocked,"Contract closes and movement resumes");
            Check(GameState.Instance.Tokens>tokens&&GameState.Instance.JobsCompleted==1,"Contract payout updates Token and project record");
            yield return Move(Vector2.right,.9f);yield return null;Check(sensor.Nearest&&sensor.Nearest.Prompt.Contains("展示"),"Showcase reachable using walking and proximity");Check(sensor.TryInteract()&&mini.IsRunning,"Showcase opens through shared interaction input");yield return Capture("03-showcase");
            for(int i=0;i<5;i++){mini.SubmitPitch(mini.ExpectedKey);yield return new WaitForSeconds(.9f);}
            Check(GameState.Instance.JobsCompleted==2,"Project showcase pays according to operation results");
            yield return Move(Vector2.down,.25f);yield return Move(Vector2.right,1.85f);yield return Move(Vector2.down,.33f);yield return null;
            Check(sensor.Nearest&&sensor.Nearest.Prompt.Contains("喂"),"Food cabinet reachable without crossing furniture colliders");
            GameState.Instance.AdvanceTime(600);yield return new WaitForSeconds(.1f);Check(whale.CurrentMood==WhaleController.Mood.Hungry,"Simulated time drains hunger and selects hungry animation");
            Check(sensor.TryInteract(),"Feeding activates through proximity interaction");yield return new WaitForSeconds(.4f);
            Check(whale.CurrentMood==WhaleController.Mood.Eat,"Feed enters eating state");yield return Capture("04-eating");
            yield return new WaitForSeconds(1f);Check(whale.CurrentMood==WhaleController.Mood.Happy,"Eating transitions to happy");
            yield return new WaitForSeconds(1f);Check(whale.CurrentMood==WhaleController.Mood.Dance,"Happy transitions to independent dance clip");yield return new WaitForSeconds(.5f);yield return Capture("05-dance");
            yield return new WaitForSeconds(4);Check(whale.CurrentMood==WhaleController.Mood.Idle||whale.CurrentMood==WhaleController.Mood.Follow,"Dance returns to normal locomotion");
            yield return Move(Vector2.left,.5f);yield return Move(Vector2.up,.2f);yield return null;Check(sensor.Nearest&&sensor.Nearest.Prompt.Contains("敲盆"),"Livestream station reachable by walking");Check(sensor.TryInteract(),"Livestream starts through proximity input");float timeout=8;while(!mini.IsRunning&&timeout>0){timeout-=Time.deltaTime;yield return null;}Check(mini.IsRunning&&whale.CurrentMood==WhaleController.Mood.Bowl,"Whale walks to livestream stage before bowl activity");yield return new WaitForSeconds(.82f);mini.SubmitBeat();yield return Capture("06-bowl");
            for(int i=1;i<5;i++){yield return new WaitForSeconds(.85f);yield return new WaitForSeconds(.82f);mini.SubmitBeat();}
            yield return new WaitForSeconds(1f);Check(!mini.IsRunning,"Bowl rhythm activity finishes");
            Check(GameDirector.Instance.BowlImpactCount==5,"Five bowl AnimationEvents trigger five assigned impact sounds");
            yield return Move(Vector2.down,.6f);yield return Move(Vector2.right,.45f);yield return null;Check(sensor.Nearest&&sensor.Nearest.Prompt.Contains("改善"),"Upgrade corner reachable by walking");
            int comfort=GameState.Instance.ComfortLevel;Check(sensor.TryInteract(),"Upgrade input detected at device corner");Check(GameState.Instance.ComfortLevel==comfort+1,"Earned Token buys visible room upgrade");yield return Capture("07-upgrade");
            Check(runtimeErrors==0,"No runtime Console errors");
            File.WriteAllLines(Path.Combine(output,"playmode-report.txt"),checks);Debug.Log("[TokenSurvival] PLAYMODE_RESULT errors="+errors);
            yield return new WaitForSeconds(.3f);
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(errors==0?0:1);
            #else
            Application.Quit(errors==0?0:1);
            #endif
        }
        IEnumerator Move(Vector2 input,float seconds){player.ValidationInput=input;yield return new WaitForSeconds(seconds);player.ValidationInput=Vector2.zero;yield return new WaitForFixedUpdate();}
        IEnumerator Capture(string name){yield return null;Canvas.ForceUpdateCanvases();var camera=Camera.main;var rt=new RenderTexture(1600,900,24);var previous=camera.targetTexture;camera.targetTexture=rt;camera.Render();var active=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes(Path.Combine(output,name+".png"),image.EncodeToPNG());RenderTexture.active=active;camera.targetTexture=previous;Destroy(image);rt.Release();Destroy(rt);yield return new WaitForSeconds(.25f);}
        void Check(bool ok,string name){checks.Add((ok?"PASS: ":"FAIL: ")+name);if(!ok)errors++;Debug.Log("[TokenSurvival] "+checks[checks.Count-1]);}
    }
}

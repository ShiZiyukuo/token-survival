#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace TokenSurvival.EditorTools {
    public static class RebuildProjectBuilder {
        const string Root="Assets/TokenSurvival";
        static Font font;

        [MenuItem("Token Survival/Rebuild All Assets")]
        public static void BuildAll(){
            EnsureFolders(); ConfigureTextures(); CreateAudio(); AssetDatabase.Refresh();
            var config=CreateConfig(); CreateAnimations(); CreateAtlas();
            var player=CreatePlayerPrefab(); var whale=CreateWhalePrefab(); var stations=CreateStationPrefabs();
            CreateMainScene(config,player,whale,stations); SetBuildSettings();
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("[TokenSurvival] BUILD_OK: saved scene, prefabs, animations, atlas, config and audio.");
        }

        public static void BuildWindows(){
            BuildAll();
            Directory.CreateDirectory("Build/Windows");
            var report=UnityEditor.BuildPipeline.BuildPlayer(new[]{Root+"/Scenes/MainRoom.unity"},"Build/Windows/TokenSurvival.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
            Debug.Log("[TokenSurvival] PLAYER_BUILD="+report.summary.result+" errors="+report.summary.totalErrors);
            if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded) throw new Exception("Windows build failed");
        }

        static void EnsureFolders(){
            foreach(var path in new[]{Root,Root+"/Art",Root+"/Animations",Root+"/Audio",Root+"/Config",Root+"/Prefabs",Root+"/Scenes",Root+"/UI"}) Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        static void ConfigureTextures(){
            ImportSingle(Root+"/Art/room.png",100);
            ImportSingle(Root+"/Art/room_clean.png",100);
            foreach(string name in new[]{"computer","showcase","livestream","food_cabinet","upgrade"})ImportSingle(Root+"/Art/"+name+".png",300);
            ImportGrid(Root+"/Art/player_sheet.png",4,4,"Player",220);
            ImportGrid(Root+"/Art/whale_sheet.png",8,4,"Whale",120);
            ImportGrid(Root+"/Art/whale_walk_sheet.png",4,4,"WhaleWalk",175);
            ImportGrid(Root+"/Art/whale_dance_sheet.png",4,2,"WhaleDance",240);
            ImportGrid(Root+"/Art/props_sheet.png",5,1,"Prop",120);
        }
        static void BaseTexture(TextureImporter ti,int ppu){ti.textureType=TextureImporterType.Sprite;ti.spritePixelsPerUnit=ppu;ti.alphaIsTransparency=true;ti.mipmapEnabled=false;ti.npotScale=TextureImporterNPOTScale.None;ti.filterMode=FilterMode.Bilinear;ti.textureCompression=TextureImporterCompression.Uncompressed;ti.maxTextureSize=4096;}
        static void ImportSingle(string path,int ppu){var ti=AssetImporter.GetAtPath(path) as TextureImporter;if(ti==null)return;BaseTexture(ti,ppu);ti.spriteImportMode=SpriteImportMode.Single;ti.SaveAndReimport();}
        static void ImportGrid(string path,int cols,int rows,string prefix,int ppu){
            var ti=AssetImporter.GetAtPath(path) as TextureImporter;if(ti==null)return;BaseTexture(ti,ppu);ti.spriteImportMode=SpriteImportMode.Multiple;
            ti.GetSourceTextureWidthAndHeight(out int sourceWidth,out int sourceHeight);int w=sourceWidth/cols,h=sourceHeight/rows;var list=new List<SpriteMetaData>();
            for(int top=0;top<rows;top++)for(int c=0;c<cols;c++){list.Add(new SpriteMetaData{name=$"{prefix}_{top}_{c}",rect=new Rect(c*w,(rows-1-top)*h,w,h),alignment=(int)SpriteAlignment.Center,pivot=new Vector2(.5f,.5f)});}
            ti.spritesheet=list.ToArray();ti.SaveAndReimport();
        }

        static GameConfig CreateConfig(){
            string p=Root+"/Config/GameConfig.asset";var a=AssetDatabase.LoadAssetAtPath<GameConfig>(p);if(!a){a=ScriptableObject.CreateInstance<GameConfig>();AssetDatabase.CreateAsset(a,p);}a.startingTokens=0;a.feedCost=25;a.computerBaseReward=18;a.upgradeCosts=new[]{70,120,190};EditorUtility.SetDirty(a);return a;
        }

        static Dictionary<string,AnimationClip> CreateAnimations(){
            var all=new Dictionary<string,AnimationClip>();var ps=LoadSprites(Root+"/Art/player_sheet.png");
            for(int r=0;r<4;r++){string d=new[]{"Down","Left","Right","Up"}[r];all["Idle"+d]=Clip("Player_Idle"+d,new[]{ps[$"Player_{r}_0"],ps[$"Player_{r}_2"]},1,true);all["Walk"+d]=Clip("Player_Walk"+d,Enumerable.Range(0,4).Select(i=>ps[$"Player_{r}_{i}"]).ToArray(),8,true);}
            Controller(Root+"/Animations/Player.controller",all.Where(x=>x.Key.StartsWith("Idle")||x.Key.StartsWith("Walk")).ToDictionary(x=>x.Key,x=>x.Value),"IdleDown");
            var ws=LoadSprites(Root+"/Art/whale_sheet.png");
            all["Idle"]=Clip("Whale_Idle",Take(ws,"Whale",0,0,4),6,true); all["Follow"]=Clip("Whale_Follow",Take(ws,"Whale",0,4,4),8,true);
            all["Hungry"]=Clip("Whale_Hungry",Take(ws,"Whale",1,0,4),5,true); all["Eat"]=Clip("Whale_Eat",Take(ws,"Whale",1,4,4),7,true);
            all["Happy"]=Clip("Whale_Happy",Take(ws,"Whale",2,0,4),7,true);
            var dance=LoadSprites(Root+"/Art/whale_dance_sheet.png");all["Dance"]=Clip("Whale_Dance",Take(dance,"WhaleDance",0,0,4).Concat(Take(dance,"WhaleDance",1,0,4)).ToArray(),6,true);
            var walk=LoadSprites(Root+"/Art/whale_walk_sheet.png");for(int r=0;r<4;r++){string dir=new[]{"Down","Right","Left","Up"}[r];all["Follow"+dir]=Clip("Whale_Follow"+dir,Take(walk,"WhaleWalk",r,0,4),8,true);}
            all["Bowl"]=Clip("Whale_Bowl",Take(ws,"Whale",3,0,8),12,false);
            AnimationUtility.SetAnimationEvents(all["Bowl"],new[]{new AnimationEvent{time=4f/12f,functionName="BowlImpact"}});
            all["BowlReady"]=Clip("Whale_BowlReady",new[]{ws["Whale_3_0"]},6,true);
            Controller(Root+"/Animations/Whale.controller",new[]{"Idle","Follow","FollowDown","FollowLeft","FollowRight","FollowUp","Hungry","Eat","Happy","Dance","Bowl","BowlReady"}.ToDictionary(k=>k,k=>all[k]),"Idle");
            return all;
        }
        static Sprite[] Take(Dictionary<string,Sprite>d,string p,int r,int start,int count){return Enumerable.Range(start,count).Select(i=>d[$"{p}_{r}_{i}"]).ToArray();}
        static Dictionary<string,Sprite> LoadSprites(string path){return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToDictionary(x=>x.name,x=>x);}
        static AnimationClip Clip(string assetName,Sprite[] frames,float fps,bool loop){
            string p=Root+"/Animations/"+assetName+".anim";var old=AssetDatabase.LoadAssetAtPath<AnimationClip>(p);if(old)AssetDatabase.DeleteAsset(p);
            var c=new AnimationClip{frameRate=fps};var key=frames.Select((s,i)=>new ObjectReferenceKeyframe{time=i/fps,value=s}).ToArray();
            AnimationUtility.SetObjectReferenceCurve(c,new EditorCurveBinding{type=typeof(SpriteRenderer),path="",propertyName="m_Sprite"},key);
            var set=AnimationUtility.GetAnimationClipSettings(c);set.loopTime=loop;AnimationUtility.SetAnimationClipSettings(c,set);AssetDatabase.CreateAsset(c,p);return c;
        }
        static AnimatorController Controller(string path,Dictionary<string,AnimationClip> clips,string initial){
            if(AssetDatabase.LoadAssetAtPath<AnimatorController>(path))AssetDatabase.DeleteAsset(path);var c=AnimatorController.CreateAnimatorControllerAtPath(path);var sm=c.layers[0].stateMachine;
            foreach(var kv in clips){var s=sm.AddState(kv.Key);s.motion=kv.Value;if(kv.Key==initial)sm.defaultState=s;}
            if(clips.ContainsKey("BowlReady")){var from=sm.states.First(x=>x.state.name=="Bowl").state;var to=sm.states.First(x=>x.state.name=="BowlReady").state;var transition=from.AddTransition(to);transition.hasExitTime=true;transition.exitTime=1;transition.duration=0;}
            return c;
        }
        static void CreateAtlas(){
            string p=Root+"/Art/Gameplay.spriteatlas";if(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(p))AssetDatabase.DeleteAsset(p);var a=new SpriteAtlas();a.SetPackingSettings(new SpriteAtlasPackingSettings{enableRotation=false,enableTightPacking=false,padding=4});a.SetTextureSettings(new SpriteAtlasTextureSettings{readable=false,generateMipMaps=false,sRGB=true,filterMode=FilterMode.Bilinear});
            a.Add(new UnityEngine.Object[]{AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/player_sheet.png"),AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/whale_sheet.png"),AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/whale_walk_sheet.png"),AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/whale_dance_sheet.png")});a.Add(new[]{"computer","showcase","livestream","food_cabinet","upgrade"}.Select(n=>(UnityEngine.Object)AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/"+n+".png")).ToArray());AssetDatabase.CreateAsset(a,p);
        }

        static GameObject CreatePlayerPrefab(){
            var go=new GameObject("Player");var sr=go.AddComponent<SpriteRenderer>();sr.sortingOrder=20;sr.sprite=LoadSprites(Root+"/Art/player_sheet.png")["Player_0_0"];go.AddComponent<DepthSort>();
            var rb=go.AddComponent<Rigidbody2D>();rb.gravityScale=0;rb.freezeRotation=true;rb.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
            var col=go.AddComponent<CapsuleCollider2D>();col.size=new Vector2(.5f,.3f);col.offset=new Vector2(0,-.5f);
            var anim=go.AddComponent<Animator>();anim.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Root+"/Animations/Player.controller");go.AddComponent<PlayerController>();
            var sensor=new GameObject("InteractionSensor");sensor.transform.SetParent(go.transform);var sc=sensor.AddComponent<CircleCollider2D>();sc.isTrigger=true;sc.radius=1.15f;sensor.AddComponent<InteractionSensor>();
            var p=SavePrefab(go,Root+"/Prefabs/Player.prefab");UnityEngine.Object.DestroyImmediate(go);return p;
        }
        static GameObject CreateWhalePrefab(){
            var go=new GameObject("DeepSeekWhaleGirl");var sr=go.AddComponent<SpriteRenderer>();sr.sortingOrder=18;sr.sprite=LoadSprites(Root+"/Art/whale_sheet.png")["Whale_0_0"];go.AddComponent<DepthSort>();
            var anim=go.AddComponent<Animator>();anim.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Root+"/Animations/Whale.controller");go.AddComponent<AudioSource>();go.AddComponent<WhaleController>();
            var p=SavePrefab(go,Root+"/Prefabs/DeepSeekWhaleGirl.prefab");UnityEngine.Object.DestroyImmediate(go);return p;
        }
        static Dictionary<StationType,GameObject> CreateStationPrefabs(){
            var map=new Dictionary<StationType,GameObject>();var spriteNames=new[]{"computer","showcase","livestream","food_cabinet","upgrade"};
            var defs=new[]{(StationType.Computer,0,"电脑工位","开发外包 · 代码冲刺"),(StationType.Showcase,1,"项目展示台","整理并展示项目"),(StationType.Begging,2,"直播募集角","和大肥鱼一起敲盆"),(StationType.Feeding,3,"食物柜","花 25 Token 喂大肥鱼"),(StationType.Upgrade,4,"房间升级","改善设备与工作环境")};
            foreach(var d in defs){var go=new GameObject(d.Item3);var sr=go.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"/Art/"+spriteNames[d.Item2]+".png");sr.sortingOrder=8;var c=go.AddComponent<CircleCollider2D>();c.isTrigger=true;c.radius=1.2f;var w=go.AddComponent<WorldInteractable>();w.Configure(d.Item1,d.Item4);var footprint=new GameObject("FurnitureFootprint");footprint.layer=8;footprint.transform.SetParent(go.transform);footprint.transform.localPosition=new Vector3(0,-.7f,0);var solid=footprint.AddComponent<BoxCollider2D>();solid.size=d.Item1==StationType.Begging?new Vector2(2.5f,.3f):new Vector2(2.8f,.6f);var p=SavePrefab(go,Root+"/Prefabs/"+d.Item3+".prefab");UnityEngine.Object.DestroyImmediate(go);map[d.Item1]=p;}return map;
        }
        static GameObject SavePrefab(GameObject go,string path){return PrefabUtility.SaveAsPrefabAsset(go,path);}

        static void CreateMainScene(GameConfig config,GameObject playerPrefab,GameObject whalePrefab,Dictionary<StationType,GameObject> stations){
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var bg=new GameObject("Room_Background");var bgsr=bg.AddComponent<SpriteRenderer>();bgsr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"/Art/room_clean.png");bgsr.sortingOrder=-20;bg.transform.position=Vector3.zero;
            var camGo=new GameObject("Main Camera");var cam=camGo.AddComponent<Camera>();camGo.AddComponent<AudioListener>();cam.orthographic=true;cam.orthographicSize=4.7f;cam.backgroundColor=new Color(.035f,.055f,.11f);camGo.tag="MainCamera";camGo.transform.position=new Vector3(0,0,-10);cam.aspect=16f/9f;
            var root=new GameObject("World");
            var player=(GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);player.transform.position=new Vector3(0,-2.5f,0);player.transform.SetParent(root.transform);
            var whale=(GameObject)PrefabUtility.InstantiatePrefab(whalePrefab);whale.transform.position=new Vector3(1.2f,-2.1f,0);whale.transform.SetParent(root.transform);SetObj(whale.GetComponent<WhaleController>(),"player",player.transform);
            var poses=new Dictionary<StationType,Vector3>{{StationType.Computer,new Vector3(-5.4f,2.25f,0)},{StationType.Showcase,new Vector3(-1.6f,2.15f,0)},{StationType.Begging,new Vector3(3.4f,1.7f,0)},{StationType.Feeding,new Vector3(6.1f,-.6f,0)},{StationType.Upgrade,new Vector3(5.4f,-2.7f,0)}};
            var scales=new Dictionary<StationType,float>{{StationType.Computer,.72f},{StationType.Showcase,.68f},{StationType.Begging,.68f},{StationType.Feeding,.68f},{StationType.Upgrade,.62f}};
            SpriteRenderer upgradePreview=null;foreach(var kv in stations){var go=(GameObject)PrefabUtility.InstantiatePrefab(kv.Value);go.transform.position=poses[kv.Key];go.transform.localScale=Vector3.one*scales[kv.Key];go.transform.SetParent(root.transform);if(kv.Key==StationType.Upgrade){upgradePreview=go.GetComponent<SpriteRenderer>();upgradePreview.color=new Color(.45f,.5f,.6f,1);}}
            CreateBounds(root.transform);var bedInteraction=new GameObject("Bed_Interaction");bedInteraction.transform.SetParent(root.transform);bedInteraction.transform.position=new Vector3(-5.4f,-2.35f,0);var bedTrigger=bedInteraction.AddComponent<BoxCollider2D>();bedTrigger.isTrigger=true;bedTrigger.size=new Vector2(3.6f,1.7f);var bedWorld=bedInteraction.AddComponent<WorldInteractable>();bedWorld.Configure(StationType.Bed,"床 · 和大肥鱼说说未来");bedInteraction.AddComponent<DepthSort>();
            var upgrades=new GameObject[3];for(int i=0;i<3;i++){upgrades[i]=new GameObject("DeviceCorner_Level_"+(i+1));var sr=upgrades[i].AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"/Art/upgrade.png");sr.sortingOrder=9;sr.color=new[]{Color.white,new Color(1,.94f,.82f),new Color(.85f,.95f,1)}[i];upgrades[i].transform.position=poses[StationType.Upgrade];upgrades[i].transform.localScale=Vector3.one*(.62f+i*.04f);upgrades[i].SetActive(false);}
            foreach(var upgraded in upgrades)upgraded.AddComponent<DepthSort>();
            var gs=new GameObject("GameState").AddComponent<GameState>();SetObj(gs,"config",config);
            var ui=CreateUI(out var mini);
            var directorGo=new GameObject("GameDirector");var director=directorGo.AddComponent<GameDirector>();var audio=directorGo.AddComponent<AudioSource>();audio.playOnAwake=false;
            SetObj(director,"player",player.GetComponent<PlayerController>());SetObj(director,"whale",whale.GetComponent<WhaleController>());SetObj(director,"minigame",mini);SetObj(director,"sfx",audio);SetObj(director,"clips",LoadAudio());SetObj(director,"roomUpgrades",upgrades);
            SetObj(director,"upgradePreview",upgradePreview);var performanceSpot=new GameObject("LivestreamPerformanceSpot");performanceSpot.transform.position=new Vector3(3.35f,.2f,0);SetObj(director,"performanceSpot",performanceSpot.transform);
            PresentationBuilder.Configure(player,whale,mini,ui,director,cam);
            EditorSceneManager.SaveScene(scene,Root+"/Scenes/MainRoom.unity");
        }
        static void CreateBounds(Transform parent){foreach(var d in new[]{("Top",new Vector2(0,4.45f),new Vector2(16.2f,.4f)),("Bottom",new Vector2(0,-4.35f),new Vector2(16.2f,.4f)),("Left",new Vector2(-8.05f,0),new Vector2(.4f,9)),("Right",new Vector2(8.05f,0),new Vector2(.4f,9)),("Bed",new Vector2(-5.4f,-2.35f),new Vector2(4.4f,2.0f)),("Kitchen",new Vector2(6.7f,-.4f),new Vector2(2.4f,4.2f))}){var go=new GameObject("Collider_"+d.Item1);go.layer=8;go.transform.SetParent(parent);go.transform.position=d.Item2;var c=go.AddComponent<BoxCollider2D>();c.size=d.Item3;}}

        static GameUI CreateUI(out MinigameController mini){
            font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");var canvasGo=new GameObject("UI_Canvas");var canvas=canvasGo.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=Camera.main;canvas.planeDistance=1;canvas.sortingOrder=1000;canvasGo.AddComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;canvasGo.GetComponent<CanvasScaler>().referenceResolution=new Vector2(1600,900);canvasGo.AddComponent<GraphicRaycaster>();
            var top=Panel(canvasGo.transform,"TopHUD",new Color(.025f,.045f,.09f,.9f),new Vector2(0,1),new Vector2(1,1),new Vector2(0,-86),new Vector2(0,0));
            var token=Label(top.transform,"Token",28,TextAnchor.MiddleLeft,new Vector2(.02f,.12f),new Vector2(.26f,.88f),Color.white);var hunger=Label(top.transform,"Hunger",23,TextAnchor.MiddleLeft,new Vector2(.29f,.12f),new Vector2(.53f,.88f),Color.white);var objective=Label(top.transform,"Objective",22,TextAnchor.MiddleRight,new Vector2(.52f,.12f),new Vector2(.98f,.88f),new Color(.75f,.9f,1));
            var hb=Panel(top.transform,"HungerBar",new Color(.08f,.12f,.2f,1),new Vector2(.29f,.08f),new Vector2(.52f,.18f),Vector2.zero,Vector2.zero);var hf=Panel(hb.transform,"Fill",new Color(.18f,.72f,.92f,1),Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero).GetComponent<Image>();hf.type=Image.Type.Filled;hf.fillMethod=Image.FillMethod.Horizontal;
            var prompt=Label(canvasGo.transform,"Prompt",27,TextAnchor.MiddleCenter,new Vector2(.3f,.08f),new Vector2(.7f,.15f),Color.white);prompt.gameObject.AddComponent<Outline>().effectColor=new Color(0,0,0,.8f);
            var toast=Label(canvasGo.transform,"Toast",25,TextAnchor.MiddleCenter,new Vector2(.25f,.82f),new Vector2(.75f,.9f),new Color(1,.88f,.5f));toast.gameObject.SetActive(false);
            var miniPanel=Panel(canvasGo.transform,"MinigamePanel",new Color(.02f,.035f,.08f,.96f),new Vector2(.12f,.13f),new Vector2(.88f,.84f),Vector2.zero,Vector2.zero);
            var title=Label(miniPanel.transform,"Title",36,TextAnchor.MiddleCenter,new Vector2(.05f,.78f),new Vector2(.95f,.96f),new Color(.65f,.9f,1));var tokenStatus=Label(miniPanel.transform,"TokenStatus",20,TextAnchor.MiddleRight,new Vector2(.72f,.92f),new Vector2(.96f,.99f),new Color(1,.85f,.35f));var ins=Label(miniPanel.transform,"Instruction",21,TextAnchor.MiddleCenter,new Vector2(.08f,.61f),new Vector2(.92f,.78f),Color.white);var seq=Label(miniPanel.transform,"Sequence",44,TextAnchor.MiddleCenter,new Vector2(.1f,.3f),new Vector2(.9f,.58f),Color.white);var res=Label(miniPanel.transform,"Result",28,TextAnchor.MiddleCenter,new Vector2(.1f,.08f),new Vector2(.9f,.25f),new Color(1,.78f,.3f));
            var bar=Panel(miniPanel.transform,"Timeline",new Color(.1f,.15f,.24f,1),new Vector2(.13f,.27f),new Vector2(.87f,.32f),Vector2.zero,Vector2.zero);var fill=Panel(bar.transform,"MovingFill",new Color(.25f,.78f,1,1),Vector2.zero,Vector2.one,Vector2.zero,Vector2.zero).GetComponent<Image>();fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;var tgt=Panel(bar.transform,"PerfectZone",new Color(1f,.72f,.08f,1f),new Vector2(.62f,-.3f),new Vector2(.82f,1.3f),Vector2.zero,Vector2.zero).GetComponent<Image>();var movingNode=Panel(bar.transform,"MovingGoldenNode",new Color(1f,.95f,.45f,1f),new Vector2(0,.5f),new Vector2(0,.5f),new Vector2(-14,-14),new Vector2(14,14)).GetComponent<Image>();movingNode.gameObject.SetActive(false);
            mini=miniPanel.AddComponent<MinigameController>();var pitchButtons=MiniUi.Create(miniPanel.transform);SetObj(mini,"pitchButtons",pitchButtons);SetObj(mini,"movingNode",movingNode);SetObj(mini,"tokenStatus",tokenStatus);SetObj(mini,"panel",miniPanel);SetObj(mini,"title",title);SetObj(mini,"instruction",ins);SetObj(mini,"sequence",seq);SetObj(mini,"result",res);SetObj(mini,"timeline",fill);SetObj(mini,"target",tgt);miniPanel.SetActive(false);
            seq.rectTransform.anchorMin=new Vector2(.07f,.21f);seq.rectTransform.anchorMax=new Vector2(.93f,.68f);
            title.rectTransform.anchorMin=new Vector2(.05f,.86f);title.rectTransform.anchorMax=new Vector2(.95f,.97f);
            ins.rectTransform.anchorMin=new Vector2(.06f,.75f);ins.rectTransform.anchorMax=new Vector2(.94f,.86f);
            res.rectTransform.anchorMin=new Vector2(.07f,.02f);res.rectTransform.anchorMax=new Vector2(.93f,.13f);
            ((RectTransform)bar.transform).anchorMin=new Vector2(.13f,.15f);((RectTransform)bar.transform).anchorMax=new Vector2(.87f,.19f);
            string catalogPath=Root+"/Config/Contracts.asset";var catalog=AssetDatabase.LoadAssetAtPath<ContractCatalog>(catalogPath);if(!catalog){catalog=ScriptableObject.CreateInstance<ContractCatalog>();AssetDatabase.CreateAsset(catalog,catalogPath);}Unity25ContractsBuilder.Apply(catalog);SetObj(mini,"catalog",catalog);
            canvasGo.AddComponent<SystemUIFont>();
            var ui=canvasGo.AddComponent<GameUI>();SetObj(ui,"tokenText",token);SetObj(ui,"hungerText",hunger);SetObj(ui,"objectiveText",objective);SetObj(ui,"promptText",prompt);SetObj(ui,"toastText",toast);SetObj(ui,"hungerFill",hf);return ui;
        }
        static GameObject Panel(Transform parent,string name,Color color,Vector2 amin,Vector2 amax,Vector2 offMin,Vector2 offMax){var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=amin;r.anchorMax=amax;r.offsetMin=offMin;r.offsetMax=offMax;go.GetComponent<Image>().color=color;return go;}
        static Text Label(Transform parent,string name,int size,TextAnchor anchor,Vector2 amin,Vector2 amax,Color color){var go=new GameObject(name,typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=amin;r.anchorMax=amax;r.offsetMin=r.offsetMax=Vector2.zero;var t=go.GetComponent<Text>();t.font=font;t.fontSize=size;t.alignment=anchor;t.color=color;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;return t;}
        static void SetObj(UnityEngine.Object target,string name,UnityEngine.Object value){var s=new SerializedObject(target);s.FindProperty(name).objectReferenceValue=value;s.ApplyModifiedPropertiesWithoutUndo();}
        static void SetObj(UnityEngine.Object target,string name,UnityEngine.Object[] values){var s=new SerializedObject(target);var p=s.FindProperty(name);p.arraySize=values.Length;for(int i=0;i<values.Length;i++)p.GetArrayElementAtIndex(i).objectReferenceValue=values[i];s.ApplyModifiedPropertiesWithoutUndo();}

        static void CreateAudio(){WriteTone("token_gain.wav",880,110,.16f);WriteTone("eat.wav",360,180,.22f);WriteTone("dance.wav",new[]{523,659,784,659,880,784},95,.18f);WriteTone("upgrade.wav",new[]{440,554,659,880},120,.18f);WriteTone("bowl_hit.wav",180,95,.32f);}
        static void WriteTone(string name,double hz,int ms,float volume){WriteTone(name,new[]{(int)hz},ms,volume);}
        static void WriteTone(string name,int[] notes,int ms,float volume){int rate=44100,samples=rate*ms*notes.Length/1000;var data=new short[samples];int each=rate*ms/1000;for(int i=0;i<samples;i++){double f=notes[Math.Min(notes.Length-1,i/each)];double env=1.0-(i%each)/(double)each;data[i]=(short)(Math.Sin(2*Math.PI*f*i/rate)*short.MaxValue*volume*env);}string path=Root+"/Audio/"+name;using(var bw=new BinaryWriter(File.Create(path))){int bytes=samples*2;bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));bw.Write(36+bytes);bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));bw.Write(16);bw.Write((short)1);bw.Write((short)1);bw.Write(rate);bw.Write(rate*2);bw.Write((short)2);bw.Write((short)16);bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));bw.Write(bytes);foreach(var s in data)bw.Write(s);}}
        static AudioClip[] LoadAudio(){return new[]{"token_gain.wav","eat.wav","dance.wav","upgrade.wav","bowl_hit.wav"}.Select(n=>AssetDatabase.LoadAssetAtPath<AudioClip>(Root+"/Audio/"+n)).ToArray();}
        static void SetBuildSettings(){EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Root+"/Scenes/MainRoom.unity",true)};PlayerSettings.productName="Token Survival - 养一只大肥鱼";PlayerSettings.companyName="Indie Rebuild";PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.allowedAutorotateToPortrait=false;PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;PlayerSettings.allowedAutorotateToLandscapeRight=true;PlayerSettings.allowedAutorotateToLandscapeLeft=true;}
    }
}
#endif

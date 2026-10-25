using System;
using UnityEngine;
using UnityEngine.UI;

namespace TokenSurvival {
    public sealed class MinigameController : MonoBehaviour {
        public enum Mode { CodeSprint, ProjectPitch, BowlBeat }
        [SerializeField] GameObject panel;
        [SerializeField] Text title, instruction, sequence, result;
        [SerializeField] Image timeline, target;
        [SerializeField] Button[] pitchButtons;
        [SerializeField] Image movingNode;
        [SerializeField] Text tokenStatus;
        [SerializeField] ContractCatalog catalog;
        public bool IsRunning { get; private set; }
        public Mode CurrentMode=>mode;
        public KeyCode ExpectedKey=>expected;
        Mode mode; Action<Mode,int> done; float phase; int attempts, score; KeyCode expected;
        int selected=-1, contractIndex; bool waiting;
        readonly KeyCode[] keys={KeyCode.W,KeyCode.A,KeyCode.S,KeyCode.D};
        void Awake(){if(pitchButtons!=null)for(int i=0;i<pitchButtons.Length;i++){int n=i;if(pitchButtons[i])pitchButtons[i].onClick.AddListener(()=>SubmitPitch(keys[n]));}}
        public void Begin(Mode value, Action<Mode,int> callback){
            mode=value; done=callback; phase=0; attempts=0; score=0; waiting=false; IsRunning=true; panel.SetActive(true); result.text="";
            if(tokenStatus)tokenStatus.text="当前 Token："+GameState.Instance.Tokens;
            title.text=value==Mode.CodeSprint?"代码冲刺 · 合并请求":value==Mode.ProjectPitch?"项目路演 · 演示节奏":"赛博讨 Token · 敲盆节拍";
            instruction.text=value==Mode.CodeSprint?"阅读代码与失败测试；按 1/2/3 选择补丁，ENTER 运行测试":value==Mode.ProjectPitch?"社区提问：点击或按亮起的 W / A / S / D 按钮回应":"看清时间轴上加粗的金色节点，节点到达中央时按 SPACE 敲盆";
            sequence.fontSize=value==Mode.CodeSprint?19:32;
            var rect=(RectTransform)panel.transform;
            rect.anchorMin=value==Mode.BowlBeat?new Vector2(.08f,.1f):new Vector2(.12f,.13f);
            rect.anchorMax=value==Mode.BowlBeat?new Vector2(.55f,.46f):new Vector2(.88f,.84f);
            title.fontSize=value==Mode.BowlBeat?24:36;instruction.fontSize=value==Mode.BowlBeat?17:21;
            timeline.transform.parent.gameObject.SetActive(value==Mode.BowlBeat);
            if(pitchButtons!=null)for(int i=0;i<pitchButtons.Length;i++)if(pitchButtons[i])pitchButtons[i].gameObject.SetActive(value==Mode.ProjectPitch);
            Next();
        }
        void Update(){
            if(!IsRunning)return;
            if(tokenStatus)tokenStatus.text="当前 Token："+GameState.Instance.Tokens;
            if(Input.GetKeyDown(KeyCode.Escape)){Cancel();return;}
            if(waiting)return;
            if(mode==Mode.CodeSprint){for(int i=0;i<3;i++)if(Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1+i))||Input.GetKeyDown((KeyCode)((int)KeyCode.Keypad1+i))){selected=i;result.text="已选补丁 "+(i+1)+"，ENTER 运行测试";}if(Input.GetKeyDown(KeyCode.Return)&&selected>=0)SubmitPatch(selected);return;}
            if(mode==Mode.ProjectPitch){ phase+=Time.deltaTime; timeline.fillAmount=Mathf.Clamp01(1-phase/2.4f); if(Input.GetKeyDown(expected)){SubmitPitch(expected);}else if(phase>2.4f)Hit(0); }
            else {phase+=Time.deltaTime*.85f;timeline.fillAmount=Mathf.PingPong(phase,1);if(Input.GetKeyDown(KeyCode.Space))SubmitBeat();}
            if(movingNode){movingNode.gameObject.SetActive(mode==Mode.BowlBeat);if(mode==Mode.BowlBeat){float p=Mathf.PingPong(phase,1);var r=movingNode.rectTransform;r.anchorMin=new Vector2(p,.5f);r.anchorMax=new Vector2(p,.5f);r.anchoredPosition=Vector2.zero;}}
        }
        public void SubmitPatch(int patch){if(IsRunning&&mode==Mode.CodeSprint&&!waiting)Hit(patch==catalog.contracts[contractIndex].correctPatch?1:0);}
        public void Cancel(){if(!IsRunning)return;CancelInvoke();IsRunning=false;panel.SetActive(false);var cb=done;done=null;cb?.Invoke(mode,-1);}
        public void SubmitPitch(KeyCode key){if(IsRunning&&mode==Mode.ProjectPitch&&!waiting)Hit(key==expected&&phase<1.5f?1:0);}
        public void SubmitBeat(){if(!IsRunning||mode!=Mode.BowlBeat||waiting)return;Hit(Mathf.Abs(Mathf.PingPong(phase,1)-.72f)<.14f?1:0);GameDirector.Instance.BowlHit();}
        void Hit(int points){if(waiting)return;waiting=true;score+=points;attempts++;result.text=points>0?(mode==Mode.CodeSprint?"测试通过 · 补丁已合并":"回应成功 · 赞助 +"):"未通过，本次收益降低";Invoke(nameof(Advance),.8f);}
        void Advance(){if(attempts>=5)Finish();else{waiting=false;Next();}}
        void Next(){phase=0;selected=-1;if(mode==Mode.CodeSprint){contractIndex=(GameState.Instance.JobsCompleted+attempts)%catalog.contracts.Length;var c=catalog.contracts[contractIndex];sequence.text=c.title+"\n\n"+c.code+"\n\n测试："+c.failedTest+"\n\n1  "+c.patches[0]+"\n2  "+c.patches[1]+"\n3  "+c.patches[2];}else if(mode==Mode.ProjectPitch){int n=UnityEngine.Random.Range(0,keys.Length);expected=keys[n];sequence.text=new[]{"社区：请展示修复后的测试结果","社区：请展示网页布局","社区：请展示性能优化","社区：请展示需求变更对比"}[n]+"\n按 "+expected+" 展示";}else sequence.text=$"{attempts+1} / 5";}
        void Finish(){ if(!IsRunning)return; CancelInvoke(); IsRunning=false; panel.SetActive(false); var cb=done; done=null; cb?.Invoke(mode,score); }
    }
}

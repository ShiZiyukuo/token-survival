using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TokenSurvival {
    public sealed class GameUI : MonoBehaviour {
        public static GameUI Instance { get; private set; }
        [SerializeField] Text tokenText, hungerText, objectiveText, promptText, toastText;
        [SerializeField] Image hungerFill;
        Coroutine toastRoutine;
        void Awake(){Instance=this;}
        void Start(){GameState.Instance.Changed+=Refresh; Refresh(); Toast("靠近发光物件按 E 互动");}
        public void Refresh(){ var s=GameState.Instance; tokenText.text=$"TOKEN  {s.Tokens}"; hungerText.text=$"大肥鱼饱腹  {Mathf.CeilToInt(s.Hunger)}%"; hungerFill.fillAmount=s.Hunger/100f; objectiveText.text=s.JobsCompleted==0?"去电脑前完成第一份外包":s.Hunger<45?"到食物柜喂大肥鱼":"接单 / 展示 / 直播   ·   设备角升级"; }
        public void SetPrompt(string value){promptText.text=value;}
        public void SetObjective(string value){objectiveText.text=value;}
        public void Toast(string value){if(toastRoutine!=null)StopCoroutine(toastRoutine);toastRoutine=StartCoroutine(ToastRoutine(value));}
        IEnumerator ToastRoutine(string value){toastText.text=value; toastText.gameObject.SetActive(true); yield return new WaitForSeconds(3); toastText.gameObject.SetActive(false);}
    }
}

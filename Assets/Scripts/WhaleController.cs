using System.Collections;
using UnityEngine;

namespace TokenSurvival {
    [RequireComponent(typeof(Animator), typeof(AudioSource))]
    public sealed class WhaleController : MonoBehaviour {
        public enum Mood { Idle, Follow, Hungry, Eat, Happy, Dance, Bowl }
        [SerializeField] Transform player;
        Animator animator; AudioSource audioSource; Mood mood; string current;
        Vector2 facing=Vector2.down;
        bool walkingToStage;
        public Mood CurrentMood => mood;
        public bool IsPerforming=>walkingToStage||mood==Mood.Eat||mood==Mood.Happy||mood==Mood.Dance||mood==Mood.Bowl;
        void Awake() { animator=GetComponent<Animator>(); audioSource=GetComponent<AudioSource>(); }
        void Update() {
            if(walkingToStage)return;
            if (mood==Mood.Eat || mood==Mood.Happy || mood==Mood.Dance || mood==Mood.Bowl) return;
            if (GameState.Instance.Hunger < 24) { SetMood(Mood.Hungry); return; }
            float dist=Vector2.Distance(transform.position, player.position);
            if(dist>2.4f){facing=(player.position-transform.position).normalized;SetMood(Mood.Follow);MoveToward(player.position-(Vector3)(player.GetComponent<PlayerController>().LastDirection*1.2f));}
            else SetMood(Mood.Idle);
        }
        public void PlayFeed() { StopAllCoroutines(); StartCoroutine(FeedRoutine()); }
        public void Celebrate(){StopAllCoroutines();StartCoroutine(Celebration());}
        public void PlayCheer(){StopAllCoroutines();StartCoroutine(CheerRoutine());}
        IEnumerator CheerRoutine(){SetMood(Mood.Bowl);animator.Play("BowlReady",0,0);yield return new WaitForSeconds(.35f);PlayBowlHit();yield return new WaitForSeconds(.8f);SetMood(Mood.Idle);}
        IEnumerator Celebration(){SetMood(Mood.Dance);GameDirector.Instance.PlaySfx(2);yield return new WaitForSeconds(4.2f);SetMood(Mood.Idle);}
        IEnumerator FeedRoutine(){ SetMood(Mood.Eat); GameDirector.Instance.PlaySfx(1); yield return new WaitForSeconds(1.5f); SetMood(Mood.Happy); yield return new WaitForSeconds(1f); SetMood(Mood.Dance); GameDirector.Instance.PlaySfx(2); yield return new WaitForSeconds(4.2f); SetMood(Mood.Idle); }
        public void BeginBowl(){ StopAllCoroutines(); SetMood(Mood.Bowl); animator.Play("BowlReady",0,0); }
        public void PlayBowlHit(){if(mood!=Mood.Bowl)return;animator.Play("Bowl",0,0);}
        public void BowlImpact(){GameDirector.Instance.PlaySfx(4);}
        public IEnumerator WalkToStage(Vector3 destination){
            StopAllCoroutines();walkingToStage=true;float remaining=5;
            while(Vector2.Distance(transform.position,destination)>.15f&&remaining>0){remaining-=Time.deltaTime;facing=(destination-transform.position).normalized;SetMood(Mood.Follow);MoveToward(destination);yield return null;}
            walkingToStage=false;BeginBowl();
        }
        void MoveToward(Vector3 destination){var next=Vector2.MoveTowards(transform.position,destination,2.25f*Time.deltaTime);var delta=next-(Vector2)transform.position;var feet=(Vector2)transform.position+Vector2.down*.65f;var hit=Physics2D.CircleCast(feet,.2f,delta.normalized,delta.magnitude,1<<8);if(!hit.collider)transform.position=next;else{var x=new Vector2(delta.x,0);if(x.sqrMagnitude>.000001f&&!Physics2D.CircleCast(feet,.2f,x.normalized,x.magnitude,1<<8).collider)transform.position+=(Vector3)x;var y=new Vector2(0,delta.y);if(y.sqrMagnitude>.000001f&&!Physics2D.CircleCast((Vector2)transform.position+Vector2.down*.65f,.2f,y.normalized,y.magnitude,1<<8).collider)transform.position+=(Vector3)y;}}
        public void EndBowl(){ SetMood(Mood.Happy); StartCoroutine(ReturnIdle()); }
        IEnumerator ReturnIdle(){ yield return new WaitForSeconds(1.2f); SetMood(Mood.Idle); }
        void SetMood(Mood next){mood=next;string name=next.ToString();if(next==Mood.Follow)name+=Mathf.Abs(facing.x)>Mathf.Abs(facing.y)?(facing.x<0?"Left":"Right"):(facing.y>0?"Up":"Down");if(current==name)return;current=name;animator.Play(name,0,0);}
    }
}

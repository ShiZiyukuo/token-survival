using UnityEngine;

namespace TokenSurvival {
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public sealed class PlayerController : MonoBehaviour {
        Rigidbody2D body; Animator animator; Vector2 input; string current;
        [SerializeField] float speed = 4.2f;
        public bool InputLocked { get; set; }
        public Vector2 LastDirection { get; private set; } = Vector2.down;
        public Vector2 ValidationInput { get; set; }
        public bool ValidationMode { get; set; }

        void Awake() { body = GetComponent<Rigidbody2D>(); animator = GetComponent<Animator>(); }
        void Update() {
            input = InputLocked || (StoryDirector.Instance && StoryDirector.Instance.Blocking) ? Vector2.zero : ValidationMode ? ValidationInput.normalized : Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) + TouchJoystick.Axis, 1);
            if (input.sqrMagnitude > .01f) LastDirection = input;
            string dir = Mathf.Abs(LastDirection.x) > Mathf.Abs(LastDirection.y) ? (LastDirection.x < 0 ? "Left" : "Right") : (LastDirection.y > 0 ? "Up" : "Down");
            SetAnimation((input.sqrMagnitude > .01f ? "Walk" : "Idle") + dir);
        }
        void FixedUpdate() { body.velocity = input * speed; }
        void SetAnimation(string name) { if (current == name) return; current = name; animator.Play(name, 0, 0); }
    }
}

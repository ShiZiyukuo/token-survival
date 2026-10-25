#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
namespace TokenSurvival.EditorTools {
    public static class PlayModeValidation {
        public static void Run(){EditorSceneManager.OpenScene("Assets/TokenSurvival/Scenes/MainRoom.unity");EditorApplication.isPlaying=true;}
    }
}
#endif

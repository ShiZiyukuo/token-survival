using System;
using UnityEngine;

namespace TokenSurvival {
    [Serializable] public struct SaveData { public bool valid; public int tokens; public float hunger; public int comfort; public int jobs; public int chapter, feeds, showcases, performances; public bool bedVisited, bowlAfterSeen; }
    public static class SaveSystem {
        const string Key = "TokenSurvivalRebuild_Save_v1";
        static bool Validation => Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-tokenSmoke"||a=="-storySmoke");
        public static SaveData Load() { if(Validation)return default;var json = PlayerPrefs.GetString(Key, ""); return string.IsNullOrEmpty(json) ? default : JsonUtility.FromJson<SaveData>(json); }
        public static void Save(GameState s) {if(Validation)return; var d = new SaveData { valid=true, tokens=s.Tokens, hunger=s.Hunger, comfort=s.ComfortLevel, jobs=s.JobsCompleted, chapter=s.StoryChapter,feeds=s.Feeds,showcases=s.Showcases,performances=s.Performances,bedVisited=s.BedVisited,bowlAfterSeen=s.BowlAfterSeen }; PlayerPrefs.SetString(Key, JsonUtility.ToJson(d)); PlayerPrefs.Save(); }
        public static void Reset() { PlayerPrefs.DeleteKey(Key); }
    }
}

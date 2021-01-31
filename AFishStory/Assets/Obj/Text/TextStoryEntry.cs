using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct TextEntry {
    public float wait;
    public float duration;
    public string text;
    public GameEvent trigger;
    public AudioClip audio;
}

[CreateAssetMenu(fileName = "TextStoryEntry", menuName = "AFishStory/TextStoryEntry", order = 0)]
public class TextStoryEntry : ScriptableObject {
    public List<TextEntry> texts;
}
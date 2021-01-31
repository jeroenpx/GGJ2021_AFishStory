using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMsg : MonoBehaviour
{
    public StoryMgr mgr;
    public TextStoryEntry storyEntry;

    private bool started = false;

    public GameEvent onFound;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player") {
            if(!started) {
                started = true;
                mgr.StartScenario(storyEntry);
                if(onFound) {
                    onFound.Raise();
                }
            }
        }
    }
}

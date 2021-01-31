using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryMgr : MonoBehaviour
{
    private bool running;
    private int scenarioNum = 0;

    public TextStoryEntry introScenario;

    public TextRevealer revealer;

    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        if(introScenario) {
            StartScenario(introScenario);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsScenarioRunning () {
        return running;
    }

    public void StartScenario(TextStoryEntry scenario) {
        running = true;
        StopAllCoroutines();
        revealer.RestartWithText("");
        StartCoroutine(RunScenario(scenario));
    }

    /**
     * Private - run one of the scenarios
     */
    private IEnumerator RunScenario(TextStoryEntry scenario) {
        scenarioNum++;
        int myScenario = scenarioNum;

        int idx = 0;
        while (scenarioNum == myScenario && idx < scenario.texts.Count) {
            TextEntry e = scenario.texts[idx];
            float wait = e.wait;
            if(wait>0) {
                yield return new WaitForSeconds(wait);
            }
            if(e.trigger) {
                e.trigger.Raise();
            }
            if(e.audio) {
                // Stop previous audio if any - NOTE: we don't fade out so you might hear a "click"
                // Whatever
                source.Stop();

                // Play new audio!
                source.PlayOneShot(e.audio);
            }
            Debug.Log("Trigger text: "+e.text);
            bool moved = false;
            revealer.RestartWithText(e.text);
            IEnumerator sub = revealer.RevealNextParagraph();
            while(sub.MoveNext()) {
                if(!moved) {
                    Debug.Log("MoveNext");
                    moved = true;
                }
                yield return sub.Current;
            }
            float duration = e.duration;
            if(duration>0) {
                yield return new WaitForSeconds(duration);
            }
            revealer.RestartWithText("");
            idx++;
        }
        yield return null;

        if(scenarioNum == myScenario) {
            running = false;
        }
    }
}

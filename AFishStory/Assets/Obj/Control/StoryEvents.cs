using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEvents : MonoBehaviour
{
    public PlayerControls controls;
    public Rigidbody2D fish;
    public Animator fishAnimator;
    public CamFollow camControl;
    public AudioSource musicAudioSource;
    public Transform escapePoint;

    public Animator myAnimator;
    public StoryMgr mgr;
    public TextStoryEntry reIntro;

    private bool secondTime = false;

    private void Start() {
        fishAnimator.SetTrigger("StartIntro");
    }

    public void EventEnableControls (){
        if(!secondTime) {
            secondTime = true;
            controls.controlsActive = true;
            camControl.lookAheadEnabled = true;
        } else {
            // Fade out to black & back to main menu
            myAnimator.SetTrigger("GamEnd");
        }
    }

    public void EventDisableControls (){
        controls.controlsActive = false;
        camControl.lookAheadEnabled = false;
        fish.gravityScale = 0;// Stop gravity as well
        fishAnimator.SetBool("Swim", false);
    }

    public void EventUpsideDown () {
        fishAnimator.SetBool("UpsideDown", true);
    }
    public void EventRotateRightUp () {
        fishAnimator.SetBool("UpsideDown", false);
    }

    public void EventEyesClosed () {
        fishAnimator.SetBool("CloseEyes", true);
    }
    
    public void EventEyesClosedReopen () {
        fishAnimator.SetBool("CloseEyes", false);
    }
    
    public void EventEyesHalfClosed () {
        fishAnimator.SetBool("CloseEyes", true);
    }
    
    public void EventEyesHalfClosedReopen () {
        fishAnimator.SetBool("CloseEyes", false);
    }
    
    public void EventEyesBlink () {
        fishAnimator.SetTrigger("Blink");
    }

    public void EventThink () {
        Debug.Log("Think Triggered");
        fishAnimator.SetTrigger("Think");
    }

    public void EventShrug () {
        fishAnimator.SetTrigger("Shrug");
    }

    public void EventAngry () {
        fishAnimator.SetTrigger("Angry");
    }

    public void EventSadEndFace () {
        fishAnimator.SetTrigger("VerySadEnd");
    }

    public void TeleportFishToOrigin () {
        fish.transform.position = new Vector3(0.02f, -1.97f,0);
        fish.gravityScale = 1;// Restart gravity
    }

    public void StopMusic () {
        StartCoroutine(StopMusicCo ());
    }

    public void StartFlee () {
        controls.StartAutoSwimTo(escapePoint.position);
    }

    public void StopFlee () {
        controls.StopAutoSwim();
    }

    public void StartBeginSecondTime() {
        myAnimator.SetTrigger("Restart");
        fishAnimator.SetTrigger("RestartIntro");
        mgr.StartScenario(reIntro);
    }

    public void BackToMainMenu() {
        Debug.Log("Back to main menu");
    }

    public IEnumerator StopMusicCo () {
        float time = 0;
        float initialVolume = musicAudioSource.volume;
        while(time < 1) {
            musicAudioSource.volume = initialVolume * (1-time);
            yield return null;
            time += Time.deltaTime;
        }
    }
}

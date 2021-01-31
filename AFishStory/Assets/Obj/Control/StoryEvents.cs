using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryEvents : MonoBehaviour
{
    public PlayerControls controls;
    public Rigidbody2D fish;
    public Animator fishAnimator;
    public CamFollow camControl;

    public void EventEnableControls (){
        controls.controlsActive = true;
        camControl.lookAheadEnabled = true;
    }

    public void EventDisableControls (){
        controls.controlsActive = false;
        camControl.lookAheadEnabled = false;
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

    public void TeleportFishToOrigin () {
        fish.MovePosition(Vector3.zero);
    }
}

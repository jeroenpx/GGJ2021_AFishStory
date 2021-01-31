using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBtnLogic : MonoBehaviour
{
    public Animator animator;
    public Animator mainMenuAnimator;

    public AudioClip enter;
    public AudioClip click;
    public AudioSource source;

    private void OnMouseEnter() {
        source.PlayOneShot(enter);
        animator.SetBool("Hover", true);
    }

    private void OnMouseExit() {
        //source.PlayOneShot(enter);
        animator.SetBool("Hover", false);
    }

    private void OnMouseUpAsButton() {
        source.PlayOneShot(click);
        mainMenuAnimator.SetTrigger("GotoGame");
    }
}

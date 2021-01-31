using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBtnLogic : MonoBehaviour
{
    public Animator animator;
    public Animator mainMenuAnimator;

    private void OnMouseEnter() {
        animator.SetBool("Hover", true);
    }

    private void OnMouseExit() {
        animator.SetBool("Hover", false);
    }

    private void OnMouseUpAsButton() {
        mainMenuAnimator.SetTrigger("GotoGame");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyRandomizer : MonoBehaviour
{
    public Animator animator;
    public float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init() {
        yield return new WaitForSeconds(time);
        animator.SetTrigger("Restart");
    }
}

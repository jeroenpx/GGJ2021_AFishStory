using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMushroom : MonoBehaviour
{
    
    public Transform target;

    public AudioClip[] clips;

    public AudioSource source;

    public Animator animator;

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            other.gameObject.GetComponent<PlayerControls>().DashTo(target.position);

            // Play sound
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);

            // Play animation
            animator.SetTrigger("Hit");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private float startHoldDownTime;

    public float dashTreshhold = .3f;
    public float dashDuration = .3f;
    public float dashForce = 100f;
    public float mushroomDashForce = 100f;
    public float swimForce = 100f;
    public float autoSwimForce = 5f;

    public Rigidbody2D body;

    public Animator animator;

    public bool controlsActive = false;

    public AudioSource source;
    public AudioClip dashSound;
    public Vector2 pitchRange;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void PlayDashSound () {
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(dashSound);
    }

    Vector3 GetMouseInScene() {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        return worldPosition;
    }

    IEnumerator DashCoroutine () {
        Debug.Log("[Dash] Start");
        animator.SetTrigger("Dash");
        PlayDashSound ();
        float startTime = Time.time;
        Vector3 initialDir = GetMouseInScene() - transform.position;


        while(Time.time - startTime < dashDuration) {
            yield return new WaitForFixedUpdate();
            Vector3 dir = GetMouseInScene() - transform.position;
            body.AddForce(dir.normalized*dashForce);
            SendMessage("OnDash", GetMouseInScene());
        }

        Debug.Log("[Dash] End");
    }

    IEnumerator DashCoroutineWithFixedTarget (Vector3 target) {
        Debug.Log("[AutoDash] Start");
        animator.SetTrigger("Dash");
        animator.SetTrigger("HappyTrigger");
        PlayDashSound ();
        float startTime = Time.time;
        Vector3 initialDir = target - transform.position;


        while(Time.time - startTime < dashDuration) {
            yield return new WaitForFixedUpdate();
            Vector3 dir = target - transform.position;
            body.AddForce(dir.normalized*mushroomDashForce);
            SendMessage("OnDash", target);
        }

        Debug.Log("[AutoDash] End");
    }

    public void StartAutoSwimTo(Vector3 position) {
        StartCoroutine(AutoSwimCoroutine(position));
    }

    IEnumerator AutoSwimCoroutine(Vector3 pos) {
        while(true) {
            Vector3 dir = pos - transform.position;
            body.AddForce(dir.normalized*autoSwimForce);
            SendMessage("OnSwim", pos);
            animator.SetBool("Swim", true);
            yield return null;
        }
    }

    public void StopAutoSwim() {
        StopAllCoroutines();
        animator.SetBool("Swim", false);
    }

    void DoDash() {
        // Execute Dash!
        StartCoroutine(DashCoroutine ());
    }

    /**
     * The mushroom does this!
     */
    public void DashTo(Vector3 target) {
        body.velocity = Vector2.zero;
        StartCoroutine(DashCoroutineWithFixedTarget (target));
    }

    void Update() {
        if(!controlsActive) {
            return;
        }
        if(Input.GetMouseButtonDown(0)) {
            // Click
            //startHoldDownTime = Time.time;

            if((GetMouseInScene() - transform.position).magnitude > 2f) {
                DoDash();
            }
        }

        if(Input.GetMouseButtonUp(0)) {
            // Release button -> potentially "swooosh!"
            /*float clickDuration = Time.time - startHoldDownTime;
            if(clickDuration < dashTreshhold) {
                // Dash
                DoDash();
            }*/
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!controlsActive) {
            return;
        }
        if(Input.GetMouseButton(0)) {
            Vector3 dir = GetMouseInScene() - transform.position;
            body.AddForce(dir.normalized*swimForce);
            SendMessage("OnSwim", GetMouseInScene());
            animator.SetBool("Swim", true);
        } else {
            animator.SetBool("Swim", false);
        }
    }
}

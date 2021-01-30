using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private float startHoldDownTime;

    public float dashTreshhold = .3f;
    public float dashDuration = .3f;
    public float dashForce = 100f;
    public float swimForce = 100f;

    public Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 GetMouseInScene() {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        return worldPosition;
    }

    IEnumerator DashCoroutine () {
        Debug.Log("[Dash] Start");
        float startTime = Time.time;
        Vector3 initialDir = GetMouseInScene() - transform.position;


        while(Time.time - startTime < dashDuration) {
            yield return new WaitForFixedUpdate();
            Vector3 dir = GetMouseInScene() - transform.position;
            body.AddForce(dir.normalized*dashForce);
        }

        Debug.Log("[Dash] End");
    }

    void DoDash() {
        // Execute Dash!
        StartCoroutine(DashCoroutine ());
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            // Click
            startHoldDownTime = Time.time;
        }

        if(Input.GetMouseButtonUp(0)) {
            // Release button -> potentially "swooosh!"
            float clickDuration = Time.time - startHoldDownTime;
            if(clickDuration < dashTreshhold) {
                // Dash
                DoDash();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetMouseButton(0)) {
            Vector3 dir = GetMouseInScene() - transform.position;
            body.AddForce(dir.normalized*swimForce);
        }
    }
}

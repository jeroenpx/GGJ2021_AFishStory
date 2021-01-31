using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMgr : MonoBehaviour
{
    private float nextBlinkTime;

    public float minInterval = 1;
    public float randInterval = 5;


    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        SetBlinkTime();
    }

    void SetBlinkTime() {
        nextBlinkTime = Time.time + minInterval + Random.value*randInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextBlinkTime) {
            animator.SetTrigger("Blink");
            SetBlinkTime();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRotation : MonoBehaviour
{
    public Rigidbody2D body;

    public Transform rotationTransform;

    private float normalCorrectionAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 GetMouseInScene() {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;
        return worldPosition;
    }

    public void StepCorrectRotation(Vector3 dir, float amount) {
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.forward) * Quaternion.Inverse(Quaternion.LookRotation(-Vector3.right, Vector3.forward)) * Quaternion.AngleAxis(body.rotation, -Vector3.forward);

        Quaternion sourceRotation = rotationTransform.localRotation;

        rotationTransform.localRotation = Quaternion.Lerp(sourceRotation, targetRotation, amount);
    }

    private Vector3 TargetPointDir () {
        return GetMouseInScene() - transform.position;
    }

    public void OnDash(Vector3 target) {
        Vector3 dir = target - transform.position;
        dir.z = 0;
        StepCorrectRotation(dir, .5f);
    }

    public void OnSwim(Vector3 target) {
        // To be called from Fixed Update
        Vector3 dir = target - transform.position;
        dir.z = 0;
        StepCorrectRotation(dir, .1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Auto correct to "flat orientation"
        Vector3 target = -rotationTransform.right;
        if(target.x>0) {
            target = Vector3.right;
        } else {
            target = Vector3.left;
        }
        StepCorrectRotation(target, .01f);

        // Nothing happening
        //StepCorrectRotation(.05f);

        // Simple: always show in the direction we are going?

        // Limit rotation speed
        // Dash => then, rotate instantly


        // Flip
        if(rotationTransform.right.x > 0) {
            rotationTransform.localScale = new Vector3(1, 1, 1);
        } else {
            rotationTransform.localScale = new Vector3(1, -1, 1);
        }
    }
}

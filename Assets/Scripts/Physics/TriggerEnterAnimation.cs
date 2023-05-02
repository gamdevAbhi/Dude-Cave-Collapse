using UnityEngine;

public class TriggerEnterAnimation : MonoBehaviour
{
    [Header("Reference")]
    public Animator animator;
    public string parameter;

    private void OnTriggerEnter(Collider other) {
        ColliderDetector detector = other.GetComponent<ColliderDetector>();

        if(detector == null) return;
        if(detector._isMoving == true) animator.SetBool(parameter, true);
    }

    private void OnTriggerExit(Collider other) {
        ColliderDetector detector = other.GetComponent<ColliderDetector>();

        if(detector == null) return;
        if(detector._isMoving == true) animator.SetBool(parameter, false);
    }
}

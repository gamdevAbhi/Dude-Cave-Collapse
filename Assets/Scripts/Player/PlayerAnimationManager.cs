using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject detectorPrefab;
    [SerializeField] private Animator playerAnimator;
    
    private ColliderDetector detector;

    void Start()
    {
        detector = GameObject.Instantiate(detectorPrefab, transform.position, Quaternion.identity)
        .GetComponent<ColliderDetector>();
        detector.SetTarget(gameObject, false);
    }

    void Update()
    {
        playerAnimator.SetBool("_IsMoving", detector._isMoving);
    }
}

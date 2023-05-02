using System.Collections.Generic;
using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    private GameObject target;
    private List<GameObject> currentDetection;
    private bool isMoving;
    private bool shouldStoreInformation;

    public GameObject[] _currentDetection {
        get {return currentDetection.ToArray();}
    }

    public bool _isMoving {
        get {return isMoving;}
    }

    private void Start() {
        if(shouldStoreInformation == true) currentDetection = new List<GameObject>();
    }

    public void SetTarget(GameObject target, bool shouldStoreInformation) {
        this.target = target;
        this.shouldStoreInformation = shouldStoreInformation;
    }

    private void LateUpdate() {
        if(target.transform.position == transform.position) isMoving = false;
        else isMoving = true;

        transform.position = target.transform.position;
    }

    private void OnTriggerEnter(Collider other) {
        if(currentDetection == null) return;
        currentDetection.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other) {
        if(currentDetection == null) return;
        if(currentDetection.Contains(other.gameObject)) currentDetection.Remove(other.gameObject);
    }
}

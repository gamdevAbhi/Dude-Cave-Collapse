using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {
    private Transform target;
    private float speed;
    private float delay;
    private float minDistance = 0.025f;
    [SerializeField] private float currentDelay;
    [SerializeField] private float currentDistance;

    public void SetTarget(Transform target, float speed, float delay) {
        this.target = target;
        this.speed = speed;
        this.delay = delay;
        transform.position = target.position;
    }

    private void Update() {
        if(target.position != transform.position) currentDelay += Time.deltaTime;
        else currentDelay = 0f;

        currentDistance = (target.position - transform.position).magnitude;

        if(currentDelay >= delay && minDistance >= currentDistance) {
            transform.position = target.position;
        }
        else if(currentDelay >= delay) {
            transform.position += (target.position - transform.position) * speed * Time.deltaTime;
        }
    }
}

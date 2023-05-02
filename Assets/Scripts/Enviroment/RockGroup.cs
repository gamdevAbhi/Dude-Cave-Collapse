using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGroup : MonoBehaviour, IHealth {
    [SerializeField] private List<Rock> rocks;

    public void TakeDamage(float hitPoint)
    {
        while(hitPoint > 0 && rocks.Count > 0) {
            hitPoint -= rocks[0]._health - hitPoint;
            rocks[0].TakeDamage(hitPoint);

            if(rocks[0]._health <= 0) {
                Destroy(rocks[0]._rockRef);
                rocks.Remove(rocks[0]);
            }
        }

        if(rocks.Count == 0) {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public class Rock : IHealth {
    [SerializeField] private GameObject rockRef;
    [SerializeField] private float health;
    
    public float _health {
        get {return health;}
    }

    public GameObject _rockRef {
        get {return rockRef;}
    }

    public void TakeDamage(float hitPoint) {
        health -= hitPoint;
    }
}

public interface IHealth {
    public void TakeDamage(float hitPoint);
}

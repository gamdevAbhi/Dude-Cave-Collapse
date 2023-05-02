using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveGate : MonoBehaviour
{
    private Room room;
    private CaveManager caveManager;

    public Room _room {
        get {return room;}
    }

    public void SetRoom(Room room) {
        this.room = room;
        caveManager = GameObject.FindGameObjectWithTag("Cave Manager").GetComponent<CaveManager>();
        caveManager._caveGates = this;
    }

    public void VisitDoor() {
        if(room == null) return;

        caveManager.SwitchRoom(room);
    }

    public void FixRotation(int x, int z) {
        if(z == 0) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 
            0f,
            transform.eulerAngles.z);
        }
        else if(x == 0) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 
            90f,
            transform.eulerAngles.z);
        }
        else if(z == caveManager._currentRoom.grid.GetLength(1) - 1) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 
            180f,
            transform.eulerAngles.z);
        }
        else {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 
            -90f,
            transform.eulerAngles.z);
        } 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveManager : MonoBehaviour {
    [SerializeField] private CaveCreator.Size maxRoomSize;
    [SerializeField] private CaveCreator.Size maxLakeSize;
    [SerializeField] private int maxDepth;
    [SerializeField] private Room currentRoom;
    [SerializeField] private int currentChildRoom;
    [SerializeField] private CaveBiom caveBiom;

    private int roomNo = 0;

    public Room _currentRoom {
        get { return currentRoom;}
    }

    private void Awake() {
        Test();
    }

    private void Test() {
        CaveCreator.Dungeon dungeon = new CaveCreator.Dungeon(maxRoomSize, maxLakeSize, maxDepth);

        currentRoom = CaveCreator.CreateDungeon(null, dungeon, 0);
        roomNo = dungeon.roomNo;
        CaveCreator.CreateRoom(currentRoom, caveBiom);
        currentRoom.Show(transform);
        currentRoom.MakeStatic();
        currentRoom.Save();
    }

    private void Switch(int child) {
        if(currentRoom.childRoom.Count > child && currentRoom.childRoom[child] != null) {
            currentRoom.Delete();
            currentRoom = currentRoom.childRoom[child];
            currentRoom.Show(transform);
            currentRoom.MakeStatic();
            currentRoom.Save();
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Return) && currentRoom.parentRoom != null) {
            currentRoom.Delete();
            currentRoom = currentRoom.parentRoom;
            currentRoom.Show(transform);
            currentRoom.MakeStatic();
            currentRoom.Save();
        }
        else if(Input.GetKeyDown(KeyCode.Keypad0)) Switch(0);
        else if(Input.GetKeyDown(KeyCode.Keypad1)) Switch(1);
        else if(Input.GetKeyDown(KeyCode.Keypad2)) Switch(2);
        else if(Input.GetKeyDown(KeyCode.Keypad3)) Switch(3);

        currentChildRoom = currentRoom.childRooms;
    }
}

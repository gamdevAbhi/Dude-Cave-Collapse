using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveManager : MonoBehaviour {
    [Header("Reference")]
    [SerializeField] private CaveBiom caveBiom;
    [SerializeField] private WaterManager waterManager;
    [Header("Cave Properties")]
    [SerializeField] [Range(0, 100)] private int objectTileProbablity;
    [SerializeField] [Range(0, 100)] private int objectWaterProbablity;
    [SerializeField] private CaveCreator.Size maxRoomSize;
    [SerializeField] private CaveCreator.Size maxLakeSize;
    [SerializeField] private int maxDepth;
    [Header("Room Properties")]
    [SerializeField] private Room currentRoom;
    [SerializeField] private int currentChildRoom;
    [SerializeField] private int roomNo = 0;

    public Room _currentRoom {
        get { return currentRoom;}
    }

    private void SwitchRoom(Room room) {
        if(currentRoom != null) currentRoom.Delete();

        currentRoom = room;
        currentRoom.Show(transform);
        currentRoom.MakeStatic();
        currentRoom.Save();
        waterManager.ScaleWater(currentRoom.grid.GetLength(0) - 2, currentRoom.grid.GetLength(1) - 2);
    }

    private void Awake() {
        CaveCreator.Dungeon dungeon = new CaveCreator.Dungeon(maxRoomSize, maxLakeSize, maxDepth,
        objectTileProbablity, objectWaterProbablity);
        Room parent = CaveCreator.CreateDungeon(null, dungeon, 0);
        CaveCreator.CreateRoom(parent, caveBiom, true);
        SwitchRoom(parent);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Return) && currentRoom.parentRoom != null) {
            SwitchRoom(currentRoom.parentRoom);
        }
        else if(Input.GetKeyDown(KeyCode.Keypad0) && currentChildRoom > 0) SwitchRoom(currentRoom.childRoom[0]);
        else if(Input.GetKeyDown(KeyCode.Keypad1)) SwitchRoom(currentRoom.childRoom[0]);
        else if(Input.GetKeyDown(KeyCode.Keypad2)) SwitchRoom(currentRoom.childRoom[0]);
        else if(Input.GetKeyDown(KeyCode.Keypad3)) SwitchRoom(currentRoom.childRoom[0]);
        else if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        currentChildRoom = currentRoom.childRooms;
        roomNo = currentRoom.roomNo;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveManager : MonoBehaviour {
    [Header("Reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private CaveBiom caveBiom;
    [SerializeField] private WaterManager waterManager;
    [SerializeField] private SettingManager settingManager;
    [Header("Cave Properties")]
    [SerializeField] [Range(0, 100)] private int groundObjectProbablity;
    [SerializeField] [Range(0, 100)] private int waterObjectProbablity;
    [SerializeField] private CaveCreator.Size maxRoomSize;
    [SerializeField] private CaveCreator.Size maxLakeSize;
    [SerializeField] private int maxDepth;
    
    private Room currentRoom;
    private List<CaveGate> caveGates;
    
    public Room _currentRoom {
        get {return currentRoom;}
    }

    public CaveGate _caveGates {
        set {
            if(caveGates == null) caveGates = new List<CaveGate>();
            caveGates.Add(value);}
    }

    private CaveGate GetParentGate(Room room) {
        foreach(CaveGate gate in caveGates) if(gate._room == room) return gate;
        return null;
    }

    public void SwitchRoom(Room room) {
        caveGates = new List<CaveGate>();
        
        if(currentRoom != null) {
            currentRoom.Delete();
        }
        Room previousRoom = currentRoom;
        currentRoom = room;
        currentRoom.Show(transform, caveBiom);
        CaveGate nextGate = GetParentGate(previousRoom);

        player.transform.position = new Vector3(nextGate.transform.position.x, 
        player.transform.position.y, nextGate.transform.position.z);
        currentRoom.MakeStatic();
        waterManager.ScaleWater(currentRoom.grid.GetLength(0) - 2, currentRoom.grid.GetLength(1) - 2);
        settingManager.SetSetting();
    }

    private void Awake() {
        CaveCreator.Dungeon dungeon = new CaveCreator.Dungeon(maxRoomSize, maxLakeSize, maxDepth,
        groundObjectProbablity, waterObjectProbablity);
        Room parent = CaveCreator.CreateDungeon(null, dungeon, 0);
        CaveCreator.CreateRoom(parent, caveBiom, true);
        SwitchRoom(parent);
    }

    public Tile GetTile(float x, float z) {
        if(x < 0 || x >= currentRoom.grid.GetLength(0)) return null;
        if(z < 0 || z >= currentRoom.grid.GetLength(1)) return null;
        
        return currentRoom.grid[(int)x,(int)z];
    }
}

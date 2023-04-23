using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveCreator : MonoBehaviour {
    public enum Size {Small, Normal, Large};
    public Dictionary<string, Object> hexItem;

    [SerializeField] private Size maxRoomSize;
    [SerializeField] private Size maxLakeSize;
    [SerializeField] private int maxDepth;
    [SerializeField] private Room currentRoom;
    [SerializeField] private CaveProb caveProb;

    private int roomNo = 0;

    private int[] GetRoomSize() {
        Size roomSize = (Size)UnityEngine.Random.Range(0, (int)maxRoomSize + 1);

        if(roomSize == Size.Small) return new int[] {
            UnityEngine.Random.Range(6,10), UnityEngine.Random.Range(6,10)
            };
        else if(roomSize == Size.Normal) return new int[] {
            UnityEngine.Random.Range(11,20), UnityEngine.Random.Range(11,20)
            };
        else return new int[] {
            UnityEngine.Random.Range(21,30), UnityEngine.Random.Range(21,30)
        };
    }

    public Wave LowestWave(Wave[,] waves) {
        Wave lowest = null;

        for(int x = 0; x < waves.GetLength(0); x++) {
            for(int z = 0; z < waves.GetLength(1); z++) {
                if(waves[x,z].hexes.Count > 1 && lowest == null) lowest = waves[x,z];
                else if(waves[x,z].hexes.Count > 1 && waves[x,z].hexes.Count < lowest.hexes.Count) lowest = waves[x,z];
            }
        }

        return lowest;
    }

    public void Adjust(Wave wave, Room room) {
            if(wave.hexes.Count == 1) return;

            for(int i = 0; i < wave.hexes.Count; i++) {
                if(wave.hexes[i] == hexItem["Water"]
                && room.maxLakeCount <= room.currentLakeCount) {
                    wave.hexes.Remove(wave.hexes[i]);
                    continue;
                }

                wave.probablity[wave.hexes[i]] = caveProb.GetProb(wave.hexes[i].name);

                foreach(Wave neighbour in wave.neighbours) {
                    if(neighbour.hexes.Count == 1 && neighbour.hexes[0] == wave.hexes[i]) 
                        wave.probablity[wave.hexes[i]] += wave.hexes[i].matchChange;
                }

                if(wave.probablity[wave.hexes[i]] <= 0) wave.hexes.Remove(wave.hexes[i]);
                else if(wave.probablity[wave.hexes[i]] >= 100) wave.hexes = new List<Object> {wave.hexes[i]};
            }

            wave.hexes = wave.hexes.OrderBy(x => wave.probablity[x]).ToList();
            wave.hexes.Reverse();

            while(wave.hexes.Count > 1) {
                if(UnityEngine.Random.Range(0, 100) <= wave.probablity[wave.hexes[0]]) {
                    if(wave.hexes[0] == hexItem["Water"]) room.currentLakeCount++;
                    wave.hexes = new List<Object> {wave.hexes[0]};
                }
                else wave.hexes.Remove(wave.hexes[0]);
            }
    }

    public void MakeRoom(Room room) {
        if(room.isExist == true) {
            for(int x = 0; x < room.grid.GetLength(0); x++) {
                for(int z = 0; z < room.grid.GetLength(1); z++) {
                    room.grid[x,z] = GameObject.Instantiate(room.gridSave[x,z].objectRef, 
                    new Vector3(x,0,z), Quaternion.identity).GetComponent<Entity>();
                    room.grid[x,z].transform.parent = this.transform;
                    room.grid[x,z].hex = room.gridSave[x,z];
                }
            }
        }
        else {
            Wave[,] waves = new Wave[room.grid.GetLength(0), room.grid.GetLength(1)];

            for(int x = 0; x < room.grid.GetLength(0); x++) {
                for(int z = 0; z < room.grid.GetLength(1); z++) {
                    waves[x,z] = new Wave();

                    foreach(Object value in hexItem.Values) {
                        if(hexItem["Water"] != value) waves[x,z].AddHexes(value);
                        else if(x > 0 && x < room.grid.GetLength(0) - 1 
                        && z > 0 && z < room.grid.GetLength(1) - 1) waves[x,z].AddHexes(hexItem["Water"]);
                    }
                }
            }

            for(int x = 0; x < waves.GetLength(0); x++) {
                for(int z = 0; z < waves.GetLength(1); z++) {
                    if(x > 0) waves[x,z].AddNeighbour(waves[x-1,z]);
                    if(z > 0) waves[x,z].AddNeighbour(waves[x,z-1]);
                    if(x < room.grid.GetLength(0) - 1) waves[x,z].AddNeighbour(waves[x+1,z]);
                    if(z < room.grid.GetLength(1) - 1) waves[x,z].AddNeighbour(waves[x,z+1]);
                }
            }

            Wave lowest = LowestWave(waves);

            while(lowest != null) {
                Adjust(lowest, room);
                lowest = LowestWave(waves);
            }

            for(int x = 0; x < room.grid.GetLength(0); x++) {
                for(int z = 0; z < room.grid.GetLength(1); z++) {
                    room.grid[x,z] = GameObject.Instantiate(waves[x,z].hexes[0].objectRef, 
                    new Vector3(x,0,z), Quaternion.identity).GetComponent<Entity>();
                    room.grid[x,z].transform.parent = this.transform;
                    room.grid[x,z].hex = waves[x,z].hexes[0];
                }
            }
        }
    }

    private Room CreateDungeon(Room parentRoom, int depth) {
        if(depth >= maxDepth) return null;
        Room current = new Room(GetRoomSize(), parentRoom, ref roomNo, 
        (maxLakeSize == Size.Small)? 0.2f : (maxLakeSize == Size.Normal)? 0.4f : 0.7f);
        int childs = 0;

        if(depth < maxDepth - 1 && depth > maxDepth / 2) childs = UnityEngine.Random.Range(0, 4);
        else if(depth < maxDepth - 1 && depth < maxDepth / 2) childs = UnityEngine.Random.Range(1, 4);

        for(int i = 0; i < childs; i++) current.childRoom.Add(CreateDungeon(current, depth + 1));

        return current;
    }

    private void Awake() {
        hexItem = new Dictionary<string, Object>();
        Object[] tiles = Resources.LoadAll<Object>("Tile");

        foreach(Object tile in tiles) {
            hexItem.Add(tile.name, tile);
        }
    }

    private void Start() {
        currentRoom = CreateDungeon(null, 0);
        MakeRoom(currentRoom);
        currentRoom.MakeStatic();
        currentRoom.Save();
    }

    private void Switch(int child) {
        if(currentRoom.childRoom.Count > child && currentRoom.childRoom[child] != null) {
            currentRoom.Delete();
            currentRoom = currentRoom.childRoom[child];
            MakeRoom(currentRoom);
            currentRoom.MakeStatic();
            currentRoom.Save();
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Return) && currentRoom.parentRoom != null) {
            currentRoom.Delete();
            currentRoom = currentRoom.parentRoom;
            MakeRoom(currentRoom);
            currentRoom.MakeStatic();
        }
        else if(Input.GetKeyDown(KeyCode.Keypad0)) Switch(0);
        else if(Input.GetKeyDown(KeyCode.Keypad1)) Switch(1);
        else if(Input.GetKeyDown(KeyCode.Keypad2)) Switch(2);
        else if(Input.GetKeyDown(KeyCode.Keypad3)) Switch(3);
    }
}

// WFC ALGO
public class Wave {
    public List<Object> hexes;
    public Dictionary<Object, int> probablity;
    public List<Wave>  neighbours;

    public Wave() {
        this.hexes = new List<Object>();
        this.probablity = new Dictionary<Object, int>();
        this.neighbours = new List<Wave>();
    }

    public void AddNeighbour(Wave neighbour) {
        if(this.neighbours.Contains(neighbour) == false) this.neighbours.Add(neighbour);
    }

    public void AddHexes(Object hex) {
        if(hexes.Contains(hex) == false) {
            this.hexes.Add(hex);
            probablity.Add(hex, 0);
        }
    }
}

[Serializable]
public class Room {
    public int roomNo;
    [HideInInspector] public bool isExist;
    public Entity[,] grid;
    public Object[,] gridSave;
    public Room parentRoom;
    public int maxLakeCount;
    [HideInInspector] public int currentLakeCount; 
    [HideInInspector] public List<Room> childRoom;
    public int childRooms;

    public Room(int[] roomSize, Room parent, ref int roomNo, float lakeSize) {
        this.grid = new Entity[roomSize[0], roomSize[1]];
        this.gridSave = new Object[roomSize[0], roomSize[1]];
        this.parentRoom = parent;
        this.currentLakeCount = 0;
        this.maxLakeCount = (int)((this.grid.GetLength(0) - 2) * (this.grid.GetLength(1) - 2) * lakeSize);
        this.childRoom = new List<Room>();
        this.roomNo = roomNo++;
        this.isExist = false;
    }

    public void MakeStatic() {
        for(int x = 0; x < grid.GetLength(0); x++) {
            for(int z = 0; z <grid.GetLength(1); z++) {
                if(grid[x,z]) grid[x,z].gameObject.isStatic = true;
            }
        }

        this.childRooms = childRoom.Count;
    }

    public void Save() {
        for(int x = 0; x < grid.GetLength(0); x++) {
            for(int z = 0; z <grid.GetLength(1); z++) {
                gridSave[x,z] = grid[x,z].hex;
            }
        }

        this.isExist = true;
    }

    public void Delete() {
        for(int x = 0; x < grid.GetLength(0); x++) {
            for(int z = 0; z <grid.GetLength(1); z++) {
                if(grid[x,z]) GameObject.Destroy(grid[x,z].gameObject);
                grid[x,z] = null;
            }
        }
    }
}

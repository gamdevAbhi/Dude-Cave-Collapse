using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CaveCreator {
    public enum Size {Small, Normal, Large};

    public struct Dungeon {
        public int objectTileProbablity;
        public int objectWaterProbablity;
        public Size maxRoomSize;
        public Size maxLakeSize;
        public int maxDepth;
        public int roomNo;

        public Dungeon(Size maxRoomSize, Size maxLakeSize, int maxDepth, int objectTileProbablity,
        int objectWaterProbablity) {
            this.maxDepth = maxDepth;
            this.objectTileProbablity = objectTileProbablity;
            this.objectWaterProbablity = objectWaterProbablity;
            this.maxLakeSize = maxLakeSize;
            this.maxRoomSize = maxRoomSize;
            this.roomNo = 0;
        }
    }

    private static int[] GetRandomRoomSize(Size maxRoomSize) {
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

    public static Wave LowestWave(Wave[,] waves) {
        Wave lowest = null;

        for(int x = 0; x < waves.GetLength(0); x++) {
            for(int z = 0; z < waves.GetLength(1); z++) {
                if(waves[x,z].hexes.Count > 1 && lowest == null) lowest = waves[x,z];
                else if(waves[x,z].hexes.Count > 1 && waves[x,z].hexes.Count < lowest.hexes.Count) lowest = waves[x,z];
            }
        }

        return lowest;
    }

    public static int GetProbablity(Wave wave, Eigenstate state) {
        int probablity = state.probablity;

        foreach(Wave neighbour in wave.neighbours) {
            if(neighbour.hexes.Count == 1 && neighbour.hexes[0] == state) 
                probablity += (int)(probablity * state.refer.increaseChance);
        }

        return probablity;
    }

    public static void SetObject(Wave wave) {
        wave.hexes = wave.hexes.OrderBy(x => wave.probablity[x]).ToList();
        wave.hexes.Reverse();

        int total = 0, current = 0;
        foreach(int prob in wave.probablity.Values) total += prob;
        int rand = UnityEngine.Random.Range(0, total + 1);

        for(int i = 0; i < wave.hexes.Count; i++) {
            current += wave.probablity[wave.hexes[i]];

            if(rand <= current) {
                wave.hexes = new List<Eigenstate> {wave.hexes[i]};
                break;
            }
        }
    }

    public static void Adjust(Wave wave, Room room) {
        if(wave.hexes.Count == 1) return;

        for(int i = 0; i < wave.hexes.Count; i++) {
            if(wave.hexes[i].refer.env == Object.Enviroment.Water
            && room.maxLakeCount <= room.currentLakeCount) {
                wave.hexes.Remove(wave.hexes[i]);
                continue;
            }

            wave.probablity[wave.hexes[i]] = GetProbablity(wave, wave.hexes[i]);

            if(wave.probablity[wave.hexes[i]] <= 0) wave.hexes.Remove(wave.hexes[i]);
        }

        SetObject(wave);

        if(wave.hexes[0].refer.env == Object.Enviroment.Water) room.currentLakeCount++;
    }

    public static void CreateRoom(Room room, CaveBiom caveBiom, bool withChild) {
        Wave[,] waves = new Wave[room.grid.GetLength(0), room.grid.GetLength(1)];

        for(int x = 0; x < room.grid.GetLength(0); x++) {
            for(int z = 0; z < room.grid.GetLength(1); z++) {
                waves[x,z] = new Wave();

                foreach(Eigenstate value in caveBiom.tiles) {
                    if(value.refer.env != Object.Enviroment.Water) waves[x,z].AddHexes(value);
                    else if(x > 0 && x < room.grid.GetLength(0) - 1 
                    && z > 0 && z < room.grid.GetLength(1) - 1) waves[x,z].AddHexes(value);
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
                room.gridSave[x,z] = new Tile();
                room.gridSave[x,z].tile = waves[x,z].hexes[0].refer;
            }
        }

        if(withChild) foreach(Room child in room.childRoom) CreateRoom(child, caveBiom, true);
        
        foreach(Tile tile in room.gridSave) {
            int probablity = (tile.tile.env == Object.Enviroment.Tile)? room.objectTileProbablity : 
            room.objectWaterProbablity;

            if(UnityEngine.Random.Range(0, 100) > probablity) continue;

            Wave wave = new Wave();

            foreach(Eigenstate obj in caveBiom.objects) {
                if(obj.refer.env == tile.tile.env) wave.AddHexes(obj);
            }

            if(wave.hexes.Count == 0) {
                tile._object = null;
                continue;
            }

            for(int i = 0; i < wave.hexes.Count; i++) {
                wave.probablity[wave.hexes[i]] = GetProbablity(wave, wave.hexes[i]);

                if(wave.probablity[wave.hexes[i]] <= 0) wave.hexes.Remove(wave.hexes[i]);
            }

            SetObject(wave);

            tile._object = wave.hexes[0].refer;
        }
    }

    public static Room CreateDungeon(Room parentRoom, Dungeon dungeon, int depth) {
        if(depth >= dungeon.maxDepth) return null;
        Room current = new Room(GetRandomRoomSize(dungeon.maxRoomSize), parentRoom, ref dungeon.roomNo, 
        (dungeon.maxLakeSize == Size.Small)? 0.2f : (dungeon.maxLakeSize == Size.Normal)? 0.4f : 0.7f,
        dungeon.objectTileProbablity, dungeon.objectWaterProbablity);
        int childs = 0;

        if(depth < dungeon.maxDepth - 1 && depth > dungeon.maxDepth / 2) childs = UnityEngine.Random.Range(0, 4);
        else if(depth < dungeon.maxDepth - 1 && depth < dungeon.maxDepth / 2) childs = UnityEngine.Random.Range(1, 4);

        for(int i = 0; i < childs; i++) current.childRoom.Add(CreateDungeon(current, dungeon, depth + 1));

        return current;
    }
}

public class Wave {
    public List<Eigenstate> hexes;
    public Dictionary<Eigenstate, int> probablity;
    public List<Wave>  neighbours;

    public Wave() {
        this.hexes = new List<Eigenstate>();
        this.probablity = new Dictionary<Eigenstate, int>();
        this.neighbours = new List<Wave>();
    }

    public void AddNeighbour(Wave neighbour) {
        if(this.neighbours.Contains(neighbour) == false) this.neighbours.Add(neighbour);
    }

    public void AddHexes(Eigenstate hex) {
        if(hexes.Contains(hex) == false) {
            this.hexes.Add(hex);
            probablity.Add(hex, 0);
        }
    }
}

public class Room {
    public int roomNo;
    public Entity[,] grid;
    public Tile[,] gridSave;
    public Room parentRoom;
    public int maxLakeCount;
    public int objectTileProbablity;
    public int objectWaterProbablity;
    public int currentLakeCount;
    public List<Room> childRoom;
    public int childRooms;

    public Room(int[] roomSize, Room parent, ref int roomNo, float lakeSize, 
    int objectTileProbablity, int objectWaterProbablity) {
        this.grid = new Entity[roomSize[0], roomSize[1]];
        this.gridSave = new Tile[roomSize[0], roomSize[1]];
        this.parentRoom = parent;
        this.currentLakeCount = 0;
        this.objectTileProbablity = objectTileProbablity;
        this.objectWaterProbablity = objectWaterProbablity;
        this.maxLakeCount = (int)((this.grid.GetLength(0) - 2) * (this.grid.GetLength(1) - 2) * lakeSize);
        this.childRoom = new List<Room>();
        this.roomNo = roomNo++;
    }

    public void MakeStatic() {
        for(int x = 0; x < grid.GetLength(0); x++) {
            for(int z = 0; z <grid.GetLength(1); z++) {
                if(grid[x,z]) grid[x,z].gameObject.isStatic = true;
            }
        }

        this.childRooms = childRoom.Count;
    }

    public void Show(Transform parent) {
        for(int x = 0; x < grid.GetLength(0); x++) {
            for(int z = 0; z < grid.GetLength(1); z++) {
                grid[x,z] = GameObject.Instantiate(gridSave[x,z].tile.objectRef, 
                new Vector3(x,0,z), Quaternion.identity).GetComponent<Entity>();
                grid[x,z].transform.parent = parent;
                grid[x,z].tile = gridSave[x,z].tile;

                if(gridSave[x,z]._object != null) {
                    GameObject obj = GameObject.Instantiate(gridSave[x,z]._object.objectRef,
                    new Vector3(x,gridSave[x,z]._object.objectRef.transform.position.y,z), Quaternion.identity);
                    obj.transform.parent = grid[x,z].transform;
                    grid[x,z]._object = gridSave[x,z]._object;
                }
            }
        }
    }

    public void Save() {
        for(int x = 0; x < grid.GetLength(0); x++) {
            for(int z = 0; z <grid.GetLength(1); z++) {
                gridSave[x,z] = new Tile();
                gridSave[x,z].tile = grid[x,z].tile;
                gridSave[x,z]._object = grid[x,z]._object;
            }
        }
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

public class Tile {
    public Object tile;
    public Object _object;
}

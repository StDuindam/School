using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungeonGenerator : MonoBehaviour {
    //game manager
    public GameManager manager;

    public GameObject wallPrefab;
    public GameObject[] tilePrefab;

    public int boardSizeY;
    public int boardSizeX;
    [SerializeField]
    public Tile[,] tiles;
    public Transform boardHolder;
    public List<GameObject> instantiatedTiles;
    public List<Room> rooms = new List<Room>();
    //public Navmes
    public GameObject boardMap;
    //list walls
    //list rooms
    public int amountOfRooms;
    public int roomSizeXMin;
    public int roomSizeYMin;
    public int roomSizeXMax;
    public int roomSizeYMax;
    public List<Transform> roomCenters;
    //list AI's
    public List<GameObject> AI;
    public GameObject HideBot;
    public GameObject ShootBot;

    void Start() {
        manager = this.GetComponent<GameManager>();
        rooms = new List<Room>();
        tiles = new Tile[boardSizeX, boardSizeY];
        CreateBoard(boardSizeX, boardSizeY);
        manager.Init();
        }

    public Vector3 GetStartPos() {
        return new Vector3(rooms[0].roomCenterX,1, rooms[0].roomCenterY);
    }

    public void Update() {
       // if (Input.GetKeyDown(KeyCode.Space))
         //   CleanBoard();
       // manager.Init();
    }

    //To make a new board we clean the last one so nothing overlays or gets left behind
    public void CleanBoard() {
        rooms = new List<Room>();
        foreach(Tile t in tiles) {
            t.SetState(0);
        }
        foreach(GameObject go in instantiatedTiles) {
            Destroy(go);
        }

        foreach(GameObject bot in AI) {
            Destroy(bot);
            }

        tiles = new Tile[boardSizeX, boardSizeY];
        CreateBoard(boardSizeX, boardSizeY);
        }

    public void CreateBoard(int xSize, int ySize) {

        //A playmat for the navmesh
        if (boardMap == null) {
            boardMap = Instantiate(tilePrefab[0], new Vector3 (0,0.48f,0), Quaternion.identity);
            boardMap.transform.localScale = new Vector3(boardSizeX, 0, boardSizeY);
            boardMap.transform.position = new Vector3(Mathf.Round(boardSizeX / 2), 0, Mathf.Round(boardSizeY / 2));
            }

        //Create all tiles to be used by our board
        for (int i = 0; i < xSize; i++) {
            for (int j = 0; j < ySize; j++) {
                tiles[i, j] = new Tile();
                tiles[i, j].SetState(Tile.tileState.death);
                tiles[i, j].SetPos(i, j);
                }
            }

        //Create rooms, safety net so the while loop does not break our application
        int safetyNet = 250;
        int currentIteration = 0;
        while (rooms.Count < amountOfRooms) {
            CreateRoom(Random.Range(0, boardSizeX), Random.Range(0, boardSizeY));
            currentIteration += 1;
            if (currentIteration == safetyNet)
                break;
            }
        //Instantiate the board
        for(int i =0; i < amountOfRooms - 2;i++) {
            CreatePaths(rooms[Random.Range(0, rooms.Count)]);
        }
        
        InstantiateBoard();

        //Create navmesh
        if (boardMap.GetComponent<NavMeshSurface>() == null) {
            boardMap.AddComponent<NavMeshSurface>();
            boardMap.GetComponent<NavMeshSurface>().BuildNavMesh();
            }
        else {
            boardMap.GetComponent<NavMeshSurface>().UpdateNavMesh(boardMap.GetComponent<NavMeshData>());
            }

        foreach (Room r in rooms) {
            if (r.isTreasureRoom) {
                foreach (Tile t in r.roomTiles) {
                    t.actualTile.GetComponent<Renderer>().material.color = Color.yellow;
                    }
                }

            //Instantiate AI
            int botOrNot = Random.Range(0, 3);
            Debug.Log(botOrNot);
            switch (botOrNot) {
                case 1:
                    GameObject hBot = Instantiate(HideBot, new Vector3(r.roomCenterX, 1, r.roomCenterY), Quaternion.identity);
                    AI.Add(hBot);
                    break;
                case 2:
                    GameObject sBot = Instantiate(ShootBot, new Vector3(r.roomCenterX, 1, r.roomCenterY), Quaternion.identity);
                    sBot.GetComponent<AI2>().SetWP(roomCenters);
                    sBot.GetComponent<AI2>().Init();
                    AI.Add(sBot);
                    break;
                }
            }




        }

    public void CreateRoom(int _startPosX, int _startPosY) {
        int roomSizeX = Random.Range(roomSizeXMin, roomSizeXMax);
        int roomSizeY = Random.Range(roomSizeYMin, roomSizeXMax);

        //Check if the asked room can be placed in our board
        if (!CheckIfRoomFits(_startPosX, _startPosY, roomSizeX, roomSizeY)) {
            return;
            }
        //If above returns true we can create our room
        Room room = new Room();
        //room.roomTiles = new List<Tile>();
        //Variables of the room so we dont need to calculate every loop in the for loop
        int xMax = roomSizeX + _startPosX;
        int yMax = _startPosY + roomSizeY;
        if (yMax > boardSizeY)
            yMax = boardSizeY - 1;
        if (xMax > boardSizeX)
            xMax = boardSizeX - 1;

        //Set states on tiles that are now part of a room
        for (int i = _startPosX; i < xMax; i++) {
            for (int j = _startPosY; j < yMax; j++) {
                if (tiles[i, j].x < boardSizeX - 1 || tiles[i, j].y < boardSizeY - 1) {
                    tiles[i, j].SetState(Tile.tileState.alive);
                    room.roomTiles.Add(tiles[i, j]);
                    }
                }
            }

        //Update room instance variables
        room.xSize = roomSizeX;
        room.ySize = roomSizeY;
        room.roomCenterX = (room.roomTiles[0].x + room.roomTiles[room.roomTiles.Count - 1].x) / 2;
        room.roomCenterY = (room.roomTiles[0].y + room.roomTiles[room.roomTiles.Count - 1].y) / 2;

        //Make the room part of our rooms list
        rooms.Add(room);
        }

    public void CreatePaths(Room _currentRoom) {
        Room targetRoom = rooms[Random.Range(0, rooms.Count)];
        if (targetRoom == _currentRoom) { CreatePaths(_currentRoom); return; }
        int targetXPos;
        int targetYPos;
        _currentRoom.isTreasureRoom = false;
        targetRoom.isTreasureRoom = false;
        targetXPos = (int)Mathf.Abs(_currentRoom.roomCenterX - targetRoom.roomCenterX);
        targetYPos = _currentRoom.roomCenterY - targetRoom.roomCenterY;
        //Debug.Log("from: " + _currentRoom.roomCenterX + " - " + targetRoom.roomCenterX + " = " + targetXPos);

        if (_currentRoom.roomCenterX > targetRoom.roomCenterX) {
            for (int i = _currentRoom.roomCenterX; i > _currentRoom.roomCenterX - targetXPos; i--) {
                if (tiles[i, _currentRoom.roomCenterY].state != (int)Tile.tileState.alive) {
                    tiles[i, _currentRoom.roomCenterY].SetState(Tile.tileState.path);
                    }
                }
            
            }
        else {
            for (int i = _currentRoom.roomCenterX; i < targetRoom.roomCenterX; i++) {
                if (tiles[i, _currentRoom.roomCenterY].state != (int)Tile.tileState.alive) {
                    tiles[i, _currentRoom.roomCenterY].SetState(Tile.tileState.path);

                    }
                }
            }

        if (_currentRoom.roomCenterY > targetRoom.roomCenterY) {
            for (int i = _currentRoom.roomCenterY; i > _currentRoom.roomCenterY - targetYPos; i--) {
                if (tiles[targetRoom.roomCenterX,i].state != (int)Tile.tileState.alive) {
                    tiles[targetRoom.roomCenterX,i].SetState(Tile.tileState.path);
                    }
                }
            }
        else {
            for (int i = _currentRoom.roomCenterY; i < targetRoom.roomCenterY ; i++) {
                if (tiles[targetRoom.roomCenterX, i].state != (int)Tile.tileState.alive) {
                    tiles[targetRoom.roomCenterX, i].SetState(Tile.tileState.path);
                }
            }
            }
        
        }

    //A function that checks in a one wider square than the actual to be placed room so there are no openings in walls or overlays in rooms.
    public bool CheckIfRoomFits(int _startPosX, int _startPosY, int _roomSizeX, int _roomSizeY) {
        int xMax = _roomSizeX + _startPosX;
        int yMax = _roomSizeY + _startPosY;

        //Check Y coördinates
        if (_startPosY <= 0) {
            return false;
            }
        if (yMax >= boardSizeY || (_startPosY <= 0)) {
            return false;
            }
        else {
            if (yMax + 1 <= boardSizeY)
                yMax += 1;
            }

        //Check X coördinates
        if (_startPosX <= 0) {
            return false;
            }
        if (xMax >= boardSizeX) {
            return false;
            }
        else {
            if (xMax + 1 <= boardSizeX || _startPosX <= 0)
                xMax += 1;
            }

        //Scan one square larger than the actual room so nothing overlays
        if (_startPosX != 0) {
            _startPosX -= 1;
            }
        if (_startPosY != 0) {
            _startPosY -= 1;
            }

        for (int i = _startPosX; i < xMax; i++) {
            for (int j = _startPosY; j < yMax; j++) {
                if (tiles[i, j].state != 0)
                    return false;

                }


            }
        return true;
        }

    //Create tiles and objects based on the settings
    public void InstantiateBoard() {
        foreach(Tile t in tiles) {
            GameObject go = Instantiate(tilePrefab[0], new Vector3(t.x, 0, t.y), transform.rotation);
            if (t.state == 0) {
                go.transform.localScale = new Vector3(1, Random.Range(5,8),1);
                go.GetComponent<Renderer>().material.color = Color.black;
                go.AddComponent<NavMeshObstacle>();
                go.layer = 8;
                }
            if (t.state == 1) {
                
                go.GetComponent<Renderer>().material.color = Color.red;

                }
            if (t.state == 2) {
                go.GetComponent<Renderer>().material.color = Color.green;
                }
            if (t.state == 3) {
                go.GetComponent<Renderer>().material.color = Color.blue;

                }
            go.transform.parent = boardHolder;
            t.actualTile = go;
            instantiatedTiles.Add(go);
            }

        foreach(Room r in rooms) {
            roomCenters.Add(r.roomTiles[rooms.Count / 2].actualTile.transform);
            }
        }
}

[System.Serializable]
public class Tile{
    [SerializeField]
    public int state;
    public int x;
    public int y;
    public enum tileState {death,alive,wall,path};
    public GameObject actualTile;

    //Functions to change variables in the class
    public void SetState(tileState _state) {
        state = (int)_state;
        }

    //Set positions
    public void SetPos(int _x, int _y) {
        x = _x;
        y = _y;
        }
}

[System.Serializable]
public class Room {
    [SerializeField]
    public int xSize;
    public int ySize;
    public List<Tile> roomTiles = new List<Tile>();
    public int roomCenterX;
    public int roomCenterY;
    public bool isTreasureRoom = true;
    //theme
    }

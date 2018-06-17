using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class Manager : NetworkBehaviour {

    List<NetworkConnection> players = new List<NetworkConnection>();
    List<GameObject> playerObjects = new List<GameObject>();
    //Playerstats


    public static Manager instance;

    public GameObject GameHolder;

    [Header("UI")]
    public Text whosTurn;
    public Text scoreP1;
    public Text scoreP2;
    public Text requestedKey;
    public Text state;
    public Text result;
    public Text namePlayer;

    //store this
    //[SyncVar]
    public int currentTurn = 1;
    [SyncVar]
    public int currentScoreP1 = 0;
    [SyncVar]
    public int currentScoreP2 = 0;
    [SyncVar]
    public string currentKey = "Make the first move!";
    [SyncVar]
    public string currentResult = " ";

    void Awake() {
        if (instance == null) {
            instance = this;
            }
        else {
            Destroy(this);
            }
        }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(playerObjects.Count);
	}

    public override void PreStartClient() {
        base.PreStartClient();
    }

    [Command]
    public void CmdPlayerJoined(GameObject obj) {

        NetworkConnection networkID = obj.GetComponent<NetworkIdentity>().connectionToClient;
        if(!players.Contains(obj.GetComponent<NetworkIdentity>().connectionToClient)) {
            //add to list else return;
            players.Add(obj.GetComponent<NetworkIdentity>().connectionToClient);
            playerObjects.Add(obj);
            if(playerObjects.Count == 1) {
                playerObjects[0].GetComponent<PlayerTry>().myTurn = true;
                namePlayer.text = "You are player one";
                playerObjects[0].name = "last";
                Debug.Log(playerObjects[0].GetComponent<PlayerTry>().myTurn);          
                }

            if(playerObjects.Count == 2) {
                foreach (GameObject go in playerObjects) {
                    go.GetComponent<PlayerTry>().GameStarted = true;

                    }
                GameHolder.SetActive(true);
                RpcPushUI();
                StartCoroutine(GameLoop());
                }
            }
        else {
            Debug.Log("Same ID");
            return;
            }
    }

    public IEnumerator GameLoop() {
        yield return new WaitForSeconds(30);

        foreach (GameObject go in playerObjects) {
            go.GetComponent<PlayerTry>().GameStarted = false;
        }
        /*
        playerObjects[0].GetComponent<PlayerTry>().myScore = currentScoreP1;
        playerObjects[0].GetComponent<PlayerTry>().myScore = currentScoreP2;
        RpcScoreUpdate();

        GameHolder.SetActive(false);
        RpcPushUI();(*/
        CmdUpdateState("Done!");
    }

    [ClientRpc]
    public void RpcScoreUpdate() {

        playerObjects[0].GetComponent<PlayerTry>().myScore = currentScoreP1;
        playerObjects[1].GetComponent<PlayerTry>().myScore = currentScoreP2;
        playerObjects[0].GetComponent<PlayerTry>().CmdSendScore();
        playerObjects[1].GetComponent<PlayerTry>().CmdSendScore();

    }

    public void CmdPlayerLeft(GameObject obj) {
        //Pause game for x duration
        //Else kick session

        }

    [Command]
    public void CmdChangeTurns() {
        Debug.Log("Changing turn");
        if (playerObjects[0].GetComponent<PlayerTry>().myTurn) {
            playerObjects[1].GetComponent<PlayerTry>().myTurn = true;
            playerObjects[0].GetComponent<PlayerTry>().myTurn = false;
            }
        else {
            playerObjects[0].GetComponent<PlayerTry>().myTurn = true;
            playerObjects[1].GetComponent<PlayerTry>().myTurn = false;
            }
        }

    [Command]
    public void CmdUpdatePlayerScoreUI(GameObject player) {
        if (!player.GetComponent<PlayerTry>())
            return;

        if (playerObjects[0] == player) {
            scoreP1.text = "Player1 score: " + (currentScoreP1.ToString());
            currentScoreP1 += 1;
            RpcUpdateScore1();
            }
        else {
            currentScoreP2 += 1;
            scoreP2.text = "Player2 score: " + (currentScoreP2.ToString());
            RpcUpdateScore2();
            }
        RpcPushUI();
        }

    [Command]
    public void CmdUpdatePlayer2ScoreUI(int _score) {
        currentScoreP2 = _score;
        scoreP2.text = "Player2 score: " + (currentScoreP2.ToString());
        RpcPushUI();
        }

    [Command]
    public void CmdUpdateTurnUI() {
        if (!playerObjects[0].GetComponent<PlayerTry>().myTurn) {
            currentTurn = 1;
            }
        else {
            currentTurn = 2;
            }
        whosTurn.text = "Player" + currentTurn.ToString() + "'s turn!";
        RpcUpdateTurn(currentTurn);
        RpcPushUI();
        }

    [Command]
    public void CmdUpdateCurrentKeyUI(string _key) {
        currentKey = _key;
        requestedKey.text = currentKey;
        RpcUpdateCurrentKey(_key);
        RpcPushUI();
        }

    [Command]
    public void CmdUpdateResultUI(string _result) {
        currentResult = _result;
        result.text = currentResult;
        RpcUpdateResult(_result);
        RpcPushUI();
        }

    [Command]
    public void CmdUpdateState(string _state) {
        RpcUpdateState(_state);

        }


    [ClientRpc]
    public void RpcStartGame() {
        GameHolder.SetActive(true);
        }

    [ClientRpc]
    public void RpcPushUI() {
        //ALL UI
        GameHolder.SetActive(true);
            whosTurn.text = "Player" + currentTurn.ToString() + "'s turn!";

            scoreP1.text = "Player1 score: " + (currentScoreP1.ToString());
            scoreP2.text = "Player2 score: " + (currentScoreP2.ToString());

            requestedKey.text = currentKey;
            result.text = "You chose: "+ currentResult;
        //Debug.Log("Turn: " + currentTurn.ToString() + " scorep1: " + currentScoreP1.ToString() + " scorep2: " + scoreP2.ToString());
        //Debug.Log("current key: " + currentKey.ToString() + " currentresult: " + currentResult.ToString());            
        }

    [ClientRpc]
    public void RpcUpdateCurrentKey(string _currentKey) {
        currentKey = _currentKey;
        }

    [ClientRpc]
    public void RpcUpdateState(string _currentState) {
        state.text = _currentState;
        }

    [ClientRpc]
    public void RpcUpdateResult(string _currentResult) {
        currentResult = _currentResult;
        }

    [ClientRpc]
    public void RpcUpdateTurn(int _currentTurn) {
        currentTurn = _currentTurn;
        }

    [ClientRpc]
    public void RpcUpdateScore1() {
        if(!isServer)
            currentScoreP1 += 1;
        }

    [ClientRpc]
    public void RpcUpdateScore2() {
        if (!isServer)
            currentScoreP2 += 1;
        }

    public void SetupUI() {
        whosTurn.text = "Player" + currentTurn.ToString() + "'s turn!";

        scoreP1.text = "Player1 score: " + (currentScoreP1.ToString());
        scoreP2.text = "Player2 score: " + (currentScoreP2.ToString());

        requestedKey.text = currentKey;
        result.text = "You chose: " + currentResult;
        }

}

/*
gameflow

Server gaat open

Speler 1 joined
Speler 2 joined

Server moet spelers opslaan
Server start de gameloop
Server moet speler 1 de beurt geven

Server moet speler 1 input registreren en de beurt doorgeven naar speler 2

Speler 2 moet naar server input sturen
Server controleert deze input en handelt ernaar
Speler 2 moet input sturen en de server moet de beurt doorgeven naar speler 1 

Enzovoorts

Als iemand disconnect dan pauzeert het spel


        public Text whosTurn;
    public Text scoreP1;
    public Text scoreP2;
    public Text requestedKey;
    public Text result;

*/

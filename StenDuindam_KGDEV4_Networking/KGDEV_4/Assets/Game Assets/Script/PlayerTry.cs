using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerTry : NetworkBehaviour {

    [SyncVar]
    public bool myTurn = false;
    [SyncVar]
    public bool GameStarted = false;
    [SyncVar]
    public int myScore;
    public string myName;

    public ConnectToDatabase myConnection;
    // Use this for initialization
    void Start() {
        //TurnScriptTry.Input();
        DontDestroyOnLoad(this);
        myConnection = FindObjectOfType<ConnectToDatabase>();
        myName = myConnection.playerName;
        }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer)
            return;

        if (GameStarted) {
            if (Input.GetKeyDown(KeyCode.A)) {
                TurnScriptTry.ManageTurns(this, KeyCode.A);
                Debug.Log("A");
                }
            if (Input.GetKeyDown(KeyCode.S)) {
                TurnScriptTry.ManageTurns(this, KeyCode.S);
                Debug.Log("S");
                }
            if (Input.GetKeyDown(KeyCode.D)) {
                TurnScriptTry.ManageTurns(this, KeyCode.D);
                }
            if (Input.GetKeyDown(KeyCode.W)) {
                TurnScriptTry.ManageTurns(this, KeyCode.W);
                }
            }
        }

    [Command]
    public void CmdSendScore() {
        myConnection.AddScoreToDB(myScore);
        Debug.Log("sending score " + gameObject.name);
    }

    [Command]
    public void CmdSwitchScene() {
        SceneManager.LoadScene(2);
       
        }

    public override void OnStartLocalPlayer() {
       CmdPlayerJoined();
        }

    [Command]
    public void CmdPlayerJoined() {
        Manager.instance.CmdPlayerJoined(this.gameObject);
    }
    [Command]
    public void CmdChangeTurn() {
        Manager.instance.CmdChangeTurns();
    } 
    
    [Command]
    public void CmdUpdateScoreUI(GameObject _player) {
        Manager.instance.CmdUpdatePlayerScoreUI(_player);
        }

    [Command]
    public void CmdUpdateCurrentKeyUI(string _key) {
        Manager.instance.CmdUpdateCurrentKeyUI(_key);
        }

    [Command]
    public void CmdUpdateCurrentStateUI(string _state) {
        Manager.instance.CmdUpdateState(_state);
        }

    [Command]
    public void CmdUpdateResultUI(string _result) {
        Manager.instance.CmdUpdateResultUI(_result);
        }

    [Command]
    public void CmdUpdateTurnUI() {
        Manager.instance.CmdUpdateTurnUI();
        }

    }

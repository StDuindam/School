using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//server side
public static class TurnScriptTry  {

    //Turnbased input management
    private static bool action = true;
    [SyncVar]//Can dit?
    private static KeyCode correctCounterKey;

    //Manage the players and turns
    private static int turn;

    /*SUMMARY
     * A counters S
     * S counters D
     * D counters W
     * W counters A         
    */
 
    [Command]
	public static void CmdInput(PlayerTry player, KeyCode pressedKey) {
        if (action) {
            switch (pressedKey) {
                case KeyCode.A:
                    correctCounterKey = KeyCode.W;
                    player.CmdUpdateCurrentKeyUI("A");
                    player.CmdUpdateTurnUI();
                    break;
                case KeyCode.S:
                    correctCounterKey = KeyCode.A;
                    player.CmdUpdateCurrentKeyUI("S");

                    player.CmdUpdateTurnUI();
                    break;
                case KeyCode.D:
                    correctCounterKey = KeyCode.S;
                    player.CmdUpdateCurrentKeyUI("D");
                    break;
                case KeyCode.W:
                    correctCounterKey = KeyCode.D;
                    player.CmdUpdateCurrentKeyUI("W");
                    break;
                default:
                    Debug.Log("Wrong input: " + pressedKey);
                    break;
                
            }
            //UI Elements active/inactive etc.

            //Swap turn!

            //Manager.instance.CmdChangeTurns();       
            player.CmdUpdateCurrentStateUI("React!");
            player.CmdUpdateTurnUI();
            player.CmdChangeTurn();
            action = false;

        }else {
            //does pressedKey equal reaction?
            if(pressedKey == correctCounterKey) {
                //Score add
                player.CmdUpdateResultUI("CORRECT!");
                player.CmdUpdateScoreUI(player.gameObject);
                }
            else {
                //Score subtract
                player.CmdUpdateResultUI("WRONG!");
            }

            //stay in turn
            player.CmdUpdateCurrentStateUI("Act!");
            action = true;
        }
    }

    public static void ManageTurns(PlayerTry player,KeyCode key) {
        if (player.myTurn) {
            //Debug.Log("requested");
            CmdInput(player,key);
        }
        //set player 1 to be turn one

    }

}

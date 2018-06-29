using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScoutForTarget : BTNode {

    BlackBoard myBlackBoard;

    //constructor requires blackboard for access to all variables
    public ScoutForTarget(BlackBoard bb) {
        myBlackBoard = bb;
        }

    //Main function for this action
    public override ReturnType Run() {
        if (myBlackBoard.target != null) {
            return ReturnType.Success;
            }
        if (RayCastForTarget() || myBlackBoard.target != null) {
            return ReturnType.Success;
            }
        return ReturnType.Running;
        }

    //See if we can get a target, if we find a target add that to our blackboard
    //and then return true, otherwise return false.
    private bool RayCastForTarget() {

        //Cast a detection sphere to find a targetable agent or player
        Collider[] hitColliders = Physics.OverlapSphere(myBlackBoard.currentPosition(), myBlackBoard.lookRange);
        foreach(Collider c in hitColliders) {
                if(c.transform.tag == "Targetable" && c.transform != myBlackBoard.myAIObject.transform) {
                myBlackBoard.target = c.transform.gameObject;
                if (myBlackBoard.isNavmeshAgent) {
                    NavMeshAgent agent = myBlackBoard.myAIObject.GetComponent<NavMeshAgent>();
                    //agent.destination = myBlackBoard.currentPosition();
                    agent.isStopped = true;
                    }
                return true;
                }
            }

        return false;
        }
}

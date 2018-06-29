using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : BTNode {

    private BlackBoard myBlackBoard;

    public MoveToTarget(BlackBoard bb) {
        myBlackBoard = bb;
        }
    		
    public override ReturnType Run() {

        if (Vector3.Distance(myBlackBoard.currentPosition(), myBlackBoard.targetPosition()) < myBlackBoard.myRange && myBlackBoard.target != null) {
            myBlackBoard.currentAction = null;            
            return ReturnType.Success;
            
            }
        if(myBlackBoard.target != null) {
            myBlackBoard.myAIObject.transform.position = Vector3.MoveTowards(myBlackBoard.currentPosition(), myBlackBoard.targetPosition(), myBlackBoard.moveSpeed * 2);
            myBlackBoard.myAIObject.transform.LookAt(myBlackBoard.target.transform);
            myBlackBoard.currentAction = this;
            return ReturnType.Running;
            }
        return ReturnType.Failure;
        }
    }

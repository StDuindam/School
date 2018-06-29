using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundRandom : BTNode {

    private BlackBoard myBlackBoard;
    private Vector3 direction;

    public MoveAroundRandom(BlackBoard bb) {
        myBlackBoard = bb;
        CreateDestination();
        }

    public void Start() {

        }

    public override ReturnType Run() {
        if (!myBlackBoard.canMove) {
            return ReturnType.Failure;
            }
        //walk
        myBlackBoard.myAIObject.transform.position = Vector3.MoveTowards(myBlackBoard.currentPosition(), myBlackBoard.currentPosition() + direction, myBlackBoard.moveSpeed);
        return ReturnType.Running;
        }

    public Vector3 CreateDestination() {
        direction = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        if (direction == new Vector3(0, 0, 0)) {
            CreateDestination();
            }

        return direction;
        }
}

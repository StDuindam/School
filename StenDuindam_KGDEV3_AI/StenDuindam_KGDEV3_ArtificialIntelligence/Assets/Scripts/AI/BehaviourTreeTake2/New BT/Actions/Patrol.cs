using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : BTNode {

    private List<Transform> waypoints;
    private BlackBoard myBlackBoard;
    private Transform currentTargetWaypoint;
    private int waypointIterator;
    private NavMeshAgent agent;

    public Patrol(BlackBoard bb, List<Transform> wps) {
        myBlackBoard = bb;
        waypoints = wps;
        currentTargetWaypoint = waypoints[waypointIterator];
        agent = myBlackBoard.myAIObject.GetComponent<NavMeshAgent>();
        waypointIterator = UnityEngine.Random.Range(0, waypoints.Count);
        }
    
    public override ReturnType Run() {
        if (!myBlackBoard.canMove)
            return ReturnType.Failure;

        if(Vector3.Distance(myBlackBoard.currentPosition(), currentTargetWaypoint.position) > 0.5 && !myBlackBoard.hasTarget()) {
            //myBlackBoard.myAIObject.transform.position = Vector3.MoveTowards(myBlackBoard.currentPosition(), currentTargetWaypoint.position, myBlackBoard.moveSpeed / 2);
            agent.destination = currentTargetWaypoint.position;
            Debug.Log("Patrolling");
            }
        else {
            if (waypointIterator < waypoints.Count -1) {
                waypointIterator += 1;
                }
            else {
                waypointIterator = 0;
                }

            currentTargetWaypoint = waypoints[waypointIterator];
            }
        return ReturnType.Running;
        }


}

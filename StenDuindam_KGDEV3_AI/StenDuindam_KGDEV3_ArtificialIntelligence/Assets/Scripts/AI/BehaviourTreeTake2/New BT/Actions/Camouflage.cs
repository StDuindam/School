using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camouflage : BTNode {

    private Color startColour;
    private BlackBoard myBlackBoard;
    public bool isHiding;

    void Start() {
        startColour = myBlackBoard.myAIObject.GetComponent<Renderer>().material.color;
        }

    public Camouflage(BlackBoard bb) {
        myBlackBoard = bb;
        }

    public override ReturnType Run() {
        if (isHiding) {
            return ReturnType.Success;
            }
        Ray ray;
        //Cast raycasts in all directions
        //forward
        ray = new Ray(myBlackBoard.currentPosition(), myBlackBoard.myAIObject.transform.forward);
        RayCast(ray);
        //down
        ray = new Ray(myBlackBoard.currentPosition(), -myBlackBoard.myAIObject.transform.forward);
        RayCast(ray);
        //right
        ray = new Ray(myBlackBoard.currentPosition(), myBlackBoard.myAIObject.transform.right);
        RayCast(ray);
        //left
        ray = new Ray(myBlackBoard.currentPosition(), -myBlackBoard.myAIObject.transform.right);
        RayCast(ray);
       
        return ReturnType.Failure;
        }


    private bool RayCast(Ray ray) {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.5f)) {
            if (hit.transform.gameObject.layer == 8) {
                SwitchColor(hit.transform.GetComponent<Renderer>().material.color);
                isHiding = true;
                myBlackBoard.canMove = false;
                
                return true;
                }
            }
        return false;
        }

    // Use this for initialization
    private void SwitchColor (Color targetcolour) {
        if (targetcolour != startColour) {
            myBlackBoard.myAIObject.GetComponent<Renderer>().material.color = targetcolour;
            myBlackBoard.myAIObject.tag = "Untagged";
            }
        else {
            myBlackBoard.myAIObject.GetComponent<Renderer>().material.color = startColour;
            }
	}



}

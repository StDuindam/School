using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoard {

    public BlackBoard(GameObject myObject) {
        myAIObject = myObject;
        }
    
    //Target
    public GameObject target;
    public Vector3 targetPosition() {
        if (target != null) {
            return target.transform.position;
            }
        else {
            return new Vector3(0, 0, 0);
            }
        }

    //Self
    public BTNode currentAction;
    public GameObject myAIObject;
    public Vector3 currentPosition() { return myAIObject.transform.position;}
    public bool hasTarget() {
        if (target != null)
            return true;
        else {
            return false;
            }
        }

    public bool canMove = true;
    public float moveSpeed = 0.25f;
    public bool isNavmeshAgent = false;
    public bool canAttack = false;

    public int myRange = 2;
    public float lookRange = 1.5f;
    public Animator anim;
    }

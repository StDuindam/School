using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;
    public DungeonGenerator dgen;
    //private BehaviourTree bt;
	// Use this for initialization
	public void Init () {
        dgen = this.GetComponent<DungeonGenerator>();
        player.transform.position = dgen.GetStartPos();
        //bt = new BehaviourTree(new Selector select(new Action act));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

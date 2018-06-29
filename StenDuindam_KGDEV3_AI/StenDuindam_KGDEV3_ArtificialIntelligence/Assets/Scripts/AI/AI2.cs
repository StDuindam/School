using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Close combat
public class AI2 : MonoBehaviour, IActor {

    private AgentSpecs stats;
    private BlackBoard blackboard;
    private BTNode root;
    private float timer = 0;
    public GameObject projectile;
    public List<Transform> waypoints;
    private bool startUp = false; 
    public void SetWP(List<Transform> wp) {
        waypoints = wp;
        } 

    public void Init() {
        if (this.GetComponent<AgentSpecs>() != null) {
            stats = this.GetComponent<AgentSpecs>();
            }
        else {
            this.gameObject.AddComponent<AgentSpecs>();
            }



        //Setup tree
        blackboard = new BlackBoard(this.gameObject);

        //Edit blackboard variables
        blackboard.isNavmeshAgent = true;
        blackboard.myRange = 2;
        blackboard.lookRange = 1;

        root = new Selector(
            new Sequencer(
                new ScoutForTarget(blackboard),
                new MoveToTarget(blackboard),
                new AttackTarget(blackboard,stats, AttackStyle.range, projectile)
                ),
            //Patrol
            new Patrol(blackboard, waypoints)
            );

        //Go!
        startUp = true;
        }


    public void Update() {
        //Run the tree
        if (startUp) {
            if (blackboard.currentAction == null) {
                root.Run();
                }
            else {
                blackboard.currentAction.Run();
                }

            UpdateCanAttack();
            }
        }

    public void UpdateCanAttack() {
        if (blackboard.canAttack)
            return;

        if (timer < stats.attackSpeed) {
            timer += Time.deltaTime;
            }
        else { blackboard.canAttack = true; timer = 0; }

        
        }

    
    public void TakeDamage(int damage) {
        if ((stats.health - damage) > 0) {
            stats.health -= damage;
            Debug.Log("I am damaged" + name);
            }
        else {
            this.gameObject.SetActive(false);
            }

            }
    }

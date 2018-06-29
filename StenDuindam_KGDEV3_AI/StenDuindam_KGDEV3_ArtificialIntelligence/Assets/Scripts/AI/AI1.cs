using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Sniper
public class AI1 : MonoBehaviour, IActor {

    private AgentSpecs stats;
    private BlackBoard blackboard;
    [SerializeField]
    private Selector root;

    //Timer for attacks
    private float timer = 0;

    public void Start() {
        Init();
        }

    public void Init() {
        if (this.GetComponent<AgentSpecs>() != null) {
            stats = this.GetComponent<AgentSpecs>();
            }
        else {
            this.gameObject.AddComponent<AgentSpecs>();
            }

        //Setup Behaviour tree
        blackboard = new BlackBoard(this.gameObject);

        root = new Selector(
            new Sequencer(
                //Go into hiding, to ambush later
                new Camouflage(blackboard),
                new ScoutForTarget(blackboard),
                new MoveToTarget(blackboard),
                new AttackTarget(blackboard, stats, AttackStyle.melee, null)

                ),
            new Selector(
                //walk around and look for a spot to hide)
                new MoveAroundRandom(blackboard)
                )
        );
        }

    void Update() {
        if (blackboard.currentAction == null) {
            root.Run();
            }
        else {
            Debug.Log("we got an action stored!");
            blackboard.currentAction.Run();
            }

        UpdateCanAttack();
        }

    private void UpdateCanAttack() {
        if (blackboard.canAttack)
            return;

        if (timer < stats.attackSpeed) {
            timer += Time.deltaTime;
            }
        else { blackboard.canAttack = true; timer = 0; }


        }

    public void TakeDamage(int damage) {
        if (stats.health > 0) {
            stats.health -= damage;
            }
        else {
            this.gameObject.SetActive(false);
            }
        Debug.Log("I am damaged" + name);
        }
    }

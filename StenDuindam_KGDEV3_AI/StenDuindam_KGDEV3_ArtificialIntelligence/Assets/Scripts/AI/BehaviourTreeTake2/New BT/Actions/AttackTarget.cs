using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStyle { melee, range};

public class AttackTarget : BTNode {

    private BlackBoard myBlackBoard;
    private AgentSpecs stats;
    private AttackStyle myAttackStyle;
    private GameObject projectile;


    public AttackTarget(BlackBoard bb, AgentSpecs s, AttackStyle attackstyle, GameObject projectile) {
        myBlackBoard = bb;
        stats = s;
        myAttackStyle = attackstyle;
        this.projectile = projectile;
        }

    public override ReturnType Run() {

        if (!myBlackBoard.hasTarget())
            return ReturnType.Failure;

        if (myBlackBoard.canAttack) {
            Attack(myBlackBoard.target.transform);
            }

        return ReturnType.Success;
        }

    private void Attack(Transform enemy) {
        myBlackBoard.myAIObject.tag = "Targetable";
        switch (myAttackStyle) {
            case AttackStyle.melee:
                if (enemy.gameObject.GetComponent<IActor>() != null) {
                    enemy.gameObject.GetComponent<IActor>().TakeDamage(stats.damage);
                    myBlackBoard.target = null;
                    }
                myBlackBoard.canAttack = false;
                break;
            case AttackStyle.range:

                Vector3 rot = myBlackBoard.myAIObject.transform.position - myBlackBoard.targetPosition();

                GameObject bullet = Instantiate(projectile, myBlackBoard.currentPosition(), Quaternion.LookRotation(rot));
                bullet.gameObject.GetComponent<BulletScript>().SetBoss(myBlackBoard.myAIObject, stats.damage);
                Destroy(bullet, 2f);
                myBlackBoard.canAttack = false;
                break;
            }
        }
    }

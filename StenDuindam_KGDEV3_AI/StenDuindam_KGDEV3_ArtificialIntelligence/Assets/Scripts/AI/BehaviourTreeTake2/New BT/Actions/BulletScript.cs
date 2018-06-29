using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    private Rigidbody rBody;
    private GameObject boss;
    private int damage;
    public bool inverse = false;
    // Use this for initialization   

    void Start() {
        rBody = this.GetComponent<Rigidbody>();
        if (!inverse) {
            rBody.AddForce(-transform.forward * 300f);
            }
        else {
            rBody.AddForce(transform.forward * 300f);
            }
        }
    public void SetBoss(GameObject b, int dmg) {
        boss = b;
        damage = dmg;
        }

    public void OnTriggerEnter(Collider col) {
        Debug.Log(boss + "deadbullet" + col.name);
        if (col.gameObject.tag == "Targetable" && col.gameObject != boss) {
            if (col.gameObject.GetComponent<IActor>() != null) {
                col.gameObject.GetComponent<IActor>().TakeDamage(damage);
                }
            }
        else if(col.gameObject == boss) { return; }
        else if(col.gameObject.layer == 8) {
            col.gameObject.SetActive(false);
            }
            
            

        Destroy(this.gameObject);
        }
    }

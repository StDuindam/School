using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShoot : MonoBehaviour, IActor {

    private int dmg = 20;
    private int hp = 110;
    public GameObject projectile;

    // Use this for initialization
    void Start() {

        }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
            }
        }

    public void Init() {
        return;

        }
    public void TakeDamage(int damage) {
        if (hp <= 0) {
            this.gameObject.SetActive(false);
            }
        else { hp -= 10; }
        }
    public void Shoot() {
        Debug.Log("Shoot");
        GameObject go = Instantiate(projectile, transform.position, this.gameObject.transform.rotation);
        go.GetComponent<BulletScript>().inverse = true;
        go.GetComponent<BulletScript>().SetBoss(this.gameObject, dmg);
        Destroy(go, 3f);
        }
    }

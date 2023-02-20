using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject explosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        explosionEffect = GameObject.Find("ExplosionEffect");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision col)
    {
        if(this.name == "BlueTailBullet(Clone)")
        {
            ContactPoint contact = col.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            GameObject ExplosionEffect = Instantiate(explosionEffect, pos, rot);
            Destroy(this.gameObject);
            Destroy(ExplosionEffect,5f);
        }     
    }
}

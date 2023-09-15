using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    public GameObject explosionEffect;
    public bool isExploded = false;
    public List<GameObject> GrenadeExplode;
    private int explosionRadius = 5;
    public LayerMask CollidableLayers;
    // Start is called before the first frame update
    void Start()
    {
        explosionEffect = GameObject.Find("GrenadeExplosionEffect");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        GameObject exp = Instantiate(explosionEffect, pos, rot);

        Collider[] colliders = Physics.OverlapSphere(pos, explosionRadius, CollidableLayers);
        foreach (Collider col in colliders)
        {
            GrenadeExplode.Add(col.gameObject);
        }

        isExploded = true;
       // Destroy(gameObject);
        Destroy(exp, 5f);
    }

    public List<GameObject> GetExplodeObjects()
    {

        return GrenadeExplode;
    }


}

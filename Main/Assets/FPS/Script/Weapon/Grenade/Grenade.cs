using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;

public enum GrenadeType
{
   Excitatory,
   Inhabitory
};

public class Grenade : MonoBehaviour
{
    public GameObject grenadeModel;
    private int explosionRadius = 5;
    private int explosionPower = 20;
   
    private GrenadeType type;

    // trajectory settings
    public int numberOfPoints = 50;
    public float timeBetweenPoints = 0.1f;
    public LayerMask CollidableLayers;
    public LineRenderer lineRenderer;
    

    private void Start()
    {
        
    }
    public void drawTrajectory(Vector3 startPosition, Vector3 aimDirection)
    { 
        lineRenderer.positionCount = numberOfPoints;
        List<Vector3> points = new List<Vector3>();
        Vector3 startSpeed = aimDirection.normalized * explosionPower;
        for (float i = 0; i < numberOfPoints; i += timeBetweenPoints) //totalPoints = numberOfPoints / timeBetweenPoints
        {
            Vector3 newPoint = startPosition + i * startSpeed;
            newPoint.y = startPosition.y + startSpeed.y * i + Physics.gravity.y / 2f * i * i;
            points.Add(newPoint);
            Collider[] colliders = Physics.OverlapSphere(newPoint, 0.2f, CollidableLayers);
            if (colliders.Length > 0)
            {
                lineRenderer.positionCount = points.Count;
                break;
            }
        }
        
        lineRenderer.SetPositions(points.ToArray());
    }

    public void deleteTrajectory()
    {
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
    }
}

// public class ExcitatoryGrenade : Grenade 
// {
//     
// }
//
// public class InhabitoryGrenade : Grenade
// {
//     
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    //Distance for calculate
    private Vector3 oldDistanceToTarget;
    private Vector3 newDistanceToTarget;
    private Vector3 distanceBetweenOldAndNewPos;

    private void Start()
    {
        LoadTargetForCamera();
        oldDistanceToTarget = target.position - transform.position;
    }

    private void Update()
    {
        newDistanceToTarget = target.position - transform.position;
        distanceBetweenOldAndNewPos = newDistanceToTarget - oldDistanceToTarget;
        transform.Translate(distanceBetweenOldAndNewPos);
    }

    protected virtual void LoadTargetForCamera()
    {
        if (target != null) return;
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class VelocityTracker : MonoBehaviour
{
    private Transform targetTr;

    private bool isInit = false;

    private Queue<Vector3> velocityQueue = new Queue<Vector3>();
    private int queueCount = 30;

    private Vector3 prePos = Vector3.zero;
    public void Init(Transform targetTr)
    {
        this.targetTr = targetTr;
        isInit = true;
    }

    private void Update()
    {
        if (!isInit) return;

        //queue velocity stacking 
        if(velocityQueue.Count >= queueCount)
        {
            velocityQueue.Dequeue();
        }

        Vector3 curVelocity = (targetTr.position - prePos) / Time.deltaTime;
        velocityQueue.Enqueue(curVelocity);
        prePos = targetTr.position;
    }

    public Vector3 GetAverageVelocity()
    {
        if(velocityQueue.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach(var vel in velocityQueue)
        {
            sum += vel;
        }

        return sum / velocityQueue.Count;
    }

}

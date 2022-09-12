using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletTracer : MonoBehaviour
{
    private GameObject Parent;
    public IEnumerator setLine(Vector3 startPos, Vector3 endPoint, Color teamColor, float trailTime, float trailWidth, GameObject sender)
    {
        LineRenderer line = gameObject.GetComponent<LineRenderer>();
        Parent = sender;
        //Sets the start and end positions of the tracers
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPoint);
        //Sets the tracer's colour
        line.startColor = new Color(teamColor.r, teamColor.g, teamColor.b, 1);
        line.endColor = new Color(teamColor.r, teamColor.g, teamColor.b, 1);
        //Reduces the width of the tracer over time
        float time = trailTime;
        while (time > 0)
        {
            line.startWidth = trailWidth * time;
            line.endWidth = line.startWidth;
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
    void Update()
    {
        if(Parent != null)
        {
            if(Parent.activeSelf == false)
            {
                Destroy(gameObject);
            }
        }
    }
}

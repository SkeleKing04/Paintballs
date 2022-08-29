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
        // Sets how long the trail is visible for
        //this needs to be a different variable
        // the colour here should corespond to the colour of the player's team/paint
        try
        {
            teamColor = GetComponent<TeamManager>().teamColor;
            //Debug.Log(gameObject.name + "'s team colour is " + teamColor.ToString());
            line.startColor = new Color(teamColor.r, teamColor.g, teamColor.b, 1);
            line.endColor = new Color(teamColor.r, teamColor.g, teamColor.b, 1);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to update the colour of the line. Does the sender have team manager attached?\nThe error is " + e);
        }

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
        if(Parent.activeSelf == false)
        {
            Destroy(gameObject);
        }
    }
}

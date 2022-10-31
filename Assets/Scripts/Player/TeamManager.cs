using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamManager : MonoBehaviour
{
    public Color teamColor;
    public new Renderer renderer;
    public void UpdateColour()
    {
        try
        {
            renderer.material.color = teamColor;

        }
        catch (Exception e)
        {
            Debug.LogWarning("Unable to update " + gameObject.name + "'s colour!\n Is there are renderer attached?");
        }
    }
}

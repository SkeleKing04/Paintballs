using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public Color teamColor;
    public new Renderer renderer;
    public void UpdateColour()
    {
        renderer.material.color = teamColor;
    }
}

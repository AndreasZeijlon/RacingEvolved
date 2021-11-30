using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public float[] bestScores = new float[2];
    public bool enableMusic, enableEffects, enableInstructions; 
    public int[] playerControls = new int[4];
}
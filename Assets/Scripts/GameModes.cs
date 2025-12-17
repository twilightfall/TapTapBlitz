using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class GameMode : ScriptableObject
{
    public float timerBase;
    public float timerDecay;
    public float timerMax;

    public int decayInterval;

    [TextArea(3,10)]
    public string modeDescription;

    [TextArea(3, 10)]
    public string hintText;

    public bool isActive;
    public bool isMultiplayer;

    public void ResetMode()
    {
        isActive = false;
    }

    [Header("Grid Variables")]
    public int columns;
    public int rows;
    public float cellSize;
    public float padding;
}
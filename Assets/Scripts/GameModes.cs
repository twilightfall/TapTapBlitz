using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameMode : ScriptableObject
{
    public float timerBase;
    public float timerDecay;
    public float timerMax;

    public int decayInterval;

    public string modeDescription;
}
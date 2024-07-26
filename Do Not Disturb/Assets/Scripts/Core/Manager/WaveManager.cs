using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public enum States { ready, battle, boss, gameEnd };

    public int stage;
    public int maxStage;
    public States state;

    public float battleTime;
    public float readyTime;
    public float playTime;

    public float coolTimeOfReady;
    public float coolTimeOfBattle;
}

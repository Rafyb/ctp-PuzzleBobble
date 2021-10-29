using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "PuzzleBobbleLevel")]
public class Level : ScriptableObject
{
    public BallType[] ligne1 = new BallType[8];
    public BallType[] ligne2 = new BallType[7];
    public BallType[] ligne3 = new BallType[8];
    public BallType[] ligne4 = new BallType[7];
    public BallType[] ligne5 = new BallType[8];
    public BallType[] ligne6 = new BallType[7];
    public BallType[] ligne7 = new BallType[8];
    public BallType[] ligne8 = new BallType[8];
}

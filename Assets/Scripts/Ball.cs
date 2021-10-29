using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum BallType
{
    NONE,BLUE,GREEN,PURPPLE,ORANGE,YELLOW
}
public class Ball : MonoBehaviour
{

    public BallType type;
    public List<Ball> neighbour = new List<Ball>();
    public bool marked = false;
    public bool wallFixed = false;
    private void OnDrawGizmos()
    {
        int increment = 10;
        Vector3 pos = transform.position;
        float maxDist = 1f;
        for (int angle = 0; angle < 360; angle = angle + increment)
        {
            Handles.DrawWireDisc(pos, new Vector3(0, 0, 1), maxDist);
        }
    }

    public List<Ball> FindMatch3(BallType type)
    {
        marked = true;
        List<Ball> balls = new List<Ball>();
        
        if (this.type == type)
        {
            // S'ajoute à la liste
            balls.Add(this);
            
            
            foreach (Ball ball in neighbour)
            {
                if (ball.marked) continue;
                List<Ball> neighbourBalls = ball.FindMatch3(type);
                foreach (Ball b in neighbourBalls)
                {
                    if(!balls.Contains(b)) balls.Add(b);
                }
            }
        }
        
        return balls;
    }
    
    public bool isFixed()
    {
        marked = true;
        if(wallFixed) return true;
        for (int idx = 0; idx < neighbour.Count; idx++)
        {
            if(!neighbour[idx].marked)
                if (neighbour[idx].isFixed()) 
                    return true;
        }
        
        return false;
    }


}

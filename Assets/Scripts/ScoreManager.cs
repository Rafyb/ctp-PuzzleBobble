using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text levelText;

    public int score = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int value)
    {
        score += value;
        scoreText.text = "Score : "+(""+score).PadLeft(4,'0');
    }

    public void UpdateLevel(int value)
    {
        levelText.text = "Level : " + value;
    }

    public void ClearScore()
    {
        score = 0;
        scoreText.text = "Score : "+(""+score).PadLeft(4,'0');
    }
}

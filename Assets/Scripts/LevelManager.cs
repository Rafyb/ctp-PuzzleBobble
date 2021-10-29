using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public List<Level> levels;
    private int level = 0;

    public ScoreManager score;

    public GameObject win;
    public GameObject lose;
    public Image switchImg;
    
    // Start is called before the first frame update
    void Start()
    {
        Game.Instance.addScore += score.UpdateScore;
        Game.Instance.endLevel += EndLevel;
        
        LoadLevel();
        Game.Instance.Play();
    }

    void EndLevel(bool w)
    {
        
        if (w)
        {
            level++;
            if (level == levels.Count) level = 0;
            win.transform.DOMoveY(win.transform.position.y + 8f, 1.5f).OnComplete(() =>
            {
                win.transform.DOMoveY(win.transform.position.y - 8f, 1.5f).OnComplete(() =>
                {
                    switchImg.DOFade(0, 0.1f).OnComplete(() =>
                    {
                        Game.Instance.Play();
                    });
                });
            });
        }
        else
        {
            lose.transform.DOMoveY(lose.transform.position.y + 8f, 1.5f).OnComplete(() =>
            {
                lose.transform.DOMoveY(lose.transform.position.y - 8f, 1.5f).OnComplete(() =>
                {
                    switchImg.DOFade(0, 0.1f).OnComplete(() =>
                    {
                        Game.Instance.Play();
                    });
                    
                });
            });
        }
        switchImg.DOFade(1, 0.1f).OnComplete(()=>{LoadLevel();});
        
    }

    void LoadLevel()
    {
        
        
        Game.Instance.ClearLevel();
        Game.Instance.LoadLevel(levels[level]);
        score.ClearScore();
        score.UpdateLevel(level+1);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

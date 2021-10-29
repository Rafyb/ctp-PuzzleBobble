using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [Header("Grid")] 
    //public float offestX = 0.55f;
    //public float offestY = 0.55f;
    public Transform origin;
    private List<Vector3> _gridPos = new List<Vector3>();
    private float neighboursDist = 1f;
    private Vector3 _originWall;
    
    [Header("Components")]
    public GameObject[] prefabBalls;
    public GameObject needle;
    public GameObject wall;
    public Transform limite;

    [Header("Parameters")] 
    public float timeWall = 8f;
    public float wallMove = 0.5f;
    
    public static Game Instance;
    public Action<int> addScore;
    public Action<bool> endLevel;
    
    private GameObject _ballReady;
    private GameObject _nextBall;
    private bool _locked = false;
    private List<Ball> _fixedBalls = new List<Ball>();
    private float _timer = 0f;
    private bool launched = false;
    

    private void Awake()
    {
        Instance = this;
        
        _originWall = wall.transform.position;
    }

    public void AddBall(Ball b)
    {
        if (_fixedBalls.Contains(b)) return;
        
        // Add
        _fixedBalls.Add(b);

        // Update links
        //UpdateNeighbours();
        
        foreach (Ball ball in _fixedBalls)
        {
            ball.neighbour.Clear();
            foreach (Ball otherBall in _fixedBalls)
            {
                
                if (otherBall == ball) continue;
                if (Vector3.Distance(ball.transform.position, otherBall.transform.position) <= neighboursDist)
                {
                    ball.neighbour.Add(otherBall);
                }
            }
        }
        
        // Find Match3
        
        foreach (Ball ball in _fixedBalls)
        {
            ball.marked = false;
        }
        
        List<Ball> ballsToDelete = b.FindMatch3(b.type);
        if (ballsToDelete.Count >= 3)
        {
            for (int idx = ballsToDelete.Count-1 ; idx >= 0 ; idx--)
            {
                _fixedBalls.Remove(ballsToDelete[idx]);
                ballsToDelete[idx].GetComponent<AnimatedSprite>().PlayExplose();
                Scoring(ballsToDelete[idx].transform.position, 10);
                Destroy(ballsToDelete[idx]);

                
            }
            
            UpdateNeighbours();
            
            
            
            List<Ball> ballsToDetach = new List<Ball>();
            foreach (Ball ball in _fixedBalls)
            {
                foreach (Ball node in _fixedBalls)
                {
                    node.marked = false;
                }
                if(!ball.isFixed()) ballsToDetach.Add(ball);
            }

            if (ballsToDetach.Count > 0)
            {
                for (int idx = ballsToDetach.Count-1 ; idx >= 0 ; idx--)
                {
                    _fixedBalls.Remove(ballsToDetach[idx]);
                    Scoring(ballsToDetach[idx].transform.position, 20);
                    Fall(ballsToDetach[idx].gameObject);
                    
                }
                UpdateNeighbours();
            }
        }

        // Victoire
        if (_fixedBalls.Count == 0)
        {
            endLevel.Invoke(true);
            launched = false;
        }
        
        // Defaite
        foreach (Ball ball in _fixedBalls)
        {
            if (ball.transform.position.y < limite.position.y)
            {
                endLevel.Invoke(false);
                launched = false;
                break;
            }
        }

    }

    private void Scoring(Vector3 pos, int value)
    {
        addScore.Invoke(value);
    }

    private void Fall(GameObject ball)
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector2(Random.Range(-30f,30F),20f));

        foreach (CircleCollider2D cirle in ball.GetComponents<CircleCollider2D>())
        {
            cirle.isTrigger = true;
        }
        
        Destroy(ball.GetComponent<Ball>());
        Destroy(ball,3f);
    }

    private void UpdateNeighbours()
    {
        foreach (Ball ball in _fixedBalls)
        {
            ball.neighbour.Clear();
            foreach (Ball otherBall in _fixedBalls)
            {
                if (otherBall == ball) continue;
                if (Vector3.Distance(ball.transform.position, otherBall.transform.position) <= neighboursDist)
                {
                    ball.neighbour.Add(otherBall);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!launched) return;
        
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = worldMouse - needle.transform.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x);
        angle = angle * Mathf.Rad2Deg - 90;
        
        if(angle >= -70 && angle <= 70) needle.transform.localRotation = Quaternion.Euler(0,0,angle);

        if (!_locked && !_ballReady )
        {
            NextBall();
        }
        
        if (Input.GetMouseButtonDown(0) && !_locked)
        {

            direction.Normalize();
            _ballReady.AddComponent<Projectile>().Initialize(direction);
            _ballReady = null;

            _locked = true;

        }

        if (_timer >= timeWall)
        {
            if (!_locked)
            {
                _locked = true;
                for (int idx = 0; idx < _gridPos.Count; idx++)
                {
                    Vector3 tmp = _gridPos[idx];
                    tmp.y -= wallMove;
                    _gridPos[idx] = tmp;
                }

                wall.transform.DOMoveY(wall.transform.position.y-wallMove, 0.5f).OnComplete( () =>
                {
                    _locked = false;
                    _timer -= timeWall;
                });
                
                for (int idx = 0; idx < _fixedBalls.Count; idx++)
                {
                    _fixedBalls[idx].gameObject.transform.DOMoveY(_fixedBalls[idx].gameObject.transform.position.y-wallMove,0.5f);
                }
                
                
            }
        }
        else
        {
            _timer += Time.deltaTime;
        }
        
        
    }

    void NextBall()
    {
        _ballReady = _nextBall;
        _ballReady.transform.position = needle.transform.position;
        
        _nextBall = GameObject.Instantiate(prefabBalls[Random.Range(0,prefabBalls.Length)]);
        _nextBall.transform.position = needle.transform.position + new Vector3(-2,-0.7f,0);
        _nextBall.GetComponent<Ball>().enabled = false;
    }

    public void Unlock()
    {
        _locked = false;
    }

    public Vector3 NearestPoint(Vector3 ballPos)
    {
        Vector3 nearestPoint = _gridPos[0];
        float dist = Vector3.Distance(nearestPoint, ballPos);
        for (int idx = 0; idx < _gridPos.Count; idx++)
        {
            float tmp = Vector3.Distance(_gridPos[idx], ballPos);
            if (tmp < dist)
            {
                dist = tmp;
                nearestPoint = _gridPos[idx];
            }
        }
        return nearestPoint;
    }

    public void ClearLevel()
    {
        foreach (Ball ball in _fixedBalls)
        {
            Fall(ball.gameObject);
        }
        _fixedBalls.Clear();
        
        wall.transform.position = _originWall;
        _timer = 0f;
        
        if (_ballReady != null)
        {
            Destroy(_ballReady.gameObject);
            _ballReady = null;
        }
        if (_nextBall != null)
        {
            Destroy(_nextBall.gameObject);
            _nextBall = null;
        }
        
        _gridPos = new List<Vector3>();
    }

    public void LoadLevel(Level level)
    {
        List<BallType[]> lignes = new List<BallType[]>();
        lignes.Add(level.ligne1);
        lignes.Add(level.ligne2);
        lignes.Add(level.ligne3);
        lignes.Add(level.ligne4);
        lignes.Add(level.ligne5);
        lignes.Add(level.ligne6);
        lignes.Add(level.ligne7);
        lignes.Add(level.ligne8);
        
        for (int y = 0; y < 14; y++)
        {
            int xlength = 7;
            if (y % 2 == 0) xlength = 8;
            for (int x = 0; x < xlength; x++)
            {
                // Création de la cellule
                Vector3 gridCell = new Vector3(
                    origin.position.x + 0.36f + x * 0.72f,
                    origin.position.y - 0.38f - y * 0.63f,
                    0f);
                if (y % 2 != 0) gridCell.x += 0.36f;
                _gridPos.Add(gridCell);

                // Création des billes
                if (y < lignes.Count)
                {
                    if (lignes[y][x] != BallType.NONE)
                    {
                        GameObject prefab = prefabBalls[0];
                        
                        foreach (GameObject go in prefabBalls)
                        {
                            if (go.GetComponent<Ball>().type == lignes[y][x]) prefab = go;
                        }
                        
                        _nextBall = GameObject.Instantiate(prefab);
                        _nextBall.transform.position = gridCell;
                        _nextBall.GetComponent<Ball>().enabled = false;
                        _nextBall.GetComponent<AnimatedSprite>().enabled = true;
                        if (y == 0) _nextBall.GetComponent<Ball>().wallFixed = true;
                        _fixedBalls.Add(_nextBall.GetComponent<Ball>());

                    }
  
                }

            }
        }
        UpdateNeighbours();
        
        _nextBall = GameObject.Instantiate(prefabBalls[Random.Range(0,prefabBalls.Length)]);
        _nextBall.transform.position = needle.transform.position + new Vector3(-2,-0.7f,0);
        _nextBall.GetComponent<Ball>().enabled = false;
        
        NextBall();
    }
    
    public void Play( )
    {
        launched = true;
    }

}

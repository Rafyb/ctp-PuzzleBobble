using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    private int _currentAnime = 0;
    private float _timer = 0f;
    private float _timerWait = 0f;
    private SpriteRenderer _sr;
    private bool _explo = false;
    
    public float framerate = 0.1f;
    public float timeBetween = 2f;
    public List<Sprite> sprites;
    public List<Sprite> explosion;
    
    // Start is called before the first frame update
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        _sr.sprite = sprites[0];
        _timerWait = timeBetween;
    }

    public void PlayExplose()
    {
        _explo = true;
        _currentAnime = -1;
        _timer = 0;
        //_sr.sprite = explosion[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (_explo)
        {
            if (_timer >= framerate)
            {
                _currentAnime++;
                if (_currentAnime < explosion.Count)
                {
                    _sr.sprite = explosion[_currentAnime];
                }
                else
                {
                    Destroy(gameObject);
                }
                _timer -= framerate;
            }
            else
            {
                _timer += Time.deltaTime;
            }
            return;
        }
        if (_timerWait >= timeBetween)
        {
            if (_timer >= framerate)
            {
                _currentAnime++;
                if (_currentAnime < sprites.Count)
                {
                    _sr.sprite = sprites[_currentAnime];
                }
                else
                {
                    _timerWait -= timeBetween;
                    _currentAnime = 0;
                    _sr.sprite = sprites[_currentAnime];
                }

                _timer -= framerate;
            }
            else
            {
                _timer += Time.deltaTime;
            }
            
        }
        else
        {
            _timerWait += Time.deltaTime;
        }
    }
}

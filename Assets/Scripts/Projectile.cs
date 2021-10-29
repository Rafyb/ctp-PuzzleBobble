using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Projectile : MonoBehaviour
    {
        public float speed = 7f;
        private Vector3 _direction;
        private bool active = true;

        public void Initialize(Vector3 direction)
        {
            _direction = direction;
        }

        void Update()
        {
            transform.position += _direction * speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            Ball otherBall = collision.GetComponent<Ball>();
            if( (otherBall != null || collision.tag.Equals("EndWall") ) && active)
            {
                active = false; 
                
                Ball ball = GetComponent<Ball>();
                ball.enabled = true;
                
                if (collision.tag.Equals("EndWall")) ball.wallFixed = true;
                
                AnimatedSprite anim = GetComponent<AnimatedSprite>();
                anim.enabled = true;
                
                Game.Instance.Unlock();

                transform.position = Game.Instance.NearestPoint(transform.position);
                Game.Instance.AddBall(ball);

                this.enabled = false;
                GameObject.Destroy(this);
                
            }
            else if (collision.tag.Equals("Wall"))
            {
                _direction = new Vector3(_direction.x*-1, _direction.y, _direction.z);
            }
        }

        /*
        void SetPosition(Vector3 origin)
        {
            

            int xDir = 1;
            int yDir = 1;
            
            
            if (origin.y - transform.position.y > 0) yDir *= -1;
            if (origin.x - transform.position.x > 0) xDir *= -1;
            
            transform.position = new Vector3(
                origin.x + xDir * Game.Instance.offestX, 
                origin.y + yDir * Game.Instance.offestY,
                origin.z
                );
        }*/
    }


using UnityEngine;
using System.Collections;

using DigitalRuby.Tween;

public enum Direction
{
    xAxis, zAxis
}

public class WallController : MonoBehaviour {

    public Direction direction;

    public float initialPos = 38.0f;
    public float targetPos = 0.0f;
    public float speed = 3.0f;

    private bool isMoving;
    private FloatTween moveTween;

    // Use this for initialization
    void Start () {
	    if (direction == Direction.xAxis)
        {
            Vector3 tmp = transform.position;
            transform.position = new Vector3(initialPos, tmp.y, tmp.z);
        }
        else
        {
            Vector3 tmp = transform.position;
            transform.position = new Vector3(tmp.x, tmp.y, initialPos);
        }

        speed = Random.Range(5.0f, 15.0f);

        StartCoroutine(WaitAndMove(Random.Range(0.0f, 5.0f)));

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private float getDuration()
    {
        return Mathf.Abs(targetPos - initialPos) / speed;
    }

    private IEnumerator WaitAndMove(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        TweenMove();
    }

    private void TweenMove()
    {
        isMoving = true;
        moveTween = gameObject.Tween("Move" + GetInstanceID(),
          initialPos,
          targetPos,
          getDuration(),
          TweenScaleFunctions.Linear, (t) =>
          {
              float pos = t.CurrentValue;
              if (direction == Direction.xAxis)
              {
                  Vector3 tmp = transform.position;
                  transform.position = new Vector3(pos, tmp.y, tmp.z);
              }
              else
              {
                  Vector3 tmp = transform.position;
                  transform.position = new Vector3(tmp.x, tmp.y, pos);
              }
          }, (t) =>
          {
              Debug.Log("Finished move.");
              isMoving = false;
          });
    }

    private void StopTweenMove()
    {
        moveTween.Stop(TweenStopBehavior.DoNotModify);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "MovingWall")
        {
            Debug.Log("Wall hit wall");
            WallController wallCtrl = collision.gameObject.GetComponent<WallController>();
            if (!wallCtrl.isMoving)
            {
                StopTweenMove();
            }
        }
    }
}

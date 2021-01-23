using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Goal : MonoBehaviour
{
    public float score;
    void OnTriggerEnter(Collider ball)
    {
        ball.gameObject.GetComponent<Ball>().UpdateReward(score);
    }
}

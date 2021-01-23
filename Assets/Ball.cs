using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Ball : Agent
{
    private Rigidbody ball;
    [SerializeField]
    private float highScore = 0.0f;
    [SerializeField]
    private float cummlativeScore = 0.0f;
    public int maxTries = 6;
    private int triesLeft;
    public float initialForce;
    public float initialPositionShift; // From -0.6 to +0.6
    [SerializeField]
    private Vector3 startingPosition;
    private bool isBallReady;
    private float maxTime = 6f;
    [SerializeField]
    private float timeLeft;
    private float timeSinceLastFrame;
    private Transform parent;
    private Goal goal_1;
    private Goal goal_2;
    private Goal goal_3;
    private Goal goal_4;
    private Goal goal_5;
    private Goal goal_6;
    private Goal goal_7;
    private float initialDistanceFromGoal_1;
    private float initialDistanceFromGoal_2;
    private float initialDistanceFromGoal_3;
    private float initialDistanceFromGoal_4;
    private float initialDistanceFromGoal_5;
    private float initialDistanceFromGoal_6;
    private float initialDistanceFromGoal_7;
    [SerializeField]
    private float calculatedDistanceReward;
    private StatsRecorder stats;

    public override void Initialize()
    {
        stats = Academy.Instance.StatsRecorder;
        ball = GetComponent<Rigidbody>();
        startingPosition = ball.transform.position;
        parent = transform.parent;
        goal_1 = parent.Find("goal_1").GetComponent<Goal>();
        goal_2 = parent.Find("goal_2").GetComponent<Goal>();
        goal_3 = parent.Find("goal_3").GetComponent<Goal>();
        goal_4 = parent.Find("goal_4").GetComponent<Goal>();
        goal_5 = parent.Find("goal_5").GetComponent<Goal>();
        goal_6 = parent.Find("goal_6").GetComponent<Goal>();
        goal_7 = parent.Find("goal_7").GetComponent<Goal>();
    }

    public override void OnEpisodeBegin()
    {
        triesLeft = maxTries + 1;
        Reset();
    }

    private void Reset()
    {
        ball.transform.position = startingPosition;
        ball.transform.rotation = Quaternion.Euler(Vector3.zero);
        ball.velocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;
        triesLeft -= 1;
        isBallReady = true;
        timeLeft = maxTime;
    }

    private void Update() {
        if(isBallReady)
        {
            RequestDecision();
        }
        if(triesLeft == 0)
        {
            cummlativeScore += highScore;
            stats.Add("Custom/Cumulative Score", cummlativeScore);
            highScore = 0.0f;
            EndEpisode();
        }
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            stats.Add("Custom/Time Left", timeLeft);
            stats.Add("Custom/Reward", 0);
            Reset();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        initialDistanceFromGoal_1 = (goal_1.transform.position - startingPosition).magnitude;
        initialDistanceFromGoal_2 = (goal_2.transform.position - startingPosition).magnitude;
        initialDistanceFromGoal_3 = (goal_3.transform.position - startingPosition).magnitude;
        initialDistanceFromGoal_4 = (goal_4.transform.position - startingPosition).magnitude;
        initialDistanceFromGoal_5 = (goal_5.transform.position - startingPosition).magnitude;
        initialDistanceFromGoal_6 = (goal_6.transform.position - startingPosition).magnitude;
        initialDistanceFromGoal_7 = (goal_7.transform.position - startingPosition).magnitude;
        sensor.AddObservation(goal_1.score);
        sensor.AddObservation(initialDistanceFromGoal_1);
        sensor.AddObservation(goal_2.score);
        sensor.AddObservation(initialDistanceFromGoal_2);
        sensor.AddObservation(goal_3.score);
        sensor.AddObservation(initialDistanceFromGoal_3);
        sensor.AddObservation(goal_4.score);
        sensor.AddObservation(initialDistanceFromGoal_4);
        sensor.AddObservation(goal_5.score);
        sensor.AddObservation(initialDistanceFromGoal_5);
        sensor.AddObservation(goal_6.score);
        sensor.AddObservation(initialDistanceFromGoal_6);
        sensor.AddObservation(goal_7.score);
        sensor.AddObservation(initialDistanceFromGoal_7);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = this.initialPositionShift;
        actionsOut[1] = this.initialForce;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        print(vectorAction[0] + " " + vectorAction[1]);
        Launch(vectorAction[0], vectorAction[1]);
    }

    private void Launch(float initialPositionShift, float initialForce)
    {
        isBallReady = false;
        ball.transform.position = new Vector3(startingPosition[0], startingPosition[1] , startingPosition[2]+initialPositionShift);
        ball.AddForce(initialForce, 0, 0, ForceMode.Impulse);
    }

    public void UpdateReward(float reward)
    {
        calculatedDistanceReward = goal_1.score * (initialDistanceFromGoal_1 - (goal_1.transform.position - ball.transform.position).magnitude);
        calculatedDistanceReward += goal_2.score * (initialDistanceFromGoal_2 - (goal_2.transform.position - ball.transform.position).magnitude);
        calculatedDistanceReward += goal_3.score * (initialDistanceFromGoal_3 - (goal_3.transform.position - ball.transform.position).magnitude);
        calculatedDistanceReward += goal_4.score * (initialDistanceFromGoal_4 - (goal_4.transform.position - ball.transform.position).magnitude);
        calculatedDistanceReward += goal_5.score * (initialDistanceFromGoal_5 - (goal_5.transform.position - ball.transform.position).magnitude);
        calculatedDistanceReward += goal_6.score * (initialDistanceFromGoal_6 - (goal_6.transform.position - ball.transform.position).magnitude);
        calculatedDistanceReward += goal_7.score * (initialDistanceFromGoal_7 - (goal_7.transform.position - ball.transform.position).magnitude);
        calculatedDistanceReward /= 140;
        AddReward(calculatedDistanceReward);
        highScore += 10*reward;
        AddReward(10*reward);
        stats.Add("Custom/Time Left", timeLeft);
        stats.Add("Custom/Reward", reward);
        Reset();
    }

}

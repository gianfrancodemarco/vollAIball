using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Blue = 0,
    Red = 1,
    Default = 2
}

public enum Event
{
    HitRedGoal = 0,
    HitBlueGoal = 1,
    HitOutOfBounds = 2,
    HitIntoBlueArea = 3,
    HitIntoRedArea = 4,
    HitBlueAgent = 5,
    HitRedAgent = 6
}

public class VolleyballEnvController : MonoBehaviour
{
    int ballSpawnSide;

    VolleyballSettings volleyballSettings;

    public VolleyballAgent blueAgent;
    public VolleyballAgent redAgent;

    public List<VolleyballAgent> AgentsList = new List<VolleyballAgent>();
    List<Renderer> RenderersList = new List<Renderer>();

    Rigidbody blueAgentRb;
    Rigidbody redAgentRb;

    public GameObject ball;
    Rigidbody ballRb;

    public GameObject blueGoal;
    public GameObject redGoal;

    Renderer blueGoalRenderer;

    Renderer redGoalRenderer;

    List<KeyValuePair<Team, int> > hitterHistory;

    // private int resetTimer;

    public int MaxEnvironmentSteps;

    void Start()
    {
        // Time.timeScale = 1f;
        // Used to control agent & ball starting positions
        blueAgentRb = blueAgent.GetComponent<Rigidbody>();
        redAgentRb = redAgent.GetComponent<Rigidbody>();
        ballRb = ball.GetComponent<Rigidbody>();

        // Starting ball spawn side
        // -1 = spawn blue side, 1 = spawn red side
        var spawnSideList = new List<int> { -1, 1 };
        ballSpawnSide = spawnSideList[Random.Range(0, 2)];

        // Render ground to visualise which agent scored
        blueGoalRenderer = blueGoal.GetComponent<Renderer>();
        redGoalRenderer = redGoal.GetComponent<Renderer>();
        RenderersList.Add(blueGoalRenderer);
        RenderersList.Add(redGoalRenderer);

        volleyballSettings = FindObjectOfType<VolleyballSettings>();
        hitterHistory = new List<KeyValuePair<Team, int>>();

        ResetScene();
    }

    /// <summary>
    /// Tracks which agent last had control of the ball
    /// </summary>
    public void UpdateHitterHistory(Team team, int playerId)
    {
        hitterHistory.Add(new KeyValuePair<Team, int>(team, playerId));
    }

    public KeyValuePair<Team, int> GetLastHistoryEvent() {
        return hitterHistory[hitterHistory.Count - 1];
    }

    /// <summary>
    /// Resolves scenarios when ball enters a trigger and assigns rewards.
    /// Example reward functions are shown below.
    /// To enable Self-Play: Set either Red or Blue Agent's Team ID to 1.
    /// </summary>
    public void ResolveEvent(Event triggerEvent)
    {
        KeyValuePair<Team, int> lastHitter = GetLastHistoryEvent();
        //Debug.Log("ResolveEvent: " + triggerEvent);
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
                HandleHitAgentEvent(redAgent, lastHitter);         
                break;

            case Event.HitBlueAgent:
                HandleHitAgentEvent(blueAgent, lastHitter);                
                break;

            case Event.HitOutOfBounds:
                if (lastHitter.Key == Team.Blue)
                {
                    // apply penalty to blue agent
                    blueAgent.AddReward(-0.5f);
                    //redAgent.AddReward(0.1f);
                }
                else if (lastHitter.Key == Team.Red)
                {
                    // apply penalty to red agent
                    redAgent.AddReward(-0.5f);
                    // blueAgent.AddReward(0.1f);
                }

                // end episode
                blueAgent.EndEpisode();
                redAgent.EndEpisode();
                ResetScene();
                break;

            case Event.HitBlueGoal:
                // blue wins
                blueAgent.AddReward(1f);
                redAgent.AddReward(-1f);

                // end episode
                blueAgent.EndEpisode();
                redAgent.EndEpisode();
                ResetScene();
                break;

            case Event.HitRedGoal:
                // red wins
                redAgent.AddReward(1f);
                blueAgent.AddReward(-1f);

                // end episode
                blueAgent.EndEpisode();
                redAgent.EndEpisode();
                ResetScene();
                break;

            case Event.HitIntoBlueArea:
                if (lastHitter.Key == Team.Red)
                {
                    redAgent.AddReward(0.5f);
                }
                break;

            case Event.HitIntoRedArea:
                if (lastHitter.Key == Team.Blue)
                {
                    blueAgent.AddReward(0.5f);
                }
                break;
        }
    }

    private void HandleHitAgentEvent(VolleyballAgent currentAgent, KeyValuePair<Team, int> lastHitter) {
        if (lastHitter.Key == currentAgent.teamId && lastHitter.Value == currentAgent.UUID) {
            // same player double toch
            currentAgent.AddReward(-0.1f);
        } else {
            // agent wins
            currentAgent.AddReward(0.1f);
        }
    }

    /// <summary>
    /// Called every step. Control max env steps.
    /// </summary>
    // void FixedUpdate()
    // {
    //     resetTimer += 1;
    //     if (resetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
    //     {
    //         blueAgent.EpisodeInterrupted();
    //         redAgent.EpisodeInterrupted();
    //         ResetScene();
    //     }
    // }

    /// <summary>
    /// Reset agent and ball spawn conditions.
    /// </summary>
    public void ResetScene()
    {
        // resetTimer = 0;

        // hitterHistory.Clear();

        foreach (var agent in AgentsList)
        {
            // randomise starting positions and rotations
            var randomPosX = Random.Range(-2f, 2f);
            var randomPosZ = Random.Range(-2f, 2f);
            var randomPosY = Random.Range(0.5f, 3.75f); // depends on jump height
            var randomRot = Random.Range(-45f, 45f);

            agent.transform.localPosition = new Vector3(randomPosX, 0.01f, randomPosZ);
            agent.transform.eulerAngles = new Vector3(0, randomRot, 0);

            agent.GetComponent<Rigidbody>().velocity = default(Vector3);
        }

        // reset ball to starting conditions
        ResetBall();
    }

    /// <summary>
    /// Reset ball spawn conditions
    /// </summary>
    void ResetBall()
    {
        var randomPosX = Random.Range(-2f, 2f);
        var randomPosZ = Random.Range(6f, 10f);
        var randomPosY = Random.Range(6f, 8f);

        // alternate ball spawn sàide
        // -1 = spawn blue side, 1 = spawn red side
        ballSpawnSide = -1 * ballSpawnSide;
        ball.transform.localPosition = new Vector3(randomPosX, randomPosY, ballSpawnSide * randomPosZ);

        ballRb.angularVelocity = Vector3.zero;
        ballRb.velocity = Vector3.zero;
    }
}

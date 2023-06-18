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
    HitRedAgent = 6,
    HitWall = 7
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

    private List<VolleyballAgent> hitterHistory = new List<VolleyballAgent>();

    // private int resetTimer;

    public int MaxEnvironmentSteps;

    void Start()
    {
        Time.timeScale = 0.5f;
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

        ResetScene();
    }

    /// <summary>
    /// Tracks which agent last had control of the ball
    /// </summary>
    public void AppendToHitterHistory(VolleyballAgent agent)
    {
        hitterHistory.Add(agent);
    }

    /// <summary>
    /// Resolves scenarios when ball enters a trigger and assigns rewards.
    /// Example reward functions are shown below.
    /// To enable Self-Play: Set either Red or Blue Agent's Team ID to 1.
    /// </summary>
    public void ResolveEvent(Event triggerEvent)
    {
        VolleyballAgent lastHitter = hitterHistory.Count > 0 ? hitterHistory[hitterHistory.Count - 1] : null;
        VolleyballAgent secondToLastHitter = hitterHistory.Count > 1 ? hitterHistory[hitterHistory.Count - 2] : null;
        //Debug.Log("ResolveEvent: " + triggerEvent);
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
            case Event.HitBlueAgent:
                if (secondToLastHitter != null && lastHitter.UUID == secondToLastHitter.UUID) {
                    // same player double toch
                    lastHitter.AddReward(-0.1f);
                    EndAllAgentsEpisode();
                    ResetScene();
                } else {
                    // agent wins
                    lastHitter.AddReward(0.1f);
                } 
                break;
            case Event.HitOutOfBounds:
                if (lastHitter != null && lastHitter.teamId == Team.Blue)
                {
                    // apply penalty to blue agent
                    blueAgent.AddReward(-0.5f);
                    //redAgent.AddReward(0.1f);
                }
                else if (lastHitter != null && lastHitter.teamId == Team.Red)
                {
                    // apply penalty to red agent
                    redAgent.AddReward(-0.5f);
                    // blueAgent.AddReward(0.1f);
                }

                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitBlueGoal:
                // blue wins
                blueAgent.AddReward(1f);
                redAgent.AddReward(-1f);
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitRedGoal:
                // red wins
                redAgent.AddReward(1f);
                blueAgent.AddReward(-1f);
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitIntoBlueArea:
                if (lastHitter != null && lastHitter.teamId == Team.Red)
                {
                    redAgent.AddReward(0.5f);
                }
                break;

            case Event.HitIntoRedArea:
                if (lastHitter != null && lastHitter.teamId == Team.Blue)
                {
                    blueAgent.AddReward(0.5f);
                }
                break;
            case Event.HitWall:
                if(lastHitter != null) {
                    lastHitter.AddReward(-1f);
                    EndAllAgentsEpisode();
                    ResetScene();
                }
                break;
        }
    }

    private void EndAllAgentsEpisode() {      
        foreach (var agent in AgentsList)
        {
            agent.EndEpisode();
        }
    }

    /// <summary>
    /// Reset agent and ball spawn conditions.
    /// </summary>
    public void ResetScene()
    {
        // resetTimer = 0;

        hitterHistory.Clear();

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

using System.Collections.Generic;
using System.Linq;
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
    HitWall = 7,
    EpisodeEnd = 8
}

public class VolleyballEnvController : MonoBehaviour
{
    int ballSpawnSide;

    VolleyballSettings volleyballSettings;

    public List<VolleyballAgent> AgentsList = new List<VolleyballAgent>();
    List<Renderer> RenderersList = new List<Renderer>();

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
        VolleyballAgent lastHitter = hitterHistory.Count > 0 ? hitterHistory[^1] : null;
        // VolleyballAgent secondToLastHitter = hitterHistory.Count > 1 ? hitterHistory[hitterHistory.Count - 2] : null;
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
            case Event.HitBlueAgent:
                if (IsDoubleToch()) {
                    lastHitter.AddReward(-0.1f);
                    EndAllAgentsEpisode();
                    ResetScene();
                } else {
                    int numberOfTeamPass = GetNumberOfTeamPass();
                    if (numberOfTeamPass > 0  && numberOfTeamPass <= 3) {
                        // team players do until 3 passes
                        lastHitter.AddReward(0.1f + numberOfTeamPass * 0.2f);
                    } else if(numberOfTeamPass > 3) {
                        // team players do MORE than 3 passes --> FAULT
                        lastHitter.AddReward(-(0.2f + numberOfTeamPass * 0.2f));
                        EndAllAgentsEpisode();
                        ResetScene();
                    } else {
                        // simple agent touch
                        lastHitter.AddReward(0.1f);
                    }
                }
                break;
            case Event.HitOutOfBounds:
                if (lastHitter != null)
                {
                    // apply penalty to agent
                    lastHitter.AddReward(-0.5f);
                }
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitBlueGoal:
                // blue wins
                if (lastHitter != null && lastHitter.teamId == Team.Blue)
                {
                    lastHitter.AddReward(1f);
                    applyRewardsToTeam(Team.Red, -1f);
                }
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitRedGoal:
                // red wins
                if (lastHitter != null && lastHitter.teamId == Team.Red)
                {
                    lastHitter.AddReward(1f);
                    applyRewardsToTeam(Team.Blue, -1f);
                }
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitIntoBlueArea:
                if (lastHitter != null && lastHitter.teamId == Team.Red)
                {
                    lastHitter.AddReward(0.2f);
                }
                break;

            case Event.HitIntoRedArea:
                if (lastHitter != null && lastHitter.teamId == Team.Blue)
                {
                    lastHitter.AddReward(0.2f);
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
            var randomPosY = 0f; // depends on jump height
            var randomRot = 0f;

            agent.transform.localPosition = new Vector3(randomPosX, randomPosY, randomPosZ);
            agent.transform.eulerAngles = new Vector3(0, randomRot, 0);


            agent.GetComponent<Rigidbody>().velocity = default(Vector3);
        }

        // reset ball to starting conditions
        ResetBall();
    }

    /// <summary>
    /// Reset ball spawn conditions
    /// </summary>
    private void ResetBall()
    {
        //var randomPosX = Random.Range(-2f, 2f);
        //var randomPosZ = Random.Range(6f, 10f);
        //var randomPosY = Random.Range(6f, 8f);

        // alternate ball spawn sàide
        // -1 = spawn blue side, 1 = spawn red side
        ballSpawnSide = -1 * ballSpawnSide;
        
        //Ball spawn position is hover one of the agents
        VolleyballAgent server = null;
        if (ballSpawnSide == 1){
            server = AgentsList[0];
        } else {
            server = AgentsList[1];
        }
        
        ball.transform.position = server.transform.position + new Vector3(0, 8f, 0);

        ballRb.angularVelocity = Vector3.zero;
        ballRb.velocity = Vector3.zero;
    }

    public IEnumerable<VolleyballAgent> GetOpponentAgents(Team teamId) {
        return AgentsList.Where(agent => agent.teamId != teamId);
    }

    public IEnumerable<VolleyballAgent> GetPartnertAgents(Team teamId) {
        return AgentsList.Where(agent => agent.teamId == teamId);
    }

    public List<VolleyballAgent> GetHitterHistory() {
        return hitterHistory;
    }

    private bool IsDoubleToch() {
        return 
            hitterHistory.Count > 1
            && hitterHistory[^1].UUID == hitterHistory[^2].UUID;
    }

    private void applyRewardsToTeam(Team teamId, float reward) {
        foreach (var allie in GetPartnertAgents(teamId)) {
            allie.AddReward(reward);
        }
    }

    private int GetNumberOfTeamPass() {
        int teamPass = -1;
        Team currentTeam = hitterHistory[^1].teamId;
        foreach (var agent in hitterHistory) {
            if (agent.teamId == currentTeam) {
                teamPass += 1;
            }
            else {
                break;
            }
        }
        return teamPass;
    }

}

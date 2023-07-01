using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Team
{
    Blue = 0,
    Red = 1,
    Default = 2
}

public static class TeamMap
{
    public static Dictionary<Team, string> teamMap = new Dictionary<Team, string>()
    {
        { Team.Blue, "blue" },
        { Team.Red, "red" },
        { Team.Default, "default" }
    };
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
    EpisodeEnd = 8,
    AgentsCollision = 9,
    DoubleTouch = 10
}

public class VolleyballEnvController : MonoBehaviour
{
    public float timeScale;
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

    private int resetTimer;
    public int MaxEnvironmentSteps;

    void Start()
    {
        // Used to control agent & ball starting positions
        ballRb = ball.GetComponent<Rigidbody>();

        // Starting ball spawn side
        // -1 = spawn blue side, 1 = spawn red side
        var spawnSideList = new List<int> { -1, 1 };
        ballSpawnSide = spawnSideList[Random.Range(0, 2)];

        volleyballSettings = FindObjectOfType<VolleyballSettings>();

        ResetScene();
    }

    /// <summary>
    /// Called every step. Control max env steps.
    /// </summary>
    void FixedUpdate()
    {
        resetTimer += 1;
        if (resetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            foreach (VolleyballAgent agent in AgentsList)
            {
                agent.EpisodeInterrupted();
            }
            ResetScene();
        }
    }

    /// <summary>
    /// Tracks which agent last had control of the ball
    /// </summary>
    public void AppendToHitterHistory(VolleyballAgent agent)
    {
        hitterHistory.Add(agent);
    }

    public void ResolveEvent(Event triggerEvent, VolleyballAgent triggerer, VolleyballAgent other){
        // switch (triggerEvent){
        //     case Event.AgentsCollision:
        //         triggerer.SetReward(-0.1f);
        //         other.SetReward(-0.1f);
        //         break;
        // }
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
            case Event.DoubleTouch:
                lastHitter.SetReward(-1f);
                EndAllAgentsEpisode();
                ResetScene();
                break;
            case Event.HitRedAgent:
            case Event.HitBlueAgent:
                if (IsDoubleTouch())
                {
                    lastHitter.SetReward(-1f);
                    EndAllAgentsEpisode();
                    ResetScene();
                }
                else
                {
                    int numberOfTeamTouches = GetNumberOfTeamTouches();
                    if (numberOfTeamTouches > 3)
                    {
                        // NOTE: Second to last hitter is the one who should have hit the ball in the other field, so he gets a negative reward
                        VolleyballAgent secondToLastHitter = hitterHistory[hitterHistory.Count - 2];
                        secondToLastHitter.SetReward(-1f);
                        EndAllAgentsEpisode();
                        ResetScene();
                    }
                    else
                    {
                        lastHitter.AddReward(0.1f + 0.1f * numberOfTeamTouches);
                    }
                }
                break;
            case Event.HitOutOfBounds:
                if (lastHitter != null)
                {
                    // apply penalty to agent
                    lastHitter.SetReward(-1.0f);
                }
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitBlueGoal:
                ApplyRewardToTeam(Team.Red, -1f);
                // Red took the goal, so it will be penalized
                // Blue will get a reward only if it did something
                // ApplyRewardToTeam(Team.Red, -0.5f);
                // if (PlayerOfTeamHitBall(Team.Blue))
                // {
                //     ApplyRewardToTeam(Team.Blue, 0.5f);
                // }
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitRedGoal:
                ApplyRewardToTeam(Team.Blue, -1f);
                // Blue took the goal, so it will be penalized
                // Red will get a reward only if it did something
                // ApplyRewardToTeam(Team.Blue, -0.5f);
                // if (PlayerOfTeamHitBall(Team.Red))
                // {
                //     ApplyRewardToTeam(Team.Red, 0.5f);
                // }
                EndAllAgentsEpisode();
                ResetScene();
                break;

            case Event.HitIntoBlueArea:
                // NOTE: we probably can remove the check on the team id here
                ApplyRewardToTeam(Team.Red, 1f);
                // if (lastHitter != null && lastHitter.teamId == Team.Red)
                // {
                //     lastHitter.AddReward(1);
                // }
                break;

            case Event.HitIntoRedArea:
                // NOTE: we probably can remove the check on the team id here
                ApplyRewardToTeam(Team.Blue, 1f);
                // if (lastHitter != null && lastHitter.teamId == Team.Blue)
                // {
                //     lastHitter.AddReward(1);
                // }
                break;
            case Event.HitWall:
                // if (lastHitter != null)
                // {
                //     lastHitter.SetReward(-1f);
                //     EndAllAgentsEpisode();
                //     ResetScene();
                // }
                break;
        }
    }

    private void EndAllAgentsEpisode()
    {
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
        resetTimer = 0;
        Time.timeScale = timeScale;
        hitterHistory.Clear();
        
        
        ballSpawnSide = -1 * ballSpawnSide;

        //Ball spawn position is hover one of the agents
        VolleyballAgent server = null;
        if (ballSpawnSide == 1)
        {
            // 0 and 2 must be on serving position   
            server = AgentsList[Random.Range(0, AgentsList.Count/2)];
        }
        else
        {
            server = AgentsList[Random.Range(AgentsList.Count/2, AgentsList.Count)];
        }

        foreach (var agent in AgentsList)
        {
            // randomise starting positions
            var randomPosX = Random.Range(-2f, 2f);
            var randomPosZ = Random.Range(-2f, 2f);

            agent.transform.localPosition = new Vector3(randomPosX, 0, randomPosZ);
            agent.transform.eulerAngles = new Vector3(0, 0, 0);

            agent.GetComponent<Rigidbody>().velocity = default(Vector3);
        }

        server.transform.localPosition = new Vector3(server.agentRot * Random.Range(-5f, -3f), 0, server.agentRot * Random.Range(-5f, -3f));
        server.transform.eulerAngles = new Vector3(0, 0, 0);
        ball.transform.position = server.transform.position + new Vector3(Random.Range(-1f, 1f), 8f, server.agentRot * Random.Range(0f, 2f));
        ballRb.angularVelocity = Vector3.zero;
        ballRb.velocity = Vector3.zero;

    }

    public IEnumerable<VolleyballAgent> GetAgents()
    {
        return AgentsList;
    }

    public IEnumerable<VolleyballAgent> GetAgentsInTeam(Team teamId)
    {
        return AgentsList.Where(agent => agent.teamId == teamId);
    }

    public IEnumerable<VolleyballAgent> GetAgentsNotInTeam(Team teamId)
    {
        return AgentsList.Where(agent => agent.teamId != teamId);
    }

    public List<VolleyballAgent> GetHitterHistory()
    {
        return hitterHistory;
    }

    public bool PlayerOfTeamHitBall(Team teamId)
    {
        return hitterHistory.Count > 0 && hitterHistory.Any(agent => agent.teamId == teamId);
    }

    public bool IsDoubleTouch()
    {
        return hitterHistory.Count > 1 && hitterHistory[^1].name == hitterHistory[^2].name;
    }

    private void ApplyRewardToTeam(Team teamId, float reward)
    {
        foreach (var agent in GetAgentsInTeam(teamId))
        {
            agent.SetReward(reward);
        }
    }

    private int GetNumberOfTeamTouches()
    {
        //This function is called after a touch, so we start from 1
        int teamTouches = 1;
        Team currentTeamId = hitterHistory[^1].teamId;

        if (hitterHistory.Count > 1 && currentTeamId == hitterHistory[^2].teamId)
        {
            teamTouches += 1;
        }

        if (hitterHistory.Count > 2 && currentTeamId == hitterHistory[^3].teamId)
        {
            teamTouches += 1;
        }

        return teamTouches;
    }
}

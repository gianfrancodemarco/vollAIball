using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssertDTO
{
    public string fact { get; set; }
}

public class KnowledgeBaseController : MonoBehaviour
{
    VolleyballEnvController envController;

    /// Keeps track of the current action
    /// When the player from a different team touches the ball, the action is incremented
    private int action = 0;
    private float episode = 1;
    private int number_of_team_pass = 0;

    void Start()
    {
        envController = FindObjectOfType<VolleyballEnvController>();

        StartCoroutine(ResetKnowledgeBase());
        StartCoroutine(AssertPlayers());
        StartCoroutine(AssertTeams());
    }

    public void ResolveEvent(Event triggerEvent)
    {
        // Logic for actions and touch management
        List<VolleyballAgent> HitterHistory = envController.GetHitterHistory();
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
                if (
                    HitterHistory.Count == 1
                    || (HitterHistory.Count >= 2 && HitterHistory[^2].teamId != Team.Red)
                )
                {
                    action++;
                }
                break;
            case Event.HitBlueAgent:
                if (
                    HitterHistory.Count == 1
                    || (HitterHistory.Count >= 2 && HitterHistory[^2].teamId != Team.Blue)
                )
                {
                    action++;
                }
                break;
        }

        if (HitterHistory.Count > 0) 
        {
            switch (triggerEvent) 
            {
                case Event.HitRedAgent:
                case Event.HitBlueAgent:
                    StartCoroutine(AssertTouchPlayerAtAction());
                    break;
                case Event.HitOutOfBounds:
                    StartCoroutine(AssertBaseAction("HitOutOfBounds"));
                    break;
                case Event.HitBlueGoal:
                case Event.HitRedGoal:
                    StartCoroutine(AssertBaseAction("HitGoal"));
                    break;
                case Event.HitIntoBlueArea:
                    StartCoroutine(AssertBaseAction("HitIntoBlueArea"));
                    break;
                case Event.HitIntoRedArea:
                    StartCoroutine(AssertBaseAction("HitIntoRedArea"));
                    break;
                case Event.HitWall:
                    StartCoroutine(AssertBaseAction("HitWall"));
                    break;
                case Event.EpisodeEnd:
                    episode+=0.25f;
                    break;
            }
        }
    }

    public IEnumerator ResetKnowledgeBase()
    {
        yield return new WaitForSeconds(0.010f);
        StartCoroutine(KnowledgeBaseClient.Instance.Reset());
    }

    private IEnumerator AssertTeams()
    {
        List<String> facts = new List<String>();
        facts.Add(KnowledgeBasePredicates.predicateMap["Team"].Replace("<team_name>", "red"));
        facts.Add(KnowledgeBasePredicates.predicateMap["Team"].Replace("<team_name>", "blue"));

        foreach (String fact in facts)
        {
            yield return new WaitForSeconds(0.010f);
            StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
        }
    }

    private IEnumerator AssertPlayers()
    {
        foreach (VolleyballAgent player in envController.AgentsList)
        {
            string fact = KnowledgeBasePredicates.predicateMap["Player"]
                .Replace("<player_name>", player.UUID.ToString());
                
            yield return new WaitForSeconds(0.010f);
            StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));

            fact = KnowledgeBasePredicates.predicateMap["PlaysInTeam"]
                .Replace("<player_name>", player.UUID.ToString())
                .Replace("<team_name>", TeamMap.teamMap[player.teamId]);
            yield return new WaitForSeconds(0.010f);
            StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
        }
    }

    private IEnumerator AssertTouchPlayerAtAction()
    {
        List<VolleyballAgent> HitterHistory = envController.GetHitterHistory();
        VolleyballAgent player = HitterHistory[^1];

        if (HitterHistory.Count >= 2 && HitterHistory[^2].teamId == player.teamId) {
            number_of_team_pass++;
        } else {
            number_of_team_pass = 0;
        }

        string fact = KnowledgeBasePredicates.predicateMap["TouchPlayerAtAction"]
            .Replace("<player_name>", player.UUID.ToString())
            .Replace("<episode_name>", episode.ToString())
            .Replace("<action_name>", action.ToString())
            .Replace("<number_of_team_pass>", number_of_team_pass.ToString());
        yield return new WaitForSeconds(0.010f);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
    }
    
    private IEnumerator AssertBaseAction(string assertKey) {
        List<string> AllowedAssertKeys = KnowledgeBasePredicates.predicateMap.Keys.ToList();
        if(!AllowedAssertKeys.Contains(assertKey)) 
        {
            throw new KeyNotFoundException("Predicate not allowed: " + assertKey);
        }
        
        List<VolleyballAgent> HitterHistory = envController.GetHitterHistory();
        VolleyballAgent player = HitterHistory[^1];
        string fact = KnowledgeBasePredicates.predicateMap[assertKey]
            .Replace("<player_name>", player.UUID.ToString())
            .Replace("<episode_name>", episode.ToString())
            .Replace("<action_name>", action.ToString());
        yield return new WaitForSeconds(0.010f);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
    }

}

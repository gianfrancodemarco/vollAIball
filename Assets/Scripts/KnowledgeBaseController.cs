using System;
using System.Collections;
using System.Collections.Generic;
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
                    HitterHistory.Count == 0
                    || HitterHistory[HitterHistory.Count - 1].teamId != Team.Blue
                )
                {
                    action++;
                }
                break;
            case Event.HitBlueAgent:
                if (
                    HitterHistory.Count == 0
                    || HitterHistory[HitterHistory.Count - 1].teamId != Team.Red
                )
                {
                    action++;
                }
                break;
        }

        switch (triggerEvent)
        {
            case Event.HitRedAgent:
            case Event.HitBlueAgent:
                StartCoroutine(AssertTouchPlayerAtAction());
                break;
            case Event.HitOutOfBounds:
                break;
            case Event.HitBlueGoal:
                break;
            case Event.HitRedGoal:
                break;
            case Event.HitIntoBlueArea:
                break;
            case Event.HitIntoRedArea:
                break;
            case Event.HitWall:
                break;
            case Event.EpisodeEnd:
                break;
        }
    }

    public IEnumerator ResetKnowledgeBase()
    {
        yield return new WaitForSeconds(0.010f);
        StartCoroutine(KnowledgeBaseClient.Instance.Reset());
    }

    private IEnumerator AssertTeams()
    {
        // Create list of strings
        // Assert each string
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

    public IEnumerator AssertTouchPlayerAtAction()
    {
        List<VolleyballAgent> HitterHistory = envController.GetHitterHistory();
        VolleyballAgent player = HitterHistory[HitterHistory.Count - 1];
        string fact = KnowledgeBasePredicates.predicateMap["TouchPlayerAtAction"]
            .Replace("<player_name>", player.UUID.ToString())
            .Replace("<action_name>", action.ToString());
        yield return new WaitForSeconds(0.010f);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
    }
}

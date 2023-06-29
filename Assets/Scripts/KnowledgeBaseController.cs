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
    private int point = 0;
    private int action = 0;
    private int touch = 0;

    void Start()
    {
        envController = FindObjectOfType<VolleyballEnvController>();

        StartCoroutine(ResetKnowledgeBase());
        StartCoroutine(AssertTeams());
        StartCoroutine(KnowledgeBaseClient.Instance.GetMatchNarrative());
        StartCoroutine(AssertPlayers());
        StartCoroutine(KnowledgeBaseClient.Instance.GetMatchNarrative());
        StartCoroutine(PollCommentary());
    }

    private IEnumerator PollCommentary()
    {
        while (true)
        {
            StartCoroutine(KnowledgeBaseClient.Instance.GetMatchNarrative());
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ResolveEvent(Event triggerEvent)
    {
        List<VolleyballAgent> HitterHistory = envController.GetHitterHistory();

        // We ignore everything if the ball touches the floor without touching an agent
        if (HitterHistory.Count > 0)
        {
            // Logic for point, action and touch management
            switch (triggerEvent)
            {
                case Event.DoubleTouch:
                case Event.HitBlueGoal:
                case Event.HitRedGoal:
                case Event.HitOutOfBounds:
                    point++;
                    action = 0;
                    touch = 0;
                    if (triggerEvent == Event.DoubleTouch){
                        StartCoroutine(AssertBaseAction("DoubleTouch"));
                    }
                    break;
                case Event.HitRedAgent:
                case Event.HitBlueAgent:
                    if (HitterHistory.Count >= 2 && HitterHistory[^2].teamId != HitterHistory[^1].teamId)
                    {
                        action++;
                        touch = 0;
                    }
                    touch++;
                    break;
            }

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
                    StartCoroutine(AssertBaseAction("HitBlueGoal"));
                    break;
                case Event.HitRedGoal:
                    StartCoroutine(AssertBaseAction("HitRedGoal"));
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
                .Replace("<player_name>", player.name);

            yield return new WaitForSeconds(0.010f);
            StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));

            fact = KnowledgeBasePredicates.predicateMap["PlaysInTeam"]
                .Replace("<player_name>", player.name)
                .Replace("<team_name>", TeamMap.teamMap[player.teamId]);
            yield return new WaitForSeconds(0.010f);
            StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
        }
    }

    private IEnumerator AssertTouchPlayerAtAction()
    {
        List<VolleyballAgent> HitterHistory = envController.GetHitterHistory();
        VolleyballAgent player = HitterHistory[^1];

        string fact = KnowledgeBasePredicates.predicateMap["TouchPlayerAtAction"]
            .Replace("<player_name>", player.name)
            .Replace("<point>", point.ToString())
            .Replace("<action>", action.ToString())
            .Replace("<touch>", touch.ToString());
        yield return new WaitForSeconds(0.010f);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
    }

    private IEnumerator AssertBaseAction(string assertKey)
    {
        List<string> AllowedAssertKeys = KnowledgeBasePredicates.predicateMap.Keys.ToList();
        if (!AllowedAssertKeys.Contains(assertKey))
        {
            throw new KeyNotFoundException("Predicate not allowed: " + assertKey);
        }

        List<VolleyballAgent> HitterHistory = envController.GetHitterHistory();
        VolleyballAgent player = HitterHistory[^1];
        string fact = KnowledgeBasePredicates.predicateMap[assertKey]
            .Replace("<player_name>", player.name)
            .Replace("<point>", point.ToString())
            .Replace("<action>", action.ToString());
        yield return new WaitForSeconds(0.010f);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact(fact));
    }

}

using UnityEngine;

public class AssertDTO
{
    public string fact { get; set; }
}

public class KnowledgeBaseController : MonoBehaviour
{

    void Awake() {
        StartCoroutine(KnowledgeBaseClient.Instance.Reset());
        System.Threading.Thread.Sleep(5);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact("player(redAgent1)"));
        System.Threading.Thread.Sleep(5);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact("player(redAgent2)"));
        System.Threading.Thread.Sleep(5);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact("player(blueAgent1)"));
        System.Threading.Thread.Sleep(5);
        StartCoroutine(KnowledgeBaseClient.Instance.SaveFact("player(blueAgent2)"));
        System.Threading.Thread.Sleep(5);
        StartCoroutine(KnowledgeBaseClient.Instance.QueryFact("player(X)"));   
    }

    public void ResolveEvent(Event triggerEvent)
    {
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
                break;
            case Event.HitBlueAgent:
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
}
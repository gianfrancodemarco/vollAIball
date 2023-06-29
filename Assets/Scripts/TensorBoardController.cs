
using UnityEngine;
using Unity.MLAgents;

public class TensorBoardController : MonoBehaviour {

    private StatsRecorder statsRecorder;

    void Awake()
    {
        statsRecorder = Academy.Instance.StatsRecorder;
    }

    public void ResolveEvent(Event triggerEvent) {
        
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
                statsRecorder.Add("Statistics/numberOfHitRedAgent", 1, StatAggregationMethod.Sum);
                break;
            case Event.HitBlueAgent:
                statsRecorder.Add("Statistics/numberOfHitBlueAgent", 1, StatAggregationMethod.Sum);
                break;
            case Event.HitOutOfBounds:
                statsRecorder.Add("Statistics/numberOfHitOutOfBounds", 1, StatAggregationMethod.Sum);
                break;
            case Event.HitBlueGoal:
                statsRecorder.Add("Statistics/numberOfHitBlueGoal", 1, StatAggregationMethod.Sum);
                break;
            case Event.HitRedGoal:
                statsRecorder.Add("Statistics/numberOfHitRedGoal", 1, StatAggregationMethod.Sum);
                break;
            case Event.HitIntoBlueArea:
                statsRecorder.Add("Statistics/numberOfHitIntoBlueArea", 1, StatAggregationMethod.Sum);
                break;
            case Event.HitIntoRedArea:
                statsRecorder.Add("Statistics/numberOfHitIntoRedArea", 1, StatAggregationMethod.Sum);
                break;
            case Event.HitWall:
                statsRecorder.Add("Statistics/numberOfHitWall", 1, StatAggregationMethod.Sum);
                break;
            case Event.EpisodeEnd:
                //Each field will add the same episode 4 times
                statsRecorder.Add("Statistics/numberOfEndEpisode", 1f, StatAggregationMethod.Sum);
                break;
            case Event.AgentsCollision:
                statsRecorder.Add("Statistics/numberOfAgentsCollision", 1, StatAggregationMethod.Sum);
                break;
        }
    }

}
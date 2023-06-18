
using UnityEngine;
using Unity.MLAgents;

public class TensorBoardController : MonoBehaviour {

    private StatsRecorder statsRecorder;
    
    private int numberOfHitRedAgent = 0;
    private int numberOfHitBlueAgent = 0;
    private int numberOfHitOutOfBounds = 0;
    private int numberOfHitBlueGoal = 0;
    private int numberOfHitRedGoal = 0;
    private int numberOfHitIntoBlueArea = 0;
    private int numberOfHitIntoRedArea = 0;
    private int numberOfHitWall = 0;

    void Awake()
    {
        statsRecorder = Academy.Instance.StatsRecorder;
    }

    public void ResolveEvent(Event triggerEvent) {
        
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
                numberOfHitRedAgent += 1;
                statsRecorder.Add("Statistics/numberOfHitRedAgent", numberOfHitRedAgent, StatAggregationMethod.MostRecent);
                break;
            case Event.HitBlueAgent:
                numberOfHitBlueAgent += 1;
                statsRecorder.Add("Statistics/numberOfHitBlueAgent", numberOfHitBlueAgent, StatAggregationMethod.MostRecent);
                break;
            case Event.HitOutOfBounds:
                numberOfHitOutOfBounds += 1;
                statsRecorder.Add("Statistics/numberOfHitOutOfBounds", numberOfHitOutOfBounds, StatAggregationMethod.MostRecent);
                break;
            case Event.HitBlueGoal:
                numberOfHitBlueGoal += 1;
                statsRecorder.Add("Statistics/numberOfHitBlueGoal", numberOfHitBlueGoal, StatAggregationMethod.MostRecent);
                break;
            case Event.HitRedGoal:
                numberOfHitRedGoal += 1;
                statsRecorder.Add("Statistics/numberOfHitRedGoal", numberOfHitRedGoal, StatAggregationMethod.MostRecent);
                break;
            case Event.HitIntoBlueArea:
                numberOfHitIntoBlueArea += 1;
                statsRecorder.Add("Statistics/numberOfHitIntoBlueArea", numberOfHitIntoBlueArea, StatAggregationMethod.MostRecent);
                break;
            case Event.HitIntoRedArea:
                numberOfHitIntoRedArea += 1;
                statsRecorder.Add("Statistics/numberOfHitIntoRedArea", numberOfHitIntoRedArea, StatAggregationMethod.MostRecent);
                break;
            case Event.HitWall:
                numberOfHitWall += 1;
                statsRecorder.Add("Statistics/numberOfHitWall", numberOfHitWall, StatAggregationMethod.MostRecent);
                break;
        }
    }

}
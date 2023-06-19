
using UnityEngine;
using Unity.MLAgents;

public class TensorBoardController : MonoBehaviour {

    private StatsRecorder statsRecorder;
    private int numberOfFields;

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
        numberOfFields = FindObjectsOfType<VolleyballEnvController>().Length;
        Debug.Log("Number of fields: " + numberOfFields);
    }

    public void ResolveEvent(Event triggerEvent) {
        
        switch (triggerEvent)
        {
            case Event.HitRedAgent:
                numberOfHitRedAgent += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitRedAgent", numberOfHitRedAgent, StatAggregationMethod.Sum);
                break;
            case Event.HitBlueAgent:
                numberOfHitBlueAgent += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitBlueAgent", numberOfHitBlueAgent, StatAggregationMethod.Sum);
                break;
            case Event.HitOutOfBounds:
                numberOfHitOutOfBounds += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitOutOfBounds", numberOfHitOutOfBounds, StatAggregationMethod.Sum);
                break;
            case Event.HitBlueGoal:
                numberOfHitBlueGoal += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitBlueGoal", numberOfHitBlueGoal, StatAggregationMethod.Sum);
                break;
            case Event.HitRedGoal:
                numberOfHitRedGoal += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitRedGoal", numberOfHitRedGoal, StatAggregationMethod.Sum);
                break;
            case Event.HitIntoBlueArea:
                numberOfHitIntoBlueArea += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitIntoBlueArea", numberOfHitIntoBlueArea, StatAggregationMethod.Sum);
                break;
            case Event.HitIntoRedArea:
                numberOfHitIntoRedArea += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitIntoRedArea", numberOfHitIntoRedArea, StatAggregationMethod.Sum);
                break;
            case Event.HitWall:
                numberOfHitWall += (1 / numberOfFields);
                statsRecorder.Add("Statistics/numberOfHitWall", numberOfHitWall, StatAggregationMethod.Sum);
                break;
        }
    }

}
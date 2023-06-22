using UnityEngine;

public class VolleyballController : MonoBehaviour
{
    [HideInInspector]
    public VolleyballEnvController envController;
    [HideInInspector]
    public TensorBoardController tensorBoardController;
    [HideInInspector]
    public KnowledgeBaseController knowledgeBaseController;

    public GameObject redGoal;
    public GameObject blueGoal;
    private Collider redGoalCollider;
    private Collider blueGoalCollider;

    void Start()
    {
        envController = GetComponentInParent<VolleyballEnvController>();

        redGoalCollider = redGoal.GetComponent<Collider>();
        blueGoalCollider = blueGoal.GetComponent<Collider>();
        tensorBoardController = FindObjectOfType<TensorBoardController>();
        knowledgeBaseController = FindObjectOfType<KnowledgeBaseController>();
    }

    void OnCollisionEnter(Collision collision)
    {   

        if (collision.gameObject.CompareTag("blueAgent"))
        {
            // ball hit blue goal (red side court)
            envController.AppendToHitterHistory(collision.gameObject.GetComponent<VolleyballAgent>());
            TrackTensorBoardEvent(Event.HitBlueAgent);
            envController.ResolveEvent(Event.HitBlueAgent);
            TrackTensorBoardEvent(Event.HitBlueAgent);
            TrackInKnowledgeBase(Event.HitBlueAgent);
        }
        else if (collision.gameObject.CompareTag("redAgent"))
        {
            // ball hit blue goal (red side court)            
            envController.AppendToHitterHistory(collision.gameObject.GetComponent<VolleyballAgent>());
            TrackTensorBoardEvent(Event.HitRedAgent);
            envController.ResolveEvent(Event.HitRedAgent);
            TrackInKnowledgeBase(Event.HitRedAgent);    
        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            TrackTensorBoardEvent(Event.HitWall);
            envController.ResolveEvent(Event.HitWall);
            TrackInKnowledgeBase(Event.HitWall);
        }
    }

    /// <summary>
    /// Detects whether the ball lands in the blue, red, or out of bounds area
    /// Works by detecting the when the ball enters the trigger collider
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("boundary"))
        {
            // ball went out of bounds
            TrackTensorBoardEvent(Event.HitOutOfBounds);
            envController.ResolveEvent(Event.HitOutOfBounds);
            TrackInKnowledgeBase(Event.HitOutOfBounds);
        }
        else if (other.gameObject.CompareTag("blueBoundary"))
        {
            // ball hit into blue side
            TrackTensorBoardEvent(Event.HitIntoBlueArea);
            envController.ResolveEvent(Event.HitIntoBlueArea);
            TrackInKnowledgeBase(Event.HitIntoBlueArea);
        }
        else if (other.gameObject.CompareTag("redBoundary"))
        {
            // ball hit into red side
            TrackTensorBoardEvent(Event.HitIntoRedArea);
            envController.ResolveEvent(Event.HitIntoRedArea);
            TrackInKnowledgeBase(Event.HitIntoRedArea);
        }
        else if (other.gameObject.CompareTag("redGoal"))
        {
            // ball hit red goal (blue side court)
            TrackTensorBoardEvent(Event.HitRedGoal);
            envController.ResolveEvent(Event.HitRedGoal);
            TrackInKnowledgeBase(Event.HitRedGoal);
        }
        else if (other.gameObject.CompareTag("blueGoal"))
        {
            // ball hit blue goal (red side court)
            TrackTensorBoardEvent(Event.HitBlueGoal);
            envController.ResolveEvent(Event.HitBlueGoal);
            TrackInKnowledgeBase(Event.HitBlueGoal);
        }
    }

    /// <summary>
    /// Tracks the events in TensorBoard
    /// This MUST be called before resetting the environment
    /// </summary>
    private void TrackTensorBoardEvent(Event triggerEvent)
    {
        if (envController.GetHitterHistory().Count > 0) {
            tensorBoardController.ResolveEvent(triggerEvent);
        }
    }

    private void TrackInKnowledgeBase(Event triggerEvent)
    {
        if (knowledgeBaseController != null){
            knowledgeBaseController.ResolveEvent(triggerEvent);
        }
    }
}
using UnityEngine;

public class VolleyballController : MonoBehaviour
{
    [HideInInspector]
    public VolleyballEnvController envController;
    [HideInInspector]
    public TensorBoardController tensorBoardController;

    public GameObject redGoal;
    public GameObject blueGoal;
    Collider redGoalCollider;
    Collider blueGoalCollider;

    void Start()
    {
        envController = GetComponentInParent<VolleyballEnvController>();
        redGoalCollider = redGoal.GetComponent<Collider>();
        blueGoalCollider = blueGoal.GetComponent<Collider>();
        tensorBoardController = GetComponentInParent<TensorBoardController>();
    }

    void OnCollisionEnter(Collision collision)
    {   
        if (collision.gameObject.CompareTag("blueAgent"))
        {
            // ball hit blue goal (red side court)
            envController.AppendToHitterHistory(collision.gameObject.GetComponent<VolleyballAgent>());
            envController.ResolveEvent(Event.HitBlueAgent);
            tensorBoardController.ResolveEvent(Event.HitBlueAgent);

        }
        else if (collision.gameObject.CompareTag("redAgent"))
        {
            // ball hit blue goal (red side court)            
            envController.AppendToHitterHistory(collision.gameObject.GetComponent<VolleyballAgent>());
            envController.ResolveEvent(Event.HitRedAgent);
            tensorBoardController.ResolveEvent(Event.HitRedAgent);
        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            envController.ResolveEvent(Event.HitWall);
            tensorBoardController.ResolveEvent(Event.HitWall);
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
            envController.ResolveEvent(Event.HitOutOfBounds);
            tensorBoardController.ResolveEvent(Event.HitOutOfBounds);
        }
        else if (other.gameObject.CompareTag("blueBoundary"))
        {
            // ball hit into blue side
            envController.ResolveEvent(Event.HitIntoBlueArea);
            tensorBoardController.ResolveEvent(Event.HitIntoBlueArea);
        }
        else if (other.gameObject.CompareTag("redBoundary"))
        {
            // ball hit into red side
            envController.ResolveEvent(Event.HitIntoRedArea);
            tensorBoardController.ResolveEvent(Event.HitIntoRedArea);
        }
        else if (other.gameObject.CompareTag("redGoal"))
        {
            // ball hit red goal (blue side court)
            envController.ResolveEvent(Event.HitRedGoal);
            tensorBoardController.ResolveEvent(Event.HitRedGoal);
        }
        else if (other.gameObject.CompareTag("blueGoal"))
        {
            // ball hit blue goal (red side court)
            envController.ResolveEvent(Event.HitBlueGoal);
            tensorBoardController.ResolveEvent(Event.HitBlueGoal);
        }
    }
}

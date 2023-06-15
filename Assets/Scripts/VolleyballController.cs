using UnityEngine;

public class VolleyballController : MonoBehaviour
{
    [HideInInspector]
    public VolleyballEnvController envController;

    public GameObject redGoal;
    public GameObject blueGoal;
    Collider redGoalCollider;
    Collider blueGoalCollider;

    void Start()
    {
        envController = GetComponentInParent<VolleyballEnvController>();
        redGoalCollider = redGoal.GetComponent<Collider>();
        blueGoalCollider = blueGoal.GetComponent<Collider>();
    }

    /// <summary>
    /// Detects whether the ball lands in the blue, red, or out of bounds area
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("boundary"))
        {
            // ball went out of bounds
            envController.ResolveEvent(Event.HitOutOfBounds);
        }
        else if (other.gameObject.CompareTag("blueBoundary"))
        {
            // ball hit into blue side
            envController.ResolveEvent(Event.HitIntoBlueArea);
        }
        else if (other.gameObject.CompareTag("redBoundary"))
        {
            // ball hit into red side
            envController.ResolveEvent(Event.HitIntoRedArea);
        }
        else if (other.gameObject.CompareTag("redGoal"))
        {
            // ball hit red goal (blue side court)
            envController.ResolveEvent(Event.HitRedGoal);
        }
        else if (other.gameObject.CompareTag("blueGoal"))
        {
            // ball hit blue goal (red side court)
            envController.ResolveEvent(Event.HitBlueGoal);
        }

    }


}
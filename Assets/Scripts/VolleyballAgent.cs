using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class VolleyballAgent : Agent
{
    public GameObject area;
    public Team teamId;
    public GameObject ball;
    public Collider[] hitGroundColliders = new Collider[3];

    private Rigidbody agentRb;
    private Rigidbody ballRb;
    private BehaviorParameters behaviorParameters;
    private VolleyballSettings volleyballSettings;
    private VolleyballEnvController envController;
    private TensorBoardController tensorBoardController; 
    private KnowledgeBaseController knowledgeBaseController;

    private Vector3 jumpTargetPos;
    private Vector3 jumpStartingPos;
    private EnvironmentParameters resetParams;
    private bool isGrounded;
    public float agentRot;

    void Start()
    {
        isGrounded = false;
        envController = area.GetComponent<VolleyballEnvController>();
        tensorBoardController = FindObjectOfType<TensorBoardController>();
        knowledgeBaseController = FindObjectOfType<KnowledgeBaseController>();
    }

    public new void EndEpisode()
    {
        base.EndEpisode();
        tensorBoardController.ResolveEvent(Event.EpisodeEnd);

        if (knowledgeBaseController != null){
            knowledgeBaseController.ResolveEvent(Event.EpisodeEnd);
        }
        
    }

    public override void Initialize()
    {
        volleyballSettings = FindObjectOfType<VolleyballSettings>();
        behaviorParameters = gameObject.GetComponent<BehaviorParameters>();

        agentRb = GetComponent<Rigidbody>();
        ballRb = ball.GetComponent<Rigidbody>();

        // for symmetry between player side
        if (teamId == Team.Blue)
        {
            agentRot = -1;
        }
        else
        {
            agentRot = 1;
        }

        resetParams = Academy.Instance.EnvironmentParameters;
    }

    /// <summary>
    /// Moves  a rigidbody towards a position smoothly.
    /// </summary>
    /// <param name="targetPos">Target position.</param>
    /// <param name="rb">The rigidbody to be moved.</param>
    /// <param name="targetVel">The velocity to target during the
    ///  motion.</param>
    /// <param name="maxVel">The maximum velocity posible.</param>
    private void MoveTowards(Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
    {
        var moveToPos = targetPos - rb.worldCenterOfMass;
        var velocityTarget = Time.fixedDeltaTime * targetVel * moveToPos;
        if (float.IsNaN(velocityTarget.x) == false)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.ToLower().Contains("agent"))
        {
            envController.ResolveEvent(
                Event.AgentsCollision,
                this,
                c.gameObject.GetComponent<VolleyballAgent>()
            );
            tensorBoardController.ResolveEvent(Event.AgentsCollision);
        }
    }

    /// <summary>
    /// Called when agent collides with something
    /// </summary>
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("walkableSurface"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision c)
    {
        if (c.gameObject.CompareTag("walkableSurface"))
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// Starts the jump sequence
    /// </summary>
    public void Jump(Vector3 dirToGo)
    {
        jumpTargetPos =
            new Vector3(
                agentRb.position.x,
                jumpStartingPos.y + volleyballSettings.agentJumpHeight,
                agentRb.position.z
            )
            + agentRot * dirToGo;

        MoveTowards(
            jumpTargetPos,
            agentRb,
            volleyballSettings.agentJumpVelocity,
            volleyballSettings.agentJumpVelocityMaxChange
        );
    }

    /// <summary>
    /// Resolves the agent movement
    /// </summary>
    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var rotateDirAction = act[1];
        var dirToGoSideAction = act[2];
        var jumpAction = act[3];

        float baseVelocity = 1f;
        if (!isGrounded)
        {
            baseVelocity = 0.5f;
        }

        if (dirToGoForwardAction == 1)
        { //forward
            dirToGo = baseVelocity * transform.forward * 1f;
        }
        else if (dirToGoForwardAction == 2)
        { //backward
            dirToGo = baseVelocity * transform.forward * volleyballSettings.speedReductionFactor * -1f;
        }

        if (dirToGoSideAction == 1)
        { //right
            dirToGo = baseVelocity * transform.right * volleyballSettings.speedReductionFactor * -1f;
        }
        else if (dirToGoSideAction == 2)
        { //left
            dirToGo = baseVelocity * transform.right * volleyballSettings.speedReductionFactor;
        }

        if (rotateDirAction == 1)
        { //rotate right
            rotateDir = transform.up * -1f;
        }
        else if (rotateDirAction == 2)
        { //rotate left
            rotateDir = transform.up * 1f;
        }

        if (jumpAction == 1)
        {
            if (isGrounded)
            {
                Jump(dirToGo);
            }
        }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        agentRb.AddForce(
            agentRot * dirToGo * volleyballSettings.agentRunSpeed,
            ForceMode.VelocityChange
        );
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent rotation (1 float)
        sensor.AddObservation(this.transform.rotation.y);

        // Vector from agent to ball (direction to ball) (3 floats)
        Vector3 toBall = new Vector3((ballRb.transform.position.x - this.transform.position.x)*agentRot, 
        (ballRb.transform.position.y - this.transform.position.y),
        (ballRb.transform.position.z - this.transform.position.z)*agentRot);

        sensor.AddObservation(toBall.normalized);

        // Distance from the ball (1 float)
        sensor.AddObservation(toBall.magnitude);

        // Agent velocity (3 floats)
        sensor.AddObservation(agentRb.velocity);

        // Ball velocity (3 floats)
        sensor.AddObservation(ballRb.velocity.y);
        sensor.AddObservation(ballRb.velocity.z*agentRot);
        sensor.AddObservation(ballRb.velocity.x*agentRot);

        foreach (VolleyballAgent ally in envController.GetAgentsInTeam(teamId)){
            if (ally != this){
                // Vector from agent to ally (direction to ally) (3 floats)
                Vector3 toAlly = new Vector3((ally.transform.position.x - this.transform.position.x)*ally.agentRot, 
                (ally.transform.position.y - this.transform.position.y),
                (ally.transform.position.z - this.transform.position.z)*ally.agentRot);

                sensor.AddObservation(toAlly.normalized);

                // Ally velocity (3 floats)
                sensor.AddObservation(ally.agentRb.velocity);
            }
        }
    }

    // For human controller
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            // rotate right
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            // forward
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // rotate left
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            // backward
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // move left
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            // move right
            discreteActionsOut[2] = 2;
        }
        discreteActionsOut[3] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}

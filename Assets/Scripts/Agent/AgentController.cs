using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController : Agent
{
    // Movement parameters
    public float moveSpeed = 5f; // Forward movement speed
    public float rotationSpeed = 200f; // Rotation speed
    public string[] detectableTags;
    /*
    // Parameters for penalty for standing still
    public float stationaryThreshold = 2f; // Minimum time threshold for penalty
    public float penaltyAmount = -0.01f; // Penalty amount
    public float movementThreshold = 0.01f; // Minimum movement threshold
    private Vector3 lastPosition; // Last position of the agent
    private float timeStationary = 0f; // Time the agent stood still
    */
    // Connection to the spawning system
    public ObjectSpawner objectSpawner;

    // Connection to the animator and rigidbody
    private Animator animator;
    private Rigidbody rb;

    private int positiveObjectsCollected = 0; // Number of positive objects collected
    private int negativeObjectsCollected = 0;
    private int WallObjectsCollisions = 0;

    private int currentStepCount = 0;
    public override void OnEpisodeBegin()
    {
        // Reset the counters
        positiveObjectsCollected = 0;
        negativeObjectsCollected = 0;
        WallObjectsCollisions = 0;
        currentStepCount = 0;

        // Reset the map
        objectSpawner.ResetEnvironment();

        // Reset agent position
        transform.SetPositionAndRotation(new Vector3(0f, 0f, 0f), Quaternion.identity);
        /*
        lastPosition = transform.position; // Initialize last position
        timeStationary = 0f; // Initialize time stationary
        */
    }
    public override void Initialize()
    {
        if (objectSpawner == null)
        {
            Debug.LogError("No ObjectSpawner attached to the Agent!");
        }
        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody attached to the Agent!");
        }

        transform.SetPositionAndRotation(new Vector3(0f, 0f, 0f), Quaternion.identity); // Reset agent position
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //// Add agent position (x, z)
        //Vector3 position = transform.position;
        //sensor.AddObservation(position.x); // Add X
        //sensor.AddObservation(position.z); // Add Z

        //// Add agent rotation angle
        //float rotation = transform.rotation.eulerAngles.y; // Rotation angle in Y
        //sensor.AddObservation(rotation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        // Forward movement
        if (move != 0 && rb != null)
        {
            //animator.SetBool("isWalking", true);
            Vector3 newPosition = rb.position + transform.forward * move * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            //animator.SetBool("isWalking", false);
        }
        // Rotation
        if (rotate != 0)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, rotate * rotationSpeed * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        /*
        // Negative reward for standing still
        if (IsAgentStationary())
        {
            timeStationary += Time.deltaTime;

            // If the threshold time has passed, add penalty
            if (timeStationary > stationaryThreshold)
            {
                AddReward(penaltyAmount);
                Debug.Log($"penalty added: {penaltyAmount}");

                timeStationary = 0f; // Reset the counter
                //Debug.Log("Stationary too long! Penalty applied.");
            }
        }// Check if the agent is standing still
        else
        {
            timeStationary = 0f; // Reset the counter if the agent moved
        }
        // Update the last position
        lastPosition = transform.position;
        */
        //Check if all positive objects have been collected
        if (objectSpawner.AllPositiveObjectsCollected())
        {
            AddReward(2.0f); // Bonus for success
            EndEpisode();    // End the episode
        }

        //AddReward(-0.00001f); // Time penalty

        /*
        currentStepCount++;
        if (currentStepCount >= 5000)
        {
            SetReward(-1.0f);
            EndEpisode();
        }
        */
    }
    /*
    private bool IsAgentStationary()
    {
        // Check if the distance between the current and last position is less than the movement threshold
        return Vector3.Distance(transform.position, lastPosition) < movementThreshold;
    }
    */
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : 0f;
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PositiveObject"))
        {
            AddReward(2.0f);
            Destroy(collision.gameObject);
            positiveObjectsCollected++;
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("NegativeObject"))
        {
            AddReward(-1.0f);
            Destroy(collision.gameObject);
            negativeObjectsCollected++;   
            EndEpisode();
            //if(negativeObjectsCollected >= 3)
            //    EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f);
            WallObjectsCollisions++;
            if (WallObjectsCollisions >= 10)
                EndEpisode();
        }
    }
}

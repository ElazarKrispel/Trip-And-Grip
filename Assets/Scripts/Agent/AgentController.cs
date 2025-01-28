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

    int wereToMove = 1;
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
        sensor.AddObservation(transform.localPosition); // Agent's position
        sensor.AddObservation(positiveObjectsCollected); // Positive objects collected
        sensor.AddObservation(negativeObjectsCollected); // Negative objects collected

        // Add distance to nearest positive and negative objects
        GameObject nearestPositive = FindNearestObject("PositiveObject");
        GameObject nearestNegative = FindNearestObject("NegativeObject");

        if (nearestPositive != null)
        {
            sensor.AddObservation(Vector3.Distance(transform.position, nearestPositive.transform.position));
        }
        else
        {
            sensor.AddObservation(0f); // No positive objects
        }

        if (nearestNegative != null)
        {
            sensor.AddObservation(Vector3.Distance(transform.position, nearestNegative.transform.position));
        }
        else
        {
            sensor.AddObservation(0f); // No negative objects
        }
    }

    private GameObject FindNearestObject(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = obj;
            }
        }
        return nearest;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        // Forward movement
        if (move != 0 && rb != null)
        {
            //animator.SetBool("isWalking", true);
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f) && hit.collider.CompareTag("Wall"))
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, wereToMove*90f, 0f));
                wereToMove *= -1;
            }
            else
            {
                Vector3 newPosition = rb.position + transform.forward * move * moveSpeed * Time.deltaTime;
                rb.MovePosition(newPosition);
            }
            if (move > 0)
            {
                AddReward(0.1f);
            }
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

        if (objectSpawner.AllNegativeObjectsCollected())
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
            AddReward(5.0f);
            Destroy(collision.gameObject);
            positiveObjectsCollected++;
            //EndEpisode();
        }
        else if (collision.gameObject.CompareTag("NegativeObject"))
        {
            AddReward(-9f);
            Destroy(collision.gameObject);
            negativeObjectsCollected++;   
            //EndEpisode();
            //if(negativeObjectsCollected >= 3)
            //    EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-3f);
            WallObjectsCollisions++;

            //if (WallObjectsCollisions >= 10)
                //EndEpisode();
        }
    }
}

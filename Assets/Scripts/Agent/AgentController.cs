using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BasicAgent : Agent
{
    public float moveSpeed = 5f; // Forward movement speed
    public float rotationSpeed = 200f; // Rotation speed
    private Animator animator;
    public override void Initialize()
    {
        //Debug.Log("Initialized");
        transform.localPosition = new Vector3(-3.972f, 0f, -4.292f);
        animator = GetComponent<Animator>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Debug.Log("CollectObservations called");
        //sensor.AddObservation(transform.position.x); // דוגמה לתצפית
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        //Debug.Log($"Received Move: {move}, Rotate: {rotate}");

        // תנועה קדימה
        if (move != 0)
        {
            animator.SetBool("isWalking", true);
            transform.position += transform.forward * move * moveSpeed * Time.deltaTime;
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // סיבוב
        if (rotate != 0)
        {
            transform.Rotate(Vector3.up, rotate * rotationSpeed * Time.deltaTime);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //Debug.Log("Heuristic called");
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : 0f;
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

        //Debug.Log($"Move: {continuousActions[0]}, Rotate: {continuousActions[1]}");
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PositiveObject"))
        {
            AddReward(1.0f);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("NegativeObject"))
        {
            AddReward(-1.0f);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f);
        }
    }
}


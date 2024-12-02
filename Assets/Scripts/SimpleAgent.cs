using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SimpleAgent : Agent
{
    public float speed = 5f;

    // Function that performs actions when the agent receives actions from the engine
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 move = new Vector3(moveX, 0, moveZ);
        transform.Translate(move * speed * Time.deltaTime);
    }

    // Function to collect data from the environment
    public override void CollectObservations(VectorSensor sensor)
    {
        // Example: Agent's position on the X and Z axes
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
    }

    // Reset the agent at the beginning of each episode
    public override void OnEpisodeBegin()
    {
        // Initialize the agent's position
        transform.localPosition = new Vector3(-3.972f, 4f, -4.292f);
    }
}

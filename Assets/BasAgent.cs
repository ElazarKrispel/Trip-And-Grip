using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BasAgent : Agent
{
    public override void Initialize()
    {
        Debug.Log("Initialized");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("CollectObservations called");
        //sensor.AddObservation(transform.position.x); // דוגמה לתצפית
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        Debug.Log($"Received Move: {move}, Rotate: {rotate}");

        // תנועה קדימה
        if (move != 0)
        {
            transform.position += transform.forward * move * Time.deltaTime;
        }

        // סיבוב
        if (rotate != 0)
        {
            transform.Rotate(Vector3.up, rotate * 200f * Time.deltaTime);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Heuristic called");
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : 0f;
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

        Debug.Log($"Move: {continuousActions[0]}, Rotate: {continuousActions[1]}");
    }

}
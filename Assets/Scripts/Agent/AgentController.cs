using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AgentController : Agent
{
    public float moveSpeed = 5f; // Forward movement speed
    public float rotationSpeed = 200f; // Rotation speed

    public string[] detectableTags;

   // private Animator animator;
    private Rigidbody rb;

    public override void Initialize()
    {
         Debug.Log("Initialized");
        transform.localPosition = new Vector3(-3.972f, 5f, -4.292f);
       // animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody attached to the Agent!");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Debug.Log("CollectObservations called");
        // Raycasting לאיסוף תצפיות
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f))
        {
            // בדוק אם התג של האובייקט תואם לאחד התגים שהגדרנו
            foreach (string tag in detectableTags)
            {
                if (hit.collider.CompareTag(tag))
                {
                    Debug.Log($"Detected {tag}: {hit.collider.gameObject.name}");
                }
            }
            // Debug.Log($"Saw: {hit.collider.gameObject.name}");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float rotate = actions.ContinuousActions[1];

        // Debug.Log($"Received Move: {move}, Rotate: {rotate}");

        // תנועה קדימה
        if (move != 0 && rb != null)
        {
            // animator.SetBool("isWalking", true);
            Vector3 newPosition = rb.position + transform.forward * move * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        // else
        //{
        // animator.SetBool("isWalking", false);
        //  }

        // סיבוב
        if (rotate != 0)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, rotate * rotationSpeed * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);

        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
         Debug.Log("Heuristic called");
        var continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : 0f;
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

        // Debug.Log($"Move: {continuousActions[0]}, Rotate: {continuousActions[1]}");
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

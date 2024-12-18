using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;

public class AgentController : Agent
{
    public float moveSpeed = 5f; // Forward movement speed
    public float rotationSpeed = 200f; // Rotation speed

    private Rigidbody rb;
    private Animator animator;

    public override void Initialize()
    {

        rb = GetComponent<Rigidbody>();// רכיב Rigidbody כדי לשלוט על התנועה הפיזיקלית של הסוכן
        animator = GetComponent<Animator>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // התצפיות מגיעות מ-Ray Perception Sensor, אין צורך להוסיף כאן
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // קבלת פעולות מהרשת הנוירונית
        float move = actions.ContinuousActions[0]; // תנועה קדימה/אחורה
        float rotate = actions.ContinuousActions[1]; // סיבוב

        // תנועה קדימה
        
        if (move != 0)
        {
            //rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
            rb.MovePosition(rb.position + transform.forward * move * Time.deltaTime * moveSpeed);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // סיבוב
        transform.Rotate(Vector3.up, rotate * Time.deltaTime * rotationSpeed);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // שליטה ידנית עבור בדיקות
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : 0f;
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

    }

    public void OnCollisionEnter(Collision collision)
    {
        // תגמול/ענישה לפי סוג ההתנגשות
        if (collision.gameObject.CompareTag("PositiveObject"))
        {
            AddReward(1.0f); // תגמול על איסוף חפץ חיובי
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("NegativeObject"))
        {
            AddReward(-1.0f); // ענישה על פגיעה בחפץ שלילי
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f); // ענישה על פגיעה בקיר
        }
    }
}

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

    public override void OnEpisodeBegin()
    {
        rb = GetComponent<Rigidbody>();// ���� Rigidbody ��� ����� �� ������ ��������� �� �����
        animator = GetComponent<Animator>();
        // Initialize the agent's position
        transform.localPosition = new Vector3(-3.972f, 4f, -4.292f);
    }

    //public override void Initialize()
    //{

    //    rb = GetComponent<Rigidbody>();// ���� Rigidbody ��� ����� �� ������ ��������� �� �����
    //    animator = GetComponent<Animator>();
    //}

    public override void CollectObservations(VectorSensor sensor)
    {
        // ������� ������ �-Ray Perception Sensor, ��� ���� ������ ���
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // ���� ������ ����� ���������
        float move = actions.ContinuousActions[0]; // ����� �����/�����
        float rotate = actions.ContinuousActions[1]; // �����

        // ����� �����
        
        if (move != 0)
        {
            animator.SetBool("isWalking", true);
            rb.MovePosition(rb.position + transform.forward * move * Time.deltaTime * moveSpeed);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // �����
        transform.Rotate(Vector3.up, rotate * Time.deltaTime * rotationSpeed);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // ����� ����� ���� ������
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetKey(KeyCode.W) ? 1f : 0f;
        continuousActions[1] = Input.GetKey(KeyCode.A) ? -1f : Input.GetKey(KeyCode.D) ? 1f : 0f;

    }

    public void OnCollisionEnter(Collision collision)
    {
        // �����/����� ��� ��� ��������
        if (collision.gameObject.CompareTag("PositiveObject"))
        {
            AddReward(1.0f); // ����� �� ����� ��� �����
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("NegativeObject"))
        {
            AddReward(-1.0f); // ����� �� ����� ���� �����
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f); // ����� �� ����� ����
        }
    }
}

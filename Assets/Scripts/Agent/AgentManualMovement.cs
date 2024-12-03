using UnityEngine;

public class ManualMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");  // Get input from A and D keys
        float moveZ = Input.GetAxis("Vertical");    // Get input from W and S keys

        Vector3 move = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;
        transform.Translate(move);
    }
}

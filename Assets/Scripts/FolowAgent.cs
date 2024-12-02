using UnityEngine;

public class FolowAgent : MonoBehaviour
{
    public Transform Agent;
    public Vector3 offset = new Vector3(0, 1, -0.005f);
    // Update is called once per frame
    void Update()
    {
        transform.position = Agent.position + offset;
    }
}
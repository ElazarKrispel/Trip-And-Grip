using UnityEngine;

/// <summary>
/// ������ ����� ������ ������ ����� ���� ����� ��� ����� �� ���, ����� ����� �����, �������� ����������.
/// </summary>
public class CameraController : MonoBehaviour
{
    public Transform target; // ������ ����� ������ ������ �����
    public float cameraDistance = 2f; // ���� ������ ������
    public float cameraHeight = 2f; // ���� ������ ��� �����
    public float cameraSensitivity = 100f; // ������ ������
    public float zoomSpeed = 2f; // ������ ����
    public float minCameraDistance = 2f; // ���� �������
    public float maxCameraDistance = 10f; // ���� �������
    public float minVerticalAngle = -20f; // ����� ����� ��������
    public float maxVerticalAngle = 60f; // ����� ����� ��������

    private float currentVerticalRotation = 0f;
    private Vector3 velocity = Vector3.zero; // ����� ������ ��� ����� �� ����� ����� ����

    /// <summary>
    /// ����� ���� ������ �� �� ��� ����������. ����� ������ ����� ������ ������ ����� ������ �����.
    /// </summary>
    void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();
        HandleCameraRotation();
        HandleVerticalRotation();
        HandleCameraPosition();
    }

    /// <summary>
    /// ���� ���� �� ������ ������� ����� �����, ���� ������ ���� ���� ���.
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        // ��� ������ ����� ���� �������
        cameraDistance -= scroll * zoomSpeed;
        // ����� ���� ���� ��� ���� ������� ��������
        cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
    }

    /// <summary>
    /// ����� ������ ���� ����� �� ��� �-Y ����� ����� ����� �������.
    /// </summary>
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        // ����� ������ ���� ����� �� ���� ����� (Y) ����� ������ ����� �������
        transform.RotateAround(target.position, Vector3.up, mouseX);
    }

    /// <summary>
    /// ����� ���� �� ������ ����� ������ �����, ���� ������ ����� ������.
    /// </summary>
    void HandleVerticalRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        currentVerticalRotation -= mouseY;
        // ����� ������ ������ ��� �������� ��������� �������
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalAngle, maxVerticalAngle);
    }

    /// <summary>
    /// ����� ������ �� ������ ����� ������ �����, ��� ����� �-Raycast ��� ����� ������ �� ����� �����.
    /// </summary>
    void HandleCameraPosition()
    {
        // ����� ������ ����� �� ������ ������ �����, ���� ������
        Vector3 direction = Quaternion.Euler(currentVerticalRotation, transform.eulerAngles.y, 0) * Vector3.back * cameraDistance;
        Vector3 desiredPosition = target.position + Vector3.up * cameraHeight + direction;

        // ����� �-Raycast ��� ����� �� �� ��������� ��� ������ �����, ������ �����
        RaycastHit hit;
        if (Physics.Linecast(target.position + Vector3.up * cameraHeight, desiredPosition, out hit))
        {
            // �� �� ������� �� �������, ����� ������ ����� ������ ��������
            transform.position = hit.point - hit.normal * 0.5f;
        }
        else
        {
            // �� ��� �������, ������ ���� ������ ����� ����� ����
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.2f);
        }

        // ������ ������ ���� �� ��� �����
        transform.LookAt(target.position + Vector3.up * (cameraHeight / 2));
    }
}

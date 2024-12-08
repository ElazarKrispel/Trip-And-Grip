using UnityEngine;

/// <summary>
/// סקריפט שמטפל בתנועת המצלמה ועוקב אחרי הדמות תוך שמירה על זום, סיבוב אופקי ואנכי, והתחשבות בהתנגשויות.
/// </summary>
public class CameraController : MonoBehaviour
{
    public Transform target; // הייחוס לדמות שאחריה המצלמה עוקבת
    public float cameraDistance = 2f; // מרחק המצלמה מהדמות
    public float cameraHeight = 2f; // גובה המצלמה מעל הדמות
    public float cameraSensitivity = 100f; // רגישות המצלמה
    public float zoomSpeed = 2f; // מהירות הזום
    public float minCameraDistance = 2f; // מרחק מינימלי
    public float maxCameraDistance = 10f; // מרחק מקסימלי
    public float minVerticalAngle = -20f; // זווית אנכית מינימלית
    public float maxVerticalAngle = 60f; // זווית אנכית מקסימלית

    private float currentVerticalRotation = 0f;
    private Vector3 velocity = Vector3.zero; // משתנה מהירות כדי לשמור על תנועת מצלמה חלקה

    /// <summary>
    /// מתבצע לאחר העדכון של כל שאר האובייקטים. אחראי לעדכון מיקום וזווית המצלמה בהתאם לתנועת הדמות.
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
    /// מטפל בזום של המצלמה באמצעות גלגלת העכבר, כולל הגבלות מרחק וזום חלק.
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        // זום המצלמה בהתאם לקלט מהגלגלת
        cameraDistance -= scroll * zoomSpeed;
        // הגבלת מרחק הזום בין מרחק מינימלי ומקסימלי
        cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
    }

    /// <summary>
    /// סיבוב המצלמה סביב הדמות על ציר ה-Y בעזרת תנועת העכבר האופקית.
    /// </summary>
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        // סיבוב המצלמה סביב הדמות על הציר האנכי (Y) בהתאם לתנועת העכבר האופקית
        transform.RotateAround(target.position, Vector3.up, mouseX);
    }

    /// <summary>
    /// סיבוב אנכי של המצלמה בהתאם לתנועת העכבר, כולל הגבלות זווית אנכיות.
    /// </summary>
    void HandleVerticalRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        currentVerticalRotation -= mouseY;
        // הגבלת הזווית האנכית בין המינימום והמקסימום שהוגדרו
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalAngle, maxVerticalAngle);
    }

    /// <summary>
    /// קביעת המיקום של המצלמה בהתאם למיקום הדמות, תוך שימוש ב-Raycast כדי למנוע חפיפות עם עצמים אחרים.
    /// </summary>
    void HandleCameraPosition()
    {
        // חישוב המיקום הרצוי של המצלמה בהתחשב במרחק, גובה וזווית
        Vector3 direction = Quaternion.Euler(currentVerticalRotation, transform.eulerAngles.y, 0) * Vector3.back * cameraDistance;
        Vector3 desiredPosition = target.position + Vector3.up * cameraHeight + direction;

        // שימוש ב-Raycast כדי לבדוק אם יש אובייקטים בין המצלמה לדמות, ולמנוע חפיפה
        RaycastHit hit;
        if (Physics.Linecast(target.position + Vector3.up * cameraHeight, desiredPosition, out hit))
        {
            // אם יש התנגשות עם אובייקט, מיקום המצלמה יוגבל לנקודת ההתנגשות
            transform.position = hit.point - hit.normal * 0.5f;
        }
        else
        {
            // אם אין התנגשות, המצלמה תנוע למיקום הרצוי בצורה חלקה
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.2f);
        }

        // המצלמה מכוונת תמיד אל עבר הדמות
        transform.LookAt(target.position + Vector3.up * (cameraHeight / 2));
    }
}

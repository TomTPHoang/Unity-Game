using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [Tooltip("Controls how fast the camera will move with the mouse.")]
    [SerializeField] private float sensitivity = 2f;
    [Tooltip("Controls how much the camera will continue to move after the mouse input stops.")]
    [SerializeField] private float drag = 3f;

    private Vector2 mouseDir;
    private Vector2 smoothing;
    private Vector2 result;
    private Transform character;

    public bool LookEnabled { get; set; } = true; // Enables/disables the ability to move the camera with the mouse.

    void Awake()
    {
        character = transform.parent;
        LookEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (LookEnabled == true)
        {

            mouseDir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouseDir *= sensitivity;
            smoothing = Vector2.Lerp(smoothing, mouseDir, 1 / drag);
            result += smoothing;
            result.y = Mathf.Clamp(result.y, -70, 70);

            character.rotation = Quaternion.AngleAxis(result.x, character.up);
            transform.localRotation = Quaternion.AngleAxis(-result.y, Vector3.right);
        }
    }
}

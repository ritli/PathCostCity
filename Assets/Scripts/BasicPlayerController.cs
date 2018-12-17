using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    private Vector3 oldMousePos;
    new Transform camera;

    void Start()
    {
        camera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseDelta = oldMousePos - Input.mousePosition;

        camera.localRotation = camera.localRotation * Quaternion.Euler(Input.GetAxis("Mouse Y") * -15, 0, 0);
        camera.localRotation= Quaternion.Euler(Mathf.Clamp(camera.localRotation.eulerAngles.x, 15, 35), camera.localRotation.y, camera.localRotation.z);

        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * 180);

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));

        transform.Translate(input * Time.deltaTime * 5, Space.Self);

        oldMousePos = Input.mousePosition;
    }
}

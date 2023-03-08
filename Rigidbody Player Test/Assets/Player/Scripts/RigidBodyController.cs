using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyController : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;

    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private Transform LookAt;
    [SerializeField] private Rigidbody PlayerBody;
    [Space]
    [SerializeField] private float Speed;
    [SerializeField] private float xSensitivity;
    [SerializeField] private float ySensitivity;
    [SerializeField] private float Jumpforce;
    [SerializeField] private Vector3 Zero = new Vector3(0f, 0f, 0f);
    private void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Cursor.lockState = CursorLockMode.Locked;
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * Speed;
        PlayerBody.velocity = new Vector3(MoveVector.x, PlayerBody.velocity.y, MoveVector.z);

        //keep rotation relative to camera
        if (PlayerMovementInput != Zero)
        {
            Mathf.Lerp(transform.localEulerAngles.y, PlayerCamera.localEulerAngles.y, 10f);
            //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, PlayerCamera.localEulerAngles.y, transform.localEulerAngles.z);
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerBody.AddForce(Vector3.up * Jumpforce, ForceMode.Impulse);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Range(0.1f, 1f)] public float hoveringHeight;
    [Range(5f, 15f)] public float movementSpeed;
    [Range(50f, 150f)] public float rotationSpeed;

    private float _rotateY;
    private float _movementZ;

    private void Awake()
    {
        // Create an Action that binds to LMB
        var action = new InputAction(binding: "*/{primaryAction}");

        // Call Fire(); on LMB
        action.performed += _ => Fire();

        // Start listening for control changes
        action.Enable();
    }

    private void Start()
    {
        // initialize variables
        hoveringHeight = 0.5f;
        movementSpeed = 10f;
        rotationSpeed = 100f;
    }

    private void OnMove(InputValue movementValue)
    {
        // retrieve player input and store it in variables
        Vector2 movementVector = movementValue.Get<Vector2>();
        _rotateY = movementVector.x;
        _movementZ = movementVector.y;
    }

    private void Fire()
    {
        // get the starting point and direction of the raycast
        Vector3 barrelPosition = transform.GetChild(2).position;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 1000;

        // setup Ray
        RaycastHit hit;

        // create a visible raycast to show where the player is shooting from and to
        Debug.DrawRay(barrelPosition, forward, Color.white, 1);
        
        // cast the ray
        if (Physics.Raycast(barrelPosition, forward, out hit, forward.magnitude))
        {
            
            // check if the ray has hit a paintable object
            if (hit.collider.gameObject.CompareTag("Paintable"))
            {
                Debug.Log(hit.transform.gameObject);
                Destroy(hit.transform.gameObject);

            }
        }
    }

    private void Update()
    {
        // setup Ray
        Ray landingRay = new Ray(transform.position, Vector3.down);
        RaycastHit target;

        // cast Ray
        if (Physics.Raycast(landingRay, out target))
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.white, 1);
            // check if Ray is hitting a Plane
            if (target.collider.CompareTag("Environment"))
            {
                // rotate the player to line up with the target normal
                transform.rotation = Quaternion.FromToRotation(transform.up, target.normal) * transform.rotation;

                // make sure the hovering height is always the same relative to the ground
                var currentPos = transform.position;
                transform.position = Vector3.Lerp(transform.position, new Vector3(currentPos.x, target.point.y + hoveringHeight, currentPos.z), Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        // calculate the player's framerate-independent movement and rotation speed
        Vector3 forwardMovement = transform.forward * _movementZ * movementSpeed * Time.deltaTime;
        float rotation = _rotateY * rotationSpeed * Time.deltaTime;

        // apply calculated movement and rotation speed to the player's transform
        transform.position += forwardMovement;
        transform.Rotate(0f, rotation, 0f);
    }
}
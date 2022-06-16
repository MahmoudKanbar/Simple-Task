using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform earth;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private BezierCurve jumpingCurve;

    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float lerpSpeed;

    private Vector3 velocity;
    private float radius;

    private void Awake()
    {
        var correctUp = (transform.position - earth.position).normalized;
        radius = Vector3.Dot(correctUp, transform.position);
    }

    private void Update()
    {
        // get input
        var movementPressed = Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.A);
        var lookAroundPressed = Input.GetKey(KeyCode.LeftAlt);
        var sprintPressed = Input.GetKey(KeyCode.LeftShift);
        var jumpPressed = Input.GetKeyDown(KeyCode.Space);
        
        var movementDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movementDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) movementDirection -= transform.forward;
        if (Input.GetKey(KeyCode.D)) movementDirection += transform.right;
        if (Input.GetKey(KeyCode.A)) movementDirection -= transform.right;
        movementDirection.Normalize();

        // handle player jumping
        if (jumpPressed) StartCoroutine(JumpCoroutine());
        if (isJumping) return;

        // fix player up
        var correctUp = (transform.position - earth.position).normalized;
        var targetRotation = Quaternion.FromToRotation(transform.up, correctUp) * transform.rotation;
        transform.rotation = targetRotation;

        // rotate the player using the camera
        if (movementPressed && !lookAroundPressed)
        {
            targetRotation = cameraController.transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lerpSpeed);
        }

        // rotate the player using movement direction
        targetRotation = Quaternion.FromToRotation(transform.forward, movementDirection) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * lerpSpeed);

        // applying sprint factor
        var factor = 0.75f;
        if (sprintPressed) factor = 1.0f;

        // update velocity and move the player
        if (movementPressed) velocity += transform.forward * acceleration * factor * Time.deltaTime;
        else velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * lerpSpeed);

        if (velocity.sqrMagnitude >= Mathf.Pow(speed * factor, 2)) velocity = transform.forward * speed * factor;

        var nextPosition = transform.position + velocity * Time.deltaTime;
        transform.position = Vector3.Slerp(transform.position, nextPosition, Time.deltaTime * lerpSpeed);

        // keeping the player on the ground
        var length = Vector3.Dot(correctUp, transform.position);
        if (length > radius) transform.position -= correctUp * (length - radius);
        if (length < radius) transform.position += correctUp * (radius - length);

        // animation stuff
        animator.SetFloat("Velocity", velocity.magnitude / speed);
    }

    private bool isJumping;
    private IEnumerator JumpCoroutine()
    {
        var time = 0.0f;
        var duration = jumpingCurve.GetLength() / 10;
        jumpingCurve.transform.position = transform.position;
        jumpingCurve.transform.rotation = transform.rotation;

        isJumping = true;
        animator.Play("Jump");
        while (time < duration)
        {
            transform.position = jumpingCurve.GetPoint(time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        isJumping = false;
        animator.Play("Movement");
    }
}

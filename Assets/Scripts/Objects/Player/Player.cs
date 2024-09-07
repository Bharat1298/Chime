using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public float HitPoints = 100;
    public GameObject TurretPrefab;

    private readonly float MovementSpeed = 5f;
    private readonly float RotationSpeed = 500f;
    private bool Placeable = false;
    public Transform Destination;
    public Vector3 movementDirection;

    private Vector3 rollDir;
    private float rollSpeed;

    private int count;

    private enum State
    {
        Normal,
        Dodging,
    }

    public Rigidbody rigidBody;

    private State state;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Normal;
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        movementDirection = new();
        switch (state){
            case State.Normal:
                // Forwards
                if (Input.GetKey(KeyCode.W))
                {
                    movementDirection.z += MovementSpeed;
                }
                // Backwards
                if (Input.GetKey(KeyCode.S))
                {
                    movementDirection.z -= MovementSpeed;
                }
                // Left
                if (Input.GetKey(KeyCode.A))
                {
                    movementDirection.x -= MovementSpeed;
                }
                // Right
                if (Input.GetKey(KeyCode.D))
                {
                    movementDirection.x += MovementSpeed;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    this.gameObject.SetActive(false);
                    transform.position = Destination.position;
                    this.gameObject.SetActive(true);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    TryPlaceTurret();
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    rollDir = movementDirection;
                    rollSpeed = 1.5f;
                    state = State.Dodging;
                    count = 0;
                }
                break;
            case State.Dodging:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                if (movementDirection != Vector3.zero)
                {
                    transform.Translate(movementDirection * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        Quaternion.LookRotation(movementDirection, Vector3.up),
                        RotationSpeed * Time.deltaTime);
                }
                break;
            case State.Dodging:
                if (count < 3){
                    rigidBody.AddForce(rollDir.x, 0, rollDir.z, ForceMode.Impulse);
                    count++;
                }
                if(count == 3)
                {
                    state = State.Normal;
                    count = 0;
                }
                break;
        }
    }

    /*public MemoryOrbDestination GetClosedMemoryOrb()
    {

        return ClosedOrb;
    }*/
    private void TryPlaceTurret()
    {
        if (Placeable)
        {
            Instantiate(TurretPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Placeable"))
        {
            Placeable = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Placeable"))
        {
            Placeable = false;
        }
    }

    // Basic implementation for taking damage, can modify later
    public void TakeDamage(float damage)
    {
        HitPoints -= damage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private float power = 15;
    [SerializeField]
    private float torque = 180;

    [SerializeField]
    private Vector2 movementVector;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    public void Move(Vector2 movementInput)
    {
        this.movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward*power*Time.deltaTime);
        transform.Rotate(Vector3.up*movementVector.x*torque*Time.deltaTime,Space.Self);
    }
}

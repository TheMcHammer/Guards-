using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public event System.Action OnReachEndOfLevel;
    
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed =8;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity; 

    Rigidbody rigidPlayerbody;
    bool disabled;

    void Start()
    {
    	rigidPlayerbody = GetComponent<Rigidbody> ();
    	Guard.OnGuardHasSpottedPlayer += Disable;
    }

    void Update()
    {
    	Vector3 inputDirection = Vector3.zero;
    	if (!disabled)
    	{
    		inputDirection = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
    	}

    	float inputMagnitude = inputDirection.magnitude;
    	float targetAngle = Mathf.Atan2 (inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
    	
    	angle = Mathf.LerpAngle (angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
    	smoothInputMagnitude = Mathf.SmoothDamp (smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
    	
    	velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void OnTriggerEnter(Collider hitCollider)
    {
    	if (hitCollider.tag == "Finish")
    	{
    		if (OnReachEndOfLevel != null)
    		{
    			OnReachEndOfLevel ();
    		}
    	}
    }

    void Disable()
    {
    	disabled = true;
    }

    void FixedUpdate()
    {
    	rigidPlayerbody.MoveRotation (Quaternion.Euler (Vector3.up * angle));
    	rigidPlayerbody.MovePosition (rigidPlayerbody.position + velocity * Time.deltaTime);
    }

    void OnDestroy()
    {
    	Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}

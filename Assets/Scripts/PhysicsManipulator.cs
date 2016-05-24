using UnityEngine;
using MoCap;

/// <summary>
/// Component for moving a physical object by clicking and moving it.
/// When clicked, the script will try to maintain the relative position of the rigid body using forces applied to its centre.
/// </summary>
///
public class PhysicsManipulator : MonoBehaviour
{
	[Tooltip("Name of the device to control the physics manipulator")]
	public string deviceName  = "";

	[Tooltip("Channel name of the device to control the physics manipulator")]
	public string channelName = "";

	[Tooltip("Force per mass unit of the object to apply when moving")]
	public float  maxForce = 100;


	void Start()
	{
		inputFire = new InputDeviceHandler(deviceName, channelName);
	}


	void Update()
	{
		if (inputFire.GetButtonDown())
		{
			// trigger pulled: is there any rigid body in front?
			RaycastHit hit;
			Ray ray = new Ray(transform.position, transform.forward);
			bool hitSomething = Physics.Raycast(ray, out hit);
			if (hitSomething && (hit.rigidbody != null))
			{
				// Yes: remember rigid body and its relative position.
				// This relative position is what the script will try to maintain while moving the object
				activeBody         = hit.rigidbody;
				activeBodyPosition = transform.InverseTransformPoint(activeBody.transform.position);
				// make target object weightless
				activeBody.useGravity = false;
			}
		}
		else if (inputFire.GetButtonUp())
		{
			// fire button released
			if (activeBody != null)
			{
				// trigger released holding a rigid body: turn gravity back on and cease control
				activeBody.useGravity = true;
				activeBody            = null;
				activeBodyPosition    = Vector3.zero;
			}
		}

		// moving a rigid body: apply the right force to get that body to the new target position
		if (activeBody != null)
		{
			// Don't use: activeBody.MovePosition(this.transform.TransformPoint(activeBodyPosition));
			Vector3 targetPos = transform.TransformPoint(activeBodyPosition); // target position in world coordinates
			Vector3 force     = targetPos - activeBody.position; // how to get to target position
			force -= activeBody.velocity * 0.05f;                // apply damping to avoid resonance
			force *= maxForce * activeBody.mass;                 // scale force by mass to treat every object equally
			activeBody.AddForce(force);
		}
	}


	private InputDeviceHandler inputFire;
	private Rigidbody          activeBody;
	private Vector3            activeBodyPosition;
}

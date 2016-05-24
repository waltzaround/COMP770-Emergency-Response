using UnityEngine;

public class ObjectMover : MonoBehaviour 
{
	public float maxTranslationSpeed = 1.0f;

	public float maxRotationSpeed    = 45.0f;

	public float lerpFactor          = 0.5f;

	public Space motionSpace         = Space.Self;
	


	void Start()
	{
		vecT     = new Vector3(); vecR     = new Vector3();
		vecGoalT = new Vector3(); vecGoalR = new Vector3();
	}


	void Update () 
	{
		// gather input values
		vecGoalT.x = (Input.GetKey(KeyCode.A) ? -1 : 0) + 
		             (Input.GetKey(KeyCode.D) ? +1 : 0);
		vecGoalT.y = (Input.GetKey(KeyCode.C) ? -1 : 0) + 
		             (Input.GetKey(KeyCode.E) ? +1 : 0);
		vecGoalT.z = (Input.GetKey(KeyCode.S) ? -1 : 0) + 
		             (Input.GetKey(KeyCode.W) ? +1 : 0);

		vecGoalR.x = (Input.GetKey(KeyCode.UpArrow)    ? -1 : 0) + 
		             (Input.GetKey(KeyCode.DownArrow)  ? +1 : 0);
		vecGoalR.y = (Input.GetKey(KeyCode.LeftArrow)  ? -1 : 0) + 
		             (Input.GetKey(KeyCode.RightArrow) ? +1 : 0);

		// interpolate
		vecT = Vector3.Lerp(vecT, vecGoalT, lerpFactor);
		vecR = Vector3.Lerp(vecR, vecGoalR, lerpFactor);
		//Debug.Log("T: " + vecT.ToString() + ", R: " + vecR.ToString());

		// some precalculations
		float fT = maxTranslationSpeed * Time.deltaTime;
		float fR = maxRotationSpeed    * Time.deltaTime;

		// rotate up/down 
		transform.Rotate(vecR.x * fR, 0, 0, motionSpace);
		// rotate left/right (always absolute)
		transform.Rotate(0, vecR.y * fR, 0, Space.World);
		// translate
		transform.Translate(vecT.x * fT, vecT.y * fT, vecT.z * fT, motionSpace);
	}


	private Vector3 vecGoalT, vecGoalR; // goal values for translation/rotation
	private Vector3 vecT, vecR;         // current values for translation/rotation
}

using UnityEngine;
using MoCap;

/// <summary>
/// Script for changing the size of the game object based on the input from a VR input device,
/// e.g., the joystick's hat switch Y axis (=axis2)
/// </summary>
public class SizeChanger : MonoBehaviour
{
	[Tooltip("Name of the device to read input from")]
	public string deviceName       = "Joystick1";

	[Tooltip("Device channel name that changes the size, e.g., Joystick hat switch up/down (axis2)")]
	public string channelNameScale = "axis2";

	[Tooltip("Device channel name that resets the size")]
	public string channelNameReset = "button1";


	/// <summary>
	/// Initialises the script.
	/// </summary>
	///
	void Start ()
	{
		// remember the scale at start
		originalSize = this.transform.localScale;
		
		// create the variables to check the state of the input device
		inputReset = new InputDeviceHandler(deviceName, channelNameReset);
		inputScale = new InputDeviceHandler(deviceName, channelNameScale);
	}


	/// <summary>
	/// Called once per frame
	/// </summary>
	///
	void Update()
	{
		// scale based on the axis value
		if (inputScale.GetAxis() > 0)
		{
			// larger
			transform.localScale = transform.localScale * 1.01f;
		}
		else if (inputScale.GetAxis() < 0)
		{
			// smaller
			transform.localScale = transform.localScale * 0.99f;
		}

		// reset button pressed?
		if (inputReset.GetButtonDown())
		{
			transform.localScale = originalSize;
		}
	}
	

	private InputDeviceHandler inputScale, inputReset;
	private Vector3            originalSize;
}

using UnityEngine;
using MoCap;
using System;

public class Transporter : MonoBehaviour
{
	public string deviceName = "";
	public string channelName = "";
	public string alternateButtonName = "";
	public string groundTag = "floor";
	public Transform offsetObject = null;
	public float transitionTime = 0.1f;


	public enum TransitionType
	{
		Immediate,
		Blink,
		MoveLinear,
		MoveSmooth
	}


	void Start()
	{
		rayComponent = GetComponentInChildren<PointerRay>();
		button = new InputDeviceHandler(deviceName, channelName, alternateButtonName);
	}


	void Update()
	{
		if (button.GetButtonDown())
		{
			RaycastHit hit;
			if (rayComponent != null)
			{
				// ray component is there > query
				hit = rayComponent.GetRayTarget();
			}
			else
			{
				// no ray component, constryct raycast here
				// construct ray
				Ray ray = new Ray(transform.position, transform.forward);
				// do raycast
				Physics.Raycast(ray, out hit);
			}

			if ((hit.distance > 0) && hit.transform.gameObject.tag.Equals(groundTag))
			{
				// calculate target point
				Vector3 startPoint = offsetObject.position;
				Vector3 offset = hit.point - this.transform.position;
				offset.y = 0;
				Vector3 endPoint = startPoint + offset;

				// activate transition
				transition = new Transition_Move(startPoint, endPoint, transitionTime, false);
				
				// disable ray while transitioning
				if (rayComponent != null)
				{
					rayComponent.SetEnabled(false);
				}
			}
		}

		if (transition != null)
		{
			transition.Update(offsetObject);
			if (transition.IsFinished())
			{
				transition = null;
				// enable ray after transitioning
				if (rayComponent != null)
				{
					rayComponent.SetEnabled(true);
				}
			}
		}
	}


	public void OnGUI()
	{
		if (transition != null)
		{
			transition.UpdateUI();
		}
	}


	private interface ITransition
	{
		void Update(Transform offsetObject);
		void UpdateUI();
		bool IsFinished();
	}


	private class Transition_Immediate : ITransition
	{
		public Transition_Immediate(Vector3 endPoint)
		{
			this.endPoint = endPoint;
		}

		public void Update(Transform offsetObject)
		{
			// change offset immediately
			offsetObject.position = endPoint;
		}

		public void UpdateUI()
		{
			// nothing to do
		}

		public bool IsFinished()
		{
			return true; // immediate result
		}

		private Vector3 endPoint;
	}


	private class Transition_Blink : ITransition
	{
		public Transition_Blink(Vector3 endPoint, float duration)
		{
			this.endPoint = endPoint;
			this.duration = duration;

			progress = 0;
			moved = false;
		}

		public void Update(Transform offsetObject)
		{
			// move immediately to B when blink is half way ("eyelids" closed)
			progress += Time.deltaTime / duration;
			progress = Math.Min(1, progress);
			if ((progress >= 0.5f) && !moved)
			{
				offsetObject.position = endPoint;
				moved = true; // only move once
			}
		}

		public void UpdateUI()
		{
			// draw "eyelids"
			GUI.color = new Color(1, 1, 1);
			float height = 1 - Math.Abs(progress * 2 - 1); // Vertical lid position from [0....1....0]
			height *= Screen.height / 2;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, height), Texture2D.blackTexture);
			GUI.DrawTexture(new Rect(0, Screen.height - height, Screen.width, height), Texture2D.blackTexture);
		}

		public bool IsFinished()
		{
			return progress >= 1; // movement has finished
		}

		private Vector3 endPoint;
		private float duration, progress;
		private bool moved;
	}


	private class Transition_Move : ITransition
	{
		public Transition_Move(Vector3 startPoint, Vector3 endPoint, float duration, bool smooth)
		{
			this.startPoint = startPoint;
			this.endPoint = endPoint;
			this.duration = duration;
			this.smooth = smooth;

			progress = 0;
		}

		public void Update(Transform offsetObject)
		{
			// move from A to B
			progress += Time.deltaTime / duration;
			progress = Math.Min(1, progress);
			// linear: lerpFactor = progress. smooth: lerpFactor = sin(progress * PI/2) ^ 2
			float lerpFactor = smooth ? (float)Math.Pow(Math.Sin(progress * Math.PI / 2), 2) : progress;
			offsetObject.position = Vector3.Lerp(startPoint, endPoint, lerpFactor);
		}

		public void UpdateUI()
		{
			// nothing to do
		}

		public bool IsFinished()
		{
			return progress >= 1; // movement has finished
		}

		private Vector3 startPoint, endPoint;
		private float duration, progress;
		private bool smooth;
	}


	private PointerRay rayComponent;
	private InputDeviceHandler button;
	private ITransition transition;
}

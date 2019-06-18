using DG.Tweening;
using UnityEngine;

public class Button : MonoBehaviour
{
	public Vector3 position
	{
		get
		{
			return transform.position;
		}
		set
		{
			transform.position = value;
		}
	}

	public Vector3 controlOffset;

	public Vector3 control
	{
		get
		{
			return position + controlOffset;
		}
		set
		{
			controlOffset = value - position;
		}
	}

	public int inputIndex;
	private bool currentIsPressed = false;

	private float intensityRatio;
	private bool intensityPhase;
	public float intensityMinInterval;
	public float intensityMaxInterval;

	[HideInInspector]
	public ArduinoCommunication arduinoCommunication;

	private void Pick()
	{
		intensityPhase = !intensityPhase;
		DOTween.To(
			() => intensityRatio,
			(value) =>
			{
				intensityRatio = value;
			},
			intensityPhase ? 1f : 0f,
			Random.Range(intensityMinInterval, intensityMaxInterval)
		)
		.SetEase(Ease.InOutCubic)
		.OnComplete(Pick);
	}

	private void Start()
	{
		intensityRatio = 0f;
		Pick();
	}

	private void Update()
	{
		bool isPressed = arduinoCommunication.IsButtonPressed(inputIndex);

		if (isPressed && !currentIsPressed)
		{
		}

		currentIsPressed = isPressed;

		var intensity = .01f + intensityRatio * intensityRatio * .05f;
		arduinoCommunication.SetButtonColor(inputIndex, Color.white * intensity);
	}
}

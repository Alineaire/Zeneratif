using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
	public KeyCode keyCode;
	private bool currentIsPressed = false;

	private float intensityRatio;
	private bool intensityPhase;
	public float intensityMinInterval;
	public float intensityMaxInterval;

	private float flashIntensity = 0f;
	private Tweener flashTweener;

	public float minRadius;
	public float maxRadius;

	public Image ui;
	private Material uiMaterial;

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
		uiMaterial = new Material(ui.material);
		ui.material = uiMaterial;
		Pick();
	}

	private void Update()
	{
		bool isPressed = arduinoCommunication.IsButtonPressed(inputIndex) || Input.GetKey(keyCode);

		if (isPressed && !currentIsPressed)
		{
			if (flashTweener != null)
			{
				flashTweener.Kill();
			}

			flashIntensity = 1f;
			flashTweener = DOTween.To(
				() => flashIntensity,
				(value) =>
				{
					flashIntensity = value;
				},
				0f,
				.3f
			)
				.SetEase(Ease.InOutCubic);
		}

		currentIsPressed = isPressed;

		var intensity = Mathf.Max(.01f + intensityRatio * intensityRatio * .05f, flashIntensity);
		arduinoCommunication.SetButtonColor(inputIndex, Color.white * intensity);
		uiMaterial.SetFloat("_Inside", intensity);
		uiMaterial.SetFloat("_Radius", Mathf.Lerp(minRadius, maxRadius, intensityRatio));
	}
}

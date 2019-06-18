using UnityEngine;

public class GameManager : MonoBehaviour
{
	private Button[] buttons;
	private int previousButtonIndex;
	private int nextButtonIndex;

	private float time;
	public float durationBetweenButtons = 5f;
	public float pressDuration = .5f;
	private bool pressed = false;
	private bool pressChecked = true;

	public int minScore;
	public int maxScore;
	public int scoreDecreaseOnMiss;
	public int scorePerLayer;
	private int currentScore;

	public Transform ball;

	public SuperCollider superCollider;
	public ArduinoCommunication arduinoCommunication;

#if UNITY_EDITOR
	public bool debugPressed;
#endif

	private void Awake()
	{
		if (Display.displays.Length > 1)
		{
			Display.displays[1].Activate();
		}

		buttons = GetComponentsInChildren<Button>();
		foreach (var button in buttons)
		{
			button.arduinoCommunication = arduinoCommunication;
		}

		currentScore = minScore;
		time = 0f;

		previousButtonIndex = Random.Range(0, buttons.Length);
		Bounce();

		SetBallPosition(0f);
	}

	private void Bounce()
	{
		do
		{
			nextButtonIndex = Random.Range(0, buttons.Length);
		} while (previousButtonIndex == nextButtonIndex);
	}

	private void SetBallPosition(float t)
	{
		var a = buttons[previousButtonIndex].position;
		var b = buttons[previousButtonIndex].control;
		var c = buttons[nextButtonIndex].control;
		var d = buttons[nextButtonIndex].position;
		float mt = 1f - t;
		var result = a * (mt * mt * mt)
			+ b * (3f * t * mt * mt)
			+ c * (3f * t * t * mt)
			+ d * (t * t * t);
		ball.position = result;
	}

	private void CheckPress(int buttonIndex)
	{
		if (arduinoCommunication.IsButtonPressed(buttons[buttonIndex].inputIndex))
		{
			Debug.Log("Pressed!");
			pressed = true;
		}
	}

	private void Update()
	{
		time += Time.deltaTime;
		if (time >= durationBetweenButtons)
		{
			time -= durationBetweenButtons;
			pressChecked = false;
			previousButtonIndex = nextButtonIndex;
			Bounce();
		}

		if (time < pressDuration)
		{
			CheckPress(previousButtonIndex);
		}

		if (time >= durationBetweenButtons - pressDuration)
		{
			CheckPress(nextButtonIndex);
		}

		if (!pressChecked && time >= pressDuration)
		{
			pressChecked = true;

#if UNITY_EDITOR
			pressed = pressed || debugPressed;
#endif

			if (pressed)
			{
				currentScore = Mathf.Min(currentScore + 1, maxScore);
			}
			else
			{
				currentScore = Mathf.Max(currentScore - scoreDecreaseOnMiss, minScore);
			}
			Debug.LogFormat("Score: {0}", currentScore);

			superCollider.SendBounce(currentScore / scorePerLayer);

			pressed = false;
		}

		SetBallPosition(time / durationBetweenButtons);
	}
}

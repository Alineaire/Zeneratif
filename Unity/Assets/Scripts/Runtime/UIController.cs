using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public ArduinoCommunication arduinoCommunication;
	public InputField comPort;

	public void ConnectCOM()
	{
		arduinoCommunication.Open(comPort.text);
	}
}

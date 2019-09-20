using UnityEngine;
using UnityEngine.UI;

public class ControlUIController : MonoBehaviour
{
	public ArduinoCommunication arduinoCommunication;
	public InputField comPort;

	public void ConnectCOM()
	{
		arduinoCommunication.Open(comPort.text);
	}
}

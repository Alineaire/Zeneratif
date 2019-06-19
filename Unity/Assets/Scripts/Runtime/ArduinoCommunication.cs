using CommandMessenger;
using CommandMessenger.Transport.Serial;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

public class ArduinoCommunication : MonoBehaviour
{
	private enum Command
	{
		UnknownCommand,
		InvalidArgument,

		ReadyRequest,
		ReadyResponse,

		SetButtonColorRequest,
		SetButtonColorResponse,

		TurnOffRequest,
		TurnOffResponse,

		ButtonsStateUpdated,
	};

	public int speed = 9600;

	private SerialTransport serialTransport;
	private CmdMessenger cmdMessenger;

#if UNITY_EDITOR
	public bool debugNewLines;
#endif

	public float intensity = 255f;

	private const int ButtonCount = 4;
	private bool[] buttonStates = new bool[ButtonCount];

	private void NewLineReceived(object sender, CommandEventArgs e)
	{
		Debug.LogFormat("Received: {0}", e.Command.CommandString());
	}

	private void NewLineSent(object sender, CommandEventArgs e)
	{
		Debug.LogFormat("Sent: {0}", e.Command.CommandString());
	}

	private void OnUnknownCommand(ReceivedCommand command)
	{
		Debug.LogWarningFormat("Command without attached callback received: {0}", command.CmdId);
	}

	private void OnInvalidArgument(ReceivedCommand command)
	{
		Debug.LogWarningFormat("Command with invalid argument received: {0}", command.RawString);
	}

	private void OnReadyResponse(ReceivedCommand command)
	{
		Debug.Log("Arduino is ready.");
	}

	private void OnSetButtonColorResponse(ReceivedCommand command)
	{
	}

	private void OnTurnOffResponse(ReceivedCommand command)
	{
	}

	private void OnButtonsStateUpdated(ReceivedCommand command)
	{
		for (int i = 0; i < ButtonCount; ++i)
		{
			buttonStates[i] = command.ReadBoolArg();
		}
	}

	private void OnEnable()
	{
		serialTransport = new SerialTransport
		{
			CurrentSerialSettings = {
				BaudRate = speed,
				DtrEnable = false,

			}
		};

		for (int i = 0; i < ButtonCount; ++i)
		{
			buttonStates[i] = false;
		}

		RefreshConnection();
	}

	private void OnDisable()
	{
		for (int retry = 0; retry < 10; ++retry)
		{
			var command = new SendCommand((int)Command.TurnOffRequest, (int)Command.TurnOffResponse, 5);
			var result = cmdMessenger.SendCommand(command);
			if (result.Ok)
				break;
		}

		Close();
	}

	private void Close()
	{
		cmdMessenger.Disconnect();
		cmdMessenger.Dispose();
		cmdMessenger = null;
	}

	public void Open(string portName)
	{
		if (cmdMessenger != null)
		{
			Close();
		}

		Debug.LogFormat("Opening port {0}.", portName);

		serialTransport.CurrentSerialSettings.PortName = portName;
		cmdMessenger = new CmdMessenger(serialTransport, BoardType.Bit16);

#if UNITY_EDITOR
		if (debugNewLines)
		{
			cmdMessenger.NewLineReceived += NewLineReceived;
			cmdMessenger.NewLineSent += NewLineSent;
		}
#endif

		cmdMessenger.Attach(OnUnknownCommand);
		cmdMessenger.Attach((int)Command.UnknownCommand, OnUnknownCommand);
		cmdMessenger.Attach((int)Command.InvalidArgument, OnInvalidArgument);
		cmdMessenger.Attach((int)Command.ReadyResponse, OnReadyResponse);
		cmdMessenger.Attach((int)Command.SetButtonColorResponse, OnSetButtonColorResponse);
		cmdMessenger.Attach((int)Command.TurnOffResponse, OnTurnOffResponse);
		cmdMessenger.Attach((int)Command.ButtonsStateUpdated, OnButtonsStateUpdated);

		var success = cmdMessenger.Connect();
		Debug.Log(success ? "Connection succeeded." : "Connection failed.");

		if (success)
		{
			var command = new SendCommand((int)Command.ReadyRequest);
			cmdMessenger.SendCommand(command);
		}
	}

	private void RefreshConnection()
	{
		var portNames = SerialPort.GetPortNames();

		if (cmdMessenger == null)
		{
			if (portNames.Length > 0)
			{
				var portName = portNames[0];
				Open(portName);
			}
		}
		else
		{
			var count = portNames.Count(name => name == serialTransport.CurrentSerialSettings.PortName);
			if (count == 0)
			{
				Debug.Log("Port unavailable, closing.");
				Close();
			}
		}
	}

	private void Update()
	{
		RefreshConnection();
	}

	public bool IsButtonPressed(int index)
	{
		return buttonStates[index];
	}

	public void SetButtonColor(int index, Color color)
	{
		if (cmdMessenger == null)
			return;

		short r = (short)Mathf.RoundToInt(color.r * intensity);
		short g = (short)Mathf.RoundToInt(color.g * intensity);
		short b = (short)Mathf.RoundToInt(color.b * intensity);

		var command = new SendCommand((int)Command.SetButtonColorRequest);
		command.AddArgument((short)index);
		command.AddArgument(r);
		command.AddArgument(g);
		command.AddArgument(b);
		cmdMessenger.SendCommand(command);
	}
}

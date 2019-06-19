#include <Arduino.h>
#include <CmdMessenger.h>
#include <FastLED.h>

const uint32_t UartSpeed = 38400;

const int ButtonCount = 4;
const int LedsPerButton = 17;

constexpr int ButtonPinByButton(int buttonIndex)
{
    return 3 + buttonIndex * 2;
}

constexpr int LedPinByButton(int buttonIndex)
{
    return 2 + buttonIndex * 2;
}

enum Command
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

CmdMessenger cmdMessenger{Serial};
CRGB leds[ButtonCount][LedsPerButton];

void OnUnknownCommand()
{
    cmdMessenger.sendCmd(Command::UnknownCommand, cmdMessenger.commandID());
}

void OnReadyRequest()
{
    cmdMessenger.sendCmd(Command::ReadyResponse);
}

void OnSetButtonColorRequest()
{
    auto buttonIndex = cmdMessenger.readInt16Arg();
    auto r = cmdMessenger.readInt16Arg();
    auto g = cmdMessenger.readInt16Arg();
    auto b = cmdMessenger.readInt16Arg();

    if (buttonIndex < 0 || buttonIndex >= ButtonCount || r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
    {
        cmdMessenger.sendCmdStart(Command::InvalidArgument);
        cmdMessenger.sendCmdArg(buttonIndex);
        cmdMessenger.sendCmdArg(r);
        cmdMessenger.sendCmdArg(g);
        cmdMessenger.sendCmdArg(b);
        cmdMessenger.sendCmdEnd();
        return;
    }

    CRGB color{(uint8_t)r, (uint8_t)g, (uint8_t)b};
    for (int i = 0; i < LedsPerButton; i++)
    {
        leds[buttonIndex][i] = color;
    }

    cmdMessenger.sendCmd(Command::SetButtonColorResponse);
}

void OnTurnOffRequest()
{
    for (int buttonIndex = 0; buttonIndex < ButtonCount; buttonIndex++)
    {
        for (int i = 0; i < LedsPerButton; i++)
        {
            leds[buttonIndex][i] = CRGB::Black;
        }
    }

    cmdMessenger.sendCmd(Command::TurnOffResponse);
}

template <int buttonIndex = 0>
void initButtonRecursive();

template <int buttonIndex>
void initButtonRecursive()
{
    pinMode(ButtonPinByButton(buttonIndex), INPUT_PULLUP);
    FastLED.addLeds<WS2812B, (uint8_t)LedPinByButton(buttonIndex), GRB>(leds[buttonIndex], LedsPerButton);
    initButtonRecursive<buttonIndex + 1>();
}

template <>
void initButtonRecursive<ButtonCount>()
{
}

void setup()
{
    Serial.begin(UartSpeed);
    while (!Serial)
    {
        delay(10);
    }

    initButtonRecursive();

    cmdMessenger.printLfCr();
    cmdMessenger.attach(OnUnknownCommand);
    cmdMessenger.attach(Command::ReadyRequest, OnReadyRequest);
    cmdMessenger.attach(Command::SetButtonColorRequest, OnSetButtonColorRequest);
    cmdMessenger.attach(Command::TurnOffRequest, OnTurnOffRequest);
}

template <int buttonIndex = 0>
void addButtonStateRecursive();

template <int buttonIndex>
void addButtonStateRecursive()
{
    auto state = digitalRead(ButtonPinByButton(buttonIndex));
    cmdMessenger.sendCmdArg(state == LOW);
    addButtonStateRecursive<buttonIndex + 1>();
}

template <>
void addButtonStateRecursive<ButtonCount>()
{
}

void loop()
{
    cmdMessenger.feedinSerialData();

    cmdMessenger.sendCmdStart(Command::ButtonsStateUpdated);
    addButtonStateRecursive();
    cmdMessenger.sendCmdEnd();

    FastLED.show();
}

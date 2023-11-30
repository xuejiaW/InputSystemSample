using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(displayName = "Gamepad but Only Analog", commonUsages = new[] {"Trigger", "Stick"})]
public class AnalogOnlyGamepad : InputDevice, IInputUpdateCallbackReceiver
{
    [InputControl]
    public AxisControl leftAnalog { get; private set; }

    [InputControl]
    public AxisControl rightAnalog { get; private set; }

    [InputControl(name = "leftButton", offset = 0)]
    public ButtonControl leftBtn { get; private set; }

    [InputControl(name = "rightButton", offset = 1)]
    public ButtonControl rightBtn { get; private set; }

    private Func<float> m_LeftAnalogGetFunc = default;
    private Func<float> m_RightAnalogGetFunc = default;
    private Func<bool> m_LeftBtnGetFunc = default;
    private Func<bool> m_RightBtnGetFunc = default;

    public static AnalogOnlyGamepad current { get; private set; }
    public new static IReadOnlyList<AnalogOnlyGamepad> all => s_AllDevices;
    private static List<AnalogOnlyGamepad> s_AllDevices = new();

    static AnalogOnlyGamepad()
    {
        string productSuffix = "OnlyGamepad";

        InputSystem.RegisterLayout<AnalogOnlyGamepad>(matches: new InputDeviceMatcher()
                                                         .WithProduct($".*{productSuffix}"));
        PartialGamepadCreator.BindWithGamepadCreation(productSuffix, new List<string> {"Trigger", "Stick"});
    }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        leftAnalog = GetChildControl<AxisControl>("leftAnalog");
        rightAnalog = GetChildControl<AxisControl>("rightAnalog");
        leftBtn = GetChildControl<ButtonControl>("leftButton");
        rightBtn = GetChildControl<ButtonControl>("rightButton");

        GetValueFuncBasedOnAnalogType();

        void GetValueFuncBasedOnAnalogType()
        {
            bool isTrigger = device.name.Contains("Trigger") || (description.product?.Contains("Trigger") ?? false);
            m_LeftAnalogGetFunc = isTrigger
                ? () => Gamepad.current.leftTrigger.ReadValue()
                : () => Gamepad.current.leftStick.ReadValue().magnitude;

            m_RightAnalogGetFunc = isTrigger
                ? () => Gamepad.current.rightTrigger.ReadValue()
                : () => Gamepad.current.rightStick.ReadValue().magnitude;

            m_LeftBtnGetFunc = isTrigger
                ? () => Gamepad.current.leftTrigger.IsPressed()
                : () => Gamepad.current.leftStickButton.IsPressed();

            m_RightBtnGetFunc = isTrigger
                ? () => Gamepad.current.rightTrigger.IsPressed()
                : () => Gamepad.current.rightStickButton.IsPressed();
        }
    }

    public void OnUpdate()
    {
        if (Application.isPlaying && InputState.currentUpdateType != InputUpdateType.Dynamic) return;
        InputSystem.QueueDeltaStateEvent(leftAnalog, m_LeftAnalogGetFunc());
        InputSystem.QueueDeltaStateEvent(rightAnalog, m_RightAnalogGetFunc());
        InputSystem.QueueDeltaStateEvent(leftBtn, m_LeftBtnGetFunc());
        InputSystem.QueueDeltaStateEvent(rightBtn, m_RightBtnGetFunc());
    }

    public override void MakeCurrent()
    {
        base.MakeCurrent();
        current = this;
    }

    protected override void OnAdded()
    {
        base.OnAdded();
        s_AllDevices.Add(this);
    }

    protected override void OnRemoved()
    {
        base.OnRemoved();
        s_AllDevices.Remove(this);
    }
}
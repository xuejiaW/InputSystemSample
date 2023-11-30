#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(stateType = typeof(HalfGamepadState), commonUsages = new[] {"Left", "Right"},
                       displayName = "Half Gamepad")]
public class HalfGamepad : InputDevice, IInputUpdateCallbackReceiver
{
    private List<KeyValuePair<HalfGamepadBtnEnum, Func<bool>>> m_BtnEnum2GetFunc = new();
    private Func<Vector2> m_StickGetFunc = default;
    private Func<float> m_TriggerGetFunc = default;

    public ButtonControl triggerBtn { get; private set; }
    public AxisControl axisX { get; private set; }
    public AxisControl axisY { get; private set; }

    public static HalfGamepad current { get; private set; }
    public new static IReadOnlyList<HalfGamepad> all => s_AllDevices;
    private static List<HalfGamepad> s_AllDevices = new();

    static HalfGamepad()
    {
        string productSuffix = "HalfGamepad";
        InputSystem.RegisterLayout<HalfGamepad>(matches: new InputDeviceMatcher().WithProduct($".*{productSuffix}"));
        PartialGamepadCreator.BindWithGamepadCreation(productSuffix, new List<string> {"Left", "Right"});
    }

    protected override void FinishSetup()
    {
        base.FinishSetup();

        triggerBtn = GetChildControl<ButtonControl>("Trigger");
        axisX = GetChildControl<AxisControl>("axisX");
        axisY = GetChildControl<AxisControl>("axisY");

        bool isLeft = device.name.Contains("Left") || (description.product?.Contains("Left") ?? false);
        Gamepad gamepad = Gamepad.current;
        AddBtnPair(HalfGamepadBtnEnum.Trigger,
                   () => isLeft ? gamepad.leftTrigger.isPressed : gamepad.rightTrigger.isPressed);
        AddBtnPair(HalfGamepadBtnEnum.Shoulder,
                   () => isLeft ? gamepad.leftShoulder.isPressed : gamepad.rightShoulder.isPressed);
        AddBtnPair(HalfGamepadBtnEnum.UpBtn,
                   () => isLeft ? gamepad.dpad.up.isPressed : gamepad.buttonNorth.isPressed);
        AddBtnPair(HalfGamepadBtnEnum.DownBtn,
                   () => isLeft ? gamepad.dpad.down.isPressed : gamepad.buttonSouth.isPressed);
        AddBtnPair(HalfGamepadBtnEnum.LeftBtn,
                   () => isLeft ? gamepad.dpad.left.isPressed : gamepad.buttonWest.isPressed);
        AddBtnPair(HalfGamepadBtnEnum.RightBtn,
                   () => isLeft ? gamepad.dpad.right.isPressed : gamepad.buttonEast.isPressed);
        AddBtnPair(HalfGamepadBtnEnum.Stick,
                   () => isLeft ? gamepad.leftStickButton.isPressed : gamepad.rightStickButton.isPressed);
        AddBtnPair(HalfGamepadBtnEnum.Start,
                   () => isLeft ? gamepad.selectButton.isPressed : gamepad.startButton.isPressed);
        m_StickGetFunc = () => isLeft ? gamepad.leftStick.ReadValue() : gamepad.rightStick.ReadValue();
        m_TriggerGetFunc = () => isLeft ? gamepad.leftTrigger.ReadValue() : gamepad.rightTrigger.ReadValue();


        void AddBtnPair(HalfGamepadBtnEnum btnEnum, Func<bool> getFunc)
        {
            m_BtnEnum2GetFunc.Add(new KeyValuePair<HalfGamepadBtnEnum, Func<bool>>(btnEnum, getFunc));
        }
    }

    public void OnUpdate()
    {
        if (Application.isPlaying && InputState.currentUpdateType != InputUpdateType.Dynamic) return;
        var state = new HalfGamepadState();

        m_BtnEnum2GetFunc.ForEach(pair => SetButtonBit(pair.Key, pair.Value()));

        state.axisX = m_StickGetFunc().x;
        state.axisY = m_StickGetFunc().y;
        state.triggerAxis = m_TriggerGetFunc();

        InputSystem.QueueStateEvent(this, state);

        void SetButtonBit(HalfGamepadBtnEnum bits, bool on)
        {
            state.buttons = (byte) (on ? state.buttons | (1 << (short) bits) : state.buttons & ~(1 << (short) bits));
        }
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
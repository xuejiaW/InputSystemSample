using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using BtnEnum = HalfGamepadBtnEnum;

public struct HalfGamepadState : IInputStateTypeInfo
{
    public FourCC format => new('H', 'F', 'G', 'P');

    [InputControl(name = "Trigger", layout = "Button", bit = (ushort) BtnEnum.Trigger)]
    [InputControl(name = "Shoulder", layout = "Button", bit = (ushort) BtnEnum.Shoulder)]
    [InputControl(name = "UpBtn", layout = "Button", bit = (ushort) BtnEnum.UpBtn, displayName = "Up")]
    [InputControl(name = "DownBtn", layout = "Button", bit = (ushort) BtnEnum.DownBtn, displayName = "Down")]
    [InputControl(name = "LeftBtn", layout = "Button", bit = (ushort) BtnEnum.LeftBtn, displayName = "Left")]
    [InputControl(name = "RightBtn", layout = "Button", bit = (ushort) BtnEnum.RightBtn, displayName = "Right")]
    [InputControl(name = "Stick", layout = "Button", bit = (ushort) BtnEnum.Stick)]
    [InputControl(name = "Start", layout = "Button", bit = (ushort) BtnEnum.Start)]
    public int buttons;

    [InputControl(layout = "Axis")] public float triggerAxis;
    [InputControl(layout = "Axis")] public float axisX;
    [InputControl(layout = "Axis")] public float axisY;
}
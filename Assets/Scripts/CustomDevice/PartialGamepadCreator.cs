using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public static class PartialGamepadCreator
{

    public static void BindWithGamepadCreation(string productSuffix, List<string> usageForPartialGamepad)
    {
        string productRegex = $".*{productSuffix}";
        if (Gamepad.current != null && !InputSystem.devices.Any(MatchProduct))
            AddDevice();

        InputSystem.onDeviceChange += OnDeviceChanged;
        return;

        bool MatchProduct(InputDevice device) => Regex.IsMatch(device.description.product, productRegex);

        void OnDeviceChanged(InputDevice device, InputDeviceChange change)
        {
            if (device is Gamepad && change == InputDeviceChange.Added)
                AddDevice();
            else if (device is Gamepad && change == InputDeviceChange.Removed)
                RemoveDevice();
        }

        void RemoveDevice()
        {
            List<InputDevice> toRemoveList = InputSystem.devices.Where(MatchProduct).ToList();
            toRemoveList.ForEach(InputSystem.RemoveDevice);
        }

        void AddDevice()
        {
            usageForPartialGamepad.ForEach(AddDeviceImpl);

            void AddDeviceImpl(string usage)
            {
                InputDevice device = InputSystem.AddDevice(new InputDeviceDescription
                {
                    product = $"{usage}{productSuffix}"
                });
                InputSystem.SetDeviceUsage(device, usage);
            }
        }

    }
}

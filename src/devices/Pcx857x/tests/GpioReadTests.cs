﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.Gpio;
using Xunit;

namespace Iot.Device.Pcx857x.Tests
{
    public class GpioReadTests : Pcx857xTest
    {
        [Theory]
        [MemberData(nameof(TestDevices))]
        public void Read_InvalidPin(TestDevice testDevice)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => testDevice.Device.Read(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => testDevice.Device.Read(testDevice.Device.PinCount));
            Assert.Throws<ArgumentOutOfRangeException>(() => testDevice.Device.Read(testDevice.Device.PinCount + 1));
        }

        [Theory]
        [MemberData(nameof(TestDevices))]
        public void Read_GoodPin(TestDevice testDevice)
        {
            Pcx857x device = testDevice.Device;
            for (int pin = 0; pin < testDevice.Device.PinCount; pin++)
            {
                // Set pin to input
                device.SetPinMode(pin, PinMode.Input);

                bool first = pin < 8;
                int register = first ? 0x00 : 0x01;

                // Flip the bit on (set the backing buffer directly to simulate incoming data)
                testDevice.ChipMock.Registers[register] = (byte)(1 << (first ? pin : pin - 8));
                Assert.Equal(PinValue.High, device.Read(pin));

                // Clear the register
                testDevice.ChipMock.Registers[register] = 0x00;
                Assert.Equal(PinValue.Low, device.Read(pin));
            }
        }
    }
}
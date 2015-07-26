﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib.New
{
    public interface INintrollerState
    {
        void Update(byte[] data);
        float GetValue(string input);
        void SetCalibration(Calibrations.CalibrationPreset preset);
        void SetCalibration(INintrollerState from);
        void SetCalibration(string);
        string GetCalibrationString();
    }

    public interface INintrollerParsable
    {
        void Parse(byte[] input, int offset = 0);
    }

    public interface INintrollerNormalizable
    {
        void Normalize();
    }

    public interface INintrollerBounds
    {
        bool InBounds(int x, int y = 0, int z = 0);
    }
}

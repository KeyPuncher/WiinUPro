using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib
{
    public struct CoreButtons : INintrollerParsable
    {
        public bool A, B;
        public bool One, Two;
        public bool Up, Down, Left, Right;
        public bool Plus, Minus, Home;

        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        public void Parse(byte[] input, int offset = 0)
        {
            InputReport type = (InputReport)input[0];

            if (type != InputReport.ExtOnly)
            {
                A     = (input[offset + 1] & 0x08) != 0;
                B     = (input[offset + 1] & 0x04) != 0;
                One   = (input[offset + 1] & 0x02) != 0;
                Two   = (input[offset + 1] & 0x01) != 0;
                Home  = (input[offset + 1] & 0x80) != 0;
                Minus = (input[offset + 1] & 0x10) != 0;
                Plus  = (input[offset + 0] & 0x10) != 0;
                Up    = (input[offset + 0] & 0x08) != 0;
                Down  = (input[offset + 0] & 0x04) != 0;
                Right = (input[offset + 0] & 0x02) != 0;
                Left  = (input[offset + 0] & 0x01) != 0;
            }
        }
    }

    public struct Accelerometer : INintrollerParsable, INintrollerNormalizable
    {
        public int rawX, rawY, rawZ;
        public float X, Y, Z;

        // calibration values
        public int centerX, centerY, centerZ;
        public int minX, minY, minZ;
        public int maxX, maxY, maxZ;
        public int deadX, deadY, deadZ;

        public void Parse(byte[] input, int offset = 0)
        {
            InputReport type = (InputReport)input[0];

            InputReport[] accepted = new InputReport[]
            {
                InputReport.BtnsAcc,
                InputReport.BtnsAccExt,
                InputReport.BtnsAccIR,
                InputReport.BtnsAccIRExt
            };

            if (accepted.Contains(type))
            {
                rawX = input[offset + 0];
                rawY = input[offset + 1];
                rawZ = input[offset + 2];
            }
        }

        public void Calibrate(Accelerometer calibration)
        {
            centerX = calibration.centerX;
            centerY = calibration.centerY;
            centerZ = calibration.centerZ;

            minX = calibration.minX;
            minY = calibration.minY;
            minZ = calibration.minZ;

            maxX = calibration.maxX;
            maxY = calibration.maxY;
            maxZ = calibration.maxZ;

            deadX = calibration.deadX;
            deadY = calibration.deadY;
            deadZ = calibration.deadZ;
        }

        public void Normalize()
        {
            X = Nintroller.Normalize(rawX, minX, centerX, maxX, deadX);
            Y = Nintroller.Normalize(rawY, minY, centerY, maxY, deadY);
            Z = Nintroller.Normalize(rawZ, minZ, centerZ, maxZ, deadZ);
        }

        public override string ToString()
        {
            return string.Format("X:{0} Y:{1} Z{2}", X, Y, Z);
        }
    }

    public struct IRPoint
    {
        public int rawX, rawY, size;
        //public float x, y;
        public bool visible;
    }

    public struct IR : INintrollerParsable, INintrollerNormalizable
    {
        public IRPoint point1, point2, point3, point4;
        public float rotation, distance;
        public float X, Y;

        public void Parse(byte[] input, int offset = 0)
        {
            InputReport type = (InputReport)input[0];

            if (type == InputReport.BtnsAccIR || type == InputReport.BtnsAccIRExt)
            {
                offset += 3;
            }
            else if (type != InputReport.BtnsIRExt)
            {
                return;
            }

            point1.rawX = input[offset    ] | ((input[offset + 2] >> 4) & 0x03) << 8;
            point1.rawY = input[offset + 1] | ((input[offset + 2] >> 6) & 0x03) << 8;

            if (type == InputReport.BtnsAccIR)
            {
                // Extended Mode
                point2.rawX = input[offset +  3] | ((input[offset +  5] >> 4) & 0x03) << 8;
                point2.rawY = input[offset +  4] | ((input[offset +  5] >> 6) & 0x03) << 8;
                point3.rawX = input[offset +  6] | ((input[offset +  8] >> 4) & 0x03) << 8;
                point3.rawY = input[offset +  7] | ((input[offset +  8] >> 6) & 0x03) << 8;
                point4.rawX = input[offset +  9] | ((input[offset + 11] >> 4) & 0x03) << 8;
                point4.rawY = input[offset + 10] | ((input[offset + 11] >> 6) & 0x03) << 8;

                point1.size = input[offset +  2] & 0x0F;
                point2.size = input[offset +  5] & 0x0F;
                point3.size = input[offset +  8] & 0x0F;
                point4.size = input[offset + 11] & 0x0F;

                point1.visible = !(input[offset    ] == 0xFF && input[offset +  1] == 0xFF && input[offset +  2] == 0xFF);
                point2.visible = !(input[offset + 3] == 0xFF && input[offset +  4] == 0xFF && input[offset +  5] == 0xFF);
                point3.visible = !(input[offset + 6] == 0xFF && input[offset +  7] == 0xFF && input[offset +  8] == 0xFF);
                point4.visible = !(input[offset + 9] == 0xFF && input[offset + 10] == 0xFF && input[offset + 11] == 0xFF);
            }
            else
            {
                // Basic Mode
                point2.rawX = input[offset + 3] | ((input[offset + 2] >> 0) & 0x03) << 8;
                point2.rawY = input[offset + 4] | ((input[offset + 2] >> 2) & 0x03) << 8;
                point3.rawX = input[offset + 5] | ((input[offset + 7] >> 4) & 0x03) << 8;
                point3.rawY = input[offset + 6] | ((input[offset + 7] >> 6) & 0x03) << 8;
                point4.rawX = input[offset + 8] | ((input[offset + 7] >> 0) & 0x03) << 8;
                point4.rawY = input[offset + 9] | ((input[offset + 7] >> 2) & 0x03) << 8;

                point1.size = 0x00;
                point2.size = 0x00;
                point3.size = 0x00;
                point4.size = 0x00;

                point1.visible = !(input[offset    ] == 0xFF && input[offset + 1] == 0xFF);
                point2.visible = !(input[offset + 3] == 0xFF && input[offset + 4] == 0xFF);
                point3.visible = !(input[offset + 5] == 0xFF && input[offset + 6] == 0xFF);
                point4.visible = !(input[offset + 8] == 0xFF && input[offset + 9] == 0xFF);
            }
        }

        public void Normalize()
        {
            X = (point2.rawX - point1.rawX) / 2f;
            Y = (point2.rawY - point1.rawX) / 2f;

            float denominator = (point2.rawX - point1.rawX);
            rotation = denominator == 0 ? 0f : (180 / 3.14159f) * (float)Math.Sin((point2.rawY - point1.rawX) / denominator);
            distance = (float)Math.Sqrt(Math.Pow(point2.rawX - point1.rawX, 2) + Math.Pow(point2.rawY - point1.rawY, 2));
        }

        public override string ToString()
        {
            return string.Format("X:{0} Y:{1}", X, Y);
        }
    }

    public struct Trigger : INintrollerParsable, INintrollerNormalizable
    {
        public short rawValue;
        public float value;
        public bool full;

        // calibration values
        public int min, max;

        public void Parse(byte[] input, int offset = 0)
        {
            Parse(input, offset, false);
        }

        // TODO: New: Check if we shoud be using rawValue here, & set full
        public void Parse(byte[] input, int offset, bool isLeft)
        {
            if (isLeft)
            {
                value = (byte)(((input[offset] & 0x60) >> 2) | (input[offset + 1] >> 5));
            }
            else
            {
                value = (byte)(input[offset] & 0x1F);
            }
        }

        public void Calibrate(Trigger calibration)
        {
            min = calibration.min;
            max = calibration.max;
        }

        public void Normalize()
        {
            if (rawValue < min)
            {
                value = 0f;
            }
            else
            {
                value = (float)(rawValue - min) / (float)(max == 0 ? 31 : max - min);
            }
        }
    }

    public struct Joystick : INintrollerNormalizable
    {
        public short rawX, rawY;
        public float X, Y;

        // calibration values
        public int centerX, centerY;
        public int minX, minY;
        public int maxX, maxY;
        public int deadX, deadY;
        public int deadXp, deadXn, deadYp, deadYn;

        public void Calibrate(Joystick calibration)
        {
            centerX = calibration.centerX;
            centerY = calibration.centerY;

            minX = calibration.minX;
            minY = calibration.minY;

            maxX = calibration.maxX;
            maxY = calibration.maxY;

            if (calibration.deadXp == 0 && calibration.deadXn == 0)
            {
                deadX = calibration.deadX;
                deadXp = deadX;
                deadXn = -deadXp;
            }
            else
            {
                deadXp = calibration.deadXp;
                deadXn = calibration.deadXn;
                deadX = deadXp;
            }

            if (calibration.deadYp == 0 && calibration.deadYn == 0)
            {
                deadY = calibration.deadY;
                deadYp = deadY;
                deadYn = -deadY;
            }
            else
            {
                deadYp = calibration.deadYp;
                deadYn = calibration.deadYn;
                deadY = deadYp;
            }
        }

        public void Normalize()
        {
            X = Nintroller.Normalize(rawX, minX, centerX, maxX, deadXp, deadYn);
            Y = Nintroller.Normalize(rawY, minY, centerY, maxY, deadYp, deadYn);
        }

        public override string ToString()
        {
            return string.Format("X:{0} Y:{1}", X, Y);
        }
    }

    // Balance Board
    //if (offset == 0)
    //    return;

    //raw.TopRight     = (short)((short)r[offset    ] << 8 | r[offset + 1]);
    //raw.BottomRight  = (short)((short)r[offset + 2] << 8 | r[offset + 3]);
    //raw.TopLeft      = (short)((short)r[offset + 4] << 8 | r[offset + 5]);
    //raw.BottomLeft   = (short)((short)r[offset + 6] << 8 | r[offset + 7]);

    // Calculate other members (like weight distribution)

    public struct CircularBoundry : INintrollerBounds
    {
        public int center_x;
        public int center_y;
        public int radius;

        public CircularBoundry(int center_x, int center_y, int radius)
        {
            this.center_x = center_x;
            this.center_y = center_y;
            this.radius = radius;
        }

        public bool InBounds(int x, int y = 0, int z = 0)
        {
            var dist = Math.Sqrt(Math.Pow(center_x - x, 2) + Math.Pow(center_y - y, 2));

            if (dist <= radius) 
                return true;
            else 
                return false;
        }
    }

    public struct SquareBoundry : INintrollerBounds
    {
        public int center_x;
        public int center_y;
        public int size;

        public SquareBoundry(int x, int y, int s)
        {
            center_x = x;
            center_y = y;
            size = s;
        }

        public bool InBounds(int x, int y = 0, int z = 0)
        {
            if (x > (center_x - size/2) && x < (center_x + size/2))
            {
                if (y > (center_y - size/2) && y < (center_y + size/2))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

using System;

namespace NintrollerLib
{
    /// <summary>
    /// Standard buttons included on a Wii Remote
    /// </summary>
    public struct CoreButtons : INintrollerParsable
    {
        /// <summary></summary>
        public bool A, B;
        /// <summary></summary>
        public bool One, Two;
        /// <summary></summary>
        public bool Up, Down, Left, Right;
        /// <summary></summary>
        public bool Plus, Minus, Home;

        /// <summary>
        /// Start is the same as Plus
        /// </summary>
        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        /// <summary>
        /// Select is the same as Minus
        /// </summary>
        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        /// <summary>
        /// Parses core buttons based on 2 bytes from the input data.
        /// </summary>
        /// <param name="input">Byte array of controller data.</param>
        /// <param name="offset">Starting index of Core Button bytes.</param>
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

    /// <summary>
    /// Class to hold Accerlerometer data.
    /// </summary>
    public struct Accelerometer : INintrollerParsable, INintrollerNormalizable
    {
        /// <summary>
        /// Raw Value
        /// </summary>
        public int rawX, rawY, rawZ;
        /// <summary>
        /// Normalized Value
        /// </summary>
        public float X, Y, Z;

        // calibration values
        /// <summary>Middle / Neutral position.</summary>
        public int centerX, centerY, centerZ;
        /// <summary>Minimum position (registers as -1 when normalized)</summary>
        public int minX, minY, minZ;
        /// <summary>Maximum position (registers as +1 when normalized)</summary>
        public int maxX, maxY, maxZ;
        /// <summary>Magnitude where positions should be ignored when within.</summary>
        public int deadX, deadY, deadZ;

        /// <summary>
        /// Parses 3 byte accelerometer raw data.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        public void Parse(byte[] input, int offset = 0)
        {
            rawX = input[offset + 0];
            rawY = input[offset + 1];
            rawZ = input[offset + 2];
        }

        /// <summary>
        /// Copy calibration members from provided Accelerometer structure.
        /// </summary>
        /// <param name="calibration"></param>
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

        /// <summary>
        /// Calculates the normalized values.
        /// </summary>
        public void Normalize()
        {
            // Cubic deadzone
            if (Math.Abs(rawX) < deadX && Math.Abs(rawY) < deadY && Math.Abs(rawZ) < deadZ)
            {
                X = 0;
                Y = 0;
                Z = 0;
                return;
            }

            X = Nintroller.Normalize(rawX, minX, centerX, maxX, deadX);
            Y = Nintroller.Normalize(rawY, minY, centerY, maxY, deadY);
            Z = Nintroller.Normalize(rawZ, minZ, centerZ, maxZ, deadZ);
        }

        /// <summary>
        /// Converts normalized values to displayable string.
        /// </summary>
        /// <returns>String values of X, Y, Z</returns>
        public override string ToString()
        {
            return string.Format("X:{0} Y:{1} Z{2}", X, Y, Z);
        }
    }

    /// <summary>
    /// Class to hold Gyroscopic data.
    /// </summary>
    public struct Gyroscope : INintrollerParsable
    {
        public int rawX, rawY, rawZ;

        // Degrees per second on each axis
        public double XSpeed, YSpeed, ZSpeed;

        // The current calculated angle for each axis.
        public double XAngle, YAngle, ZAngle;

        private Joystick joyConversion;

        public void Parse(byte[] input, int offset = 0)
        {
            InputReport type = (InputReport)input[0];

            // TODO: In order to calculate the angle we need an offset form Zero values and sensitivity levels.
        }
    }

    /// <summary>
    /// Individual IR sensor point.
    /// </summary>
    public struct IRPoint
    {
        /// <summary></summary>
        public int rawX, rawY, size;
        //public float x, y;
        /// <summary>
        /// If this point is visible or not
        /// </summary>
        public bool visible;
    }

    /// <summary>
    /// Collection of IR sensor data.
    /// IR resolution is 1024x768
    /// </summary>
    public struct IR : INintrollerParsable, INintrollerNormalizable
    {
        /// <summary>
        /// Individual IR Point
        /// </summary>
        public IRPoint point1, point2, point3, point4;
        /// <summary>
        /// Calculated rotation angle
        /// </summary>
        public float rotation;
        /// <summary>
        /// Distance between points 1 and 2
        /// </summary>
        public float distance;
        /// <summary>
        /// Normalized pointer position
        /// </summary>
        public float X, Y;
        /// <summary>
        /// Area in which normalization returns 0
        /// </summary>
        public INintrollerBounds boundingArea;

        public void Parse(byte[] input, int offset = 0)
        {
            Parse(input, offset, offset == 3);
        }

        /// <summary>
        /// Parse IR sensor data
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <param name="basic"></param>
        public void Parse(byte[] input, int offset, bool basic)
        {
            point1.rawX = input[offset    ] | ((input[offset + 2] & 0b_0011_0000) << 4);
            point1.rawY = input[offset + 1] | ((input[offset + 2] & 0b_1100_0000) << 2);

            if (basic)
            {
                // Basic Mode
                point2.rawX = input[offset + 3] | ((input[offset + 2] & 0b_0000_0011) << 8);
                point2.rawY = input[offset + 4] | ((input[offset + 2] & 0b_0000_1100) << 6);
                point3.rawX = input[offset + 5] | ((input[offset + 7] & 0b_0011_0000) << 4);
                point3.rawY = input[offset + 6] | ((input[offset + 7] & 0b_1100_0000) << 2);
                point4.rawX = input[offset + 8] | ((input[offset + 7] & 0b_0000_0011) << 8);
                point4.rawY = input[offset + 9] | ((input[offset + 7] & 0b_0000_1100) << 6);

                point1.size = 0x00;
                point2.size = 0x00;
                point3.size = 0x00;
                point4.size = 0x00;

                point1.visible = !(input[offset] == 0xFF && input[offset + 1] == 0xFF);
                point2.visible = !(input[offset + 3] == 0xFF && input[offset + 4] == 0xFF);
                point3.visible = !(input[offset + 5] == 0xFF && input[offset + 6] == 0xFF);
                point4.visible = !(input[offset + 8] == 0xFF && input[offset + 9] == 0xFF);
            }
            else
            {
                // Extended Mode
                point2.rawX = input[offset +  3] | ((input[offset +  5] & 0b_0011_0000) << 4);
                point2.rawY = input[offset +  4] | ((input[offset +  5] & 0b_1100_0000) << 2);
                point3.rawX = input[offset +  6] | ((input[offset +  8] & 0b_0011_0000) << 4);
                point3.rawY = input[offset +  7] | ((input[offset +  8] & 0b_1100_0000) << 2);
                point4.rawX = input[offset +  9] | ((input[offset + 11] & 0b_0011_0000) << 4);
                point4.rawY = input[offset + 10] | ((input[offset + 11] & 0b_1100_0000) << 2);

                point1.size = input[offset +  2] & 0x0F;
                point2.size = input[offset +  5] & 0x0F;
                point3.size = input[offset +  8] & 0x0F;
                point4.size = input[offset + 11] & 0x0F;

                point1.visible = !(input[offset    ] == 0xFF && input[offset +  1] == 0xFF);
                point2.visible = !(input[offset + 3] == 0xFF && input[offset +  4] == 0xFF);
                point3.visible = !(input[offset + 6] == 0xFF && input[offset +  7] == 0xFF);
                point4.visible = !(input[offset + 9] == 0xFF && input[offset + 10] == 0xFF);
            }
        }

        /// <summary>
        /// Calculates (X,Y) point, rotation, and distance
        /// </summary>
        public void Normalize()
        {
            if (!point1.visible && !point2.visible)
            {
                X = 0;
                Y = 0;
                rotation = 0;
                distance = 0;
                return;
            }

            IRPoint midPoint = new IRPoint();

            if (point1.visible && point2.visible)
            {
                midPoint.rawX = point1.rawX + (point2.rawX - point1.rawX)/2;
                midPoint.rawY = point1.rawY + (point2.rawY - point1.rawY)/2;
                midPoint.visible = true;
            }
            else if (point1.visible)
            {
                midPoint = point1;
            }
            else if (point2.visible)
            {
                midPoint = point2;
            }

            if (midPoint.visible)
            {
                if (boundingArea == null)
                {
                    boundingArea = new SquareBoundry()
                    {
                        center_x = 512,
                        center_y = 384,
                        width = 128,
                        height = 96
                    };
                }

                if (/*boundingArea != null && */boundingArea.InBounds(midPoint.rawX, midPoint.rawY))
                {
                    X = 0;
                    Y = 0;
                }
                else
                {
                    X = (midPoint.rawX - 512) / -256f;
                    Y = (midPoint.rawY - 384) / -192f;
                }
            }
            else
            {
                X = 0;
                Y = 0;
            }
        }

        /// <summary>
        /// Creates a readable string of the IR normalized oint.
        /// </summary>
        /// <returns>String representing IR Point</returns>
        public override string ToString()
        {
            return string.Format("X:{0} Y:{1}", X, Y);
        }
    }

    /// <summary>
    /// Class to represent Trigger input
    /// </summary>
    public struct Trigger : INintrollerParsable, INintrollerNormalizable
    {
        /// <summary>
        /// Raw input value
        /// </summary>
        public short rawValue;
        /// <summary>
        /// Normalized input value
        /// </summary>
        public float value;
        /// <summary>
        /// Fully pressed
        /// </summary>
        public bool full;

        /// <summary>
        /// Calibration
        /// </summary>
        public int min, max;

        /// <summary>
        /// Parses trigger data
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        public void Parse(byte[] input, int offset = 0)
        {
            Parse(input, offset, false);
        }

        // TODO: Check if we shoud be using rawValue here, & set full
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

        /// <summary>
        /// Copy calibration members from another Trigger object
        /// </summary>
        /// <param name="calibration">Calibration data to copy</param>
        public void Calibrate(Trigger calibration)
        {
            min = calibration.min;
            max = calibration.max;
        }

        /// <summary>
        /// Normalize value
        /// </summary>
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

    /// <summary>
    /// Class representing joystick data
    /// </summary>
    public struct Joystick : INintrollerNormalizable
    {
        /// <summary>
        /// Raw value
        /// </summary>
        public int rawX, rawY;
        /// <summary>
        /// Normalized value
        /// </summary>
        public float X, Y;

        // calibration values
        /// <summary>Middle / Neutral position.</summary>
        public int centerX, centerY;
        /// <summary>Minimum position (registers as -1 when normalized)</summary>
        public int minX, minY;
        /// <summary>Maximum position (registers as +1 when normalized)</summary>
        public int maxX, maxY;
        /// <summary>Magnitude where positions should be ignored when within.</summary>
        public int deadX, deadY;
        /// <summary>Asymetrical positions should be ignored when within.</summary>
        public int deadXp, deadXn, deadYp, deadYn;
        /// <summary>Minimum normalized output amount</summary>
        public float antiDeadzone;

        /// <summary>
        /// Copy calibration values from another Joystick object
        /// </summary>
        /// <param name="calibration">Joystick data to copy from</param>
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

            antiDeadzone = calibration.antiDeadzone;
        }

        /// <summary>
        /// Normalizes raw values to calculate X and Y
        /// </summary>
        public void Normalize()
        {
            // This is a square deadzone
            if ((rawX - centerX < deadXp && rawX - centerX > deadXn) && (rawY - centerY < deadYp && rawY - centerY > deadYn))
            {
                X = 0;
                Y = 0;
                return;
            }

            X = Nintroller.Normalize(rawX, minX, centerX, maxX, deadXp, deadXn);
            Y = Nintroller.Normalize(rawY, minY, centerY, maxY, deadYp, deadYn);

            if (antiDeadzone != 0)
            {
                if (X != 0)
                    X = X * (1f - antiDeadzone) + antiDeadzone * Math.Sign(X);

                if (Y != 0)
                    Y = Y * (1f - antiDeadzone) + antiDeadzone * Math.Sign(Y);
            }
        }

        /// <summary>
        /// Creates a readable string value representing X and Y
        /// </summary>
        /// <returns>String of X and Y Axes</returns>
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

    /// <summary>
    /// Represents a circular boundry
    /// </summary>
    public struct CircularBoundry : INintrollerBounds
    {
        /// <summary>Center X of circle</summary>
        public int center_x;
        /// <summary>Center Y of circle</summary>
        public int center_y;
        /// <summary>Size of the circle</summary>
        public int radius;

        /// <summary>
        /// Creates a circular boundry with the given parameters.
        /// </summary>
        /// <param name="center_x">Center point X</param>
        /// <param name="center_y">Center point Y</param>
        /// <param name="radius">Size of the boundy</param>
        public CircularBoundry(int center_x, int center_y, int radius)
        {
            this.center_x = center_x;
            this.center_y = center_y;
            this.radius = radius;
        }

        /// <summary>
        /// Checks if point is within boundry.
        /// </summary>
        /// <param name="x">X point</param>
        /// <param name="y">Y point</param>
        /// <returns>True if within boundry</returns>
        public bool InBounds(float x, float y = 0)
        {
            var dist = Math.Sqrt(Math.Pow(center_x - x, 2) + Math.Pow(center_y - y, 2));

            if (dist <= radius) 
                return true;
            else 
                return false;
        }
    }

    /// <summary>
    /// Represents a square/rectangular boundry
    /// </summary>
    public struct SquareBoundry : INintrollerBounds
    {
        /// <summary>Center X point of box</summary>
        public int center_x;
        /// <summary>Center Y point of box</summary>
        public int center_y;
        /// <summary>Width of box</summary>
        public int width;
        /// <summary>Height of box</summary>
        public int height;

        /// <summary>
        /// Creates a bounding area based on the given parameters.
        /// </summary>
        /// <param name="x">Center X</param>
        /// <param name="y">Center Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public SquareBoundry(int x, int y, int w, int h)
        {
            center_x = x;
            center_y = y;
            width = w;
            height = h;
        }

        /// <summary>
        /// Checks if the given point is in the boundry
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <returns>True if within the boundry</returns>
        public bool InBounds(float x, float y = 0)
        {
            if (x > (center_x - width/2) && x < (center_x + width/2))
            {
                if (y > (center_y - height/2) && y < (center_y + height/2))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

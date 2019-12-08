using System;

namespace Syroot.IOExtension
{
    /// <summary>
    /// Represents a 16-bit half-precision floating point value according to the IEEE 754 standard.
    /// </summary>
    /// <remarks>
    /// Examples:
    ///   SEEEEEFF_FFFFFFFF
    /// 0b00000000_00000000 = 0
    /// 1b00000000_00000000 = -0
    /// 0b00111100_00000000 = 1
    /// 0b11000000_00000000 = -2
    /// 0b11111011_11111111 = 65504 (MaxValue)
    /// 0b01111100_00000000 = PositiveInfinity
    /// 0b11111100_00000000 = NegativeInfinity
    /// </remarks>
    public struct Half
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

    /*    /// <summary>
        /// Represents the smallest positive <see cref="Half"/> value greater than zero.
        /// </summary>
        public static readonly Half Epsilon = new Half(1);

        /// <summary>
        /// Represents the largest possible value of <see cref="Half"/>.
        /// </summary>
        public static readonly Half MaxValue = new Half(0b01111011_11111111);

        /// <summary>
        /// Represents the smallest possible value of <see cref="Half"/>.
        /// </summary>
        public static readonly Half MinValue = new Half(0b11111011_11111111);

        /// <summary>
        /// Represents not a number (NaN).
        /// </summary>
        public static readonly Half NaN = new Half(0b11111110_00000000);

        /// <summary>
        /// Represents negative infinity.
        /// </summary>
        public static readonly Half NegativeInfinity = new Half(0b11111100_00000000);

        /// <summary>
        /// Represents positive infinity.
        /// </summary>
        public static readonly Half PositiveInfinity = new Half(0b01111100_00000000);*/

        private static readonly uint[] _mantissaTable;
        private static readonly uint[] _exponentTable;
        private static readonly uint[] _offsetTable;
        private static readonly ushort[] _baseTable;
        private static readonly byte[] _shiftTable;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Half"/> struct from the given <paramref name="raw"/>
        /// representation.
        /// </summary>
        /// <param name="raw">The raw representation of the internally stored bits.</param>
        public Half(ushort raw)
        {
            Raw = raw;
        }

        static Half()
        {
            int i;

            // Generate tables for Half to Single conversions.

            // Generate the mantissa table.
            _mantissaTable = new uint[2048];

            // 0 => 0
            _mantissaTable[0] = 0;

            // Transform subnormal to normalized.
            for (i = 1; i < 1024; i++)
            {
                uint m = ((uint)i) << 13;
                uint e = 0;

                while ((m & 0x00800000) == 0)
                {
                    e -= 0x00800000;
                    m <<= 1;
                }
                m &= ~0x00800000U;
                e += 0x38800000;
                _mantissaTable[i] = m | e;
            }

            // Normal case.
            for (i = 1024; i < 2048; i++)
            {
                _mantissaTable[i] = 0x38000000 + (((uint)(i - 1024)) << 13);
            }

            // Generate the exponent table.
            _exponentTable = new uint[64];

            // 0 => 0
            _exponentTable[0] = 0;

            for (i = 1; i < 63; i++)
            {
                if (i < 31)
                {
                    // Positive numbers.
                    _exponentTable[i] = ((uint)i) << 23;
                }
                else
                {
                    // Negative numbers.
                    _exponentTable[i] = 0x80000000 + (((uint)(i - 32)) << 23);
                }
            }
            _exponentTable[31] = 0x47800000;
            _exponentTable[32] = 0x80000000;
            _exponentTable[63] = 0xC7800000;

            // Generate the offset table.
            _offsetTable = new uint[64];
            _offsetTable[0] = 0;
            for (i = 1; i < 64; i++)
            {
                _offsetTable[i] = 1024;
            }
            _offsetTable[32] = 0;

            // Generate tables for Single to Half conversions.

            //Generate the base and shift tables.
            _baseTable = new ushort[512];
            _shiftTable = new byte[512];
            for (i = 0; i < 256; i++)
            {
                int e = i - 127;
                if (e < -24)
                {
                    // Very small numbers map to zero.
                    _baseTable[i | 0x000] = 0x0000;
                    _baseTable[i | 0x100] = 0x8000;
                    _shiftTable[i | 0x000] = 24;
                    _shiftTable[i | 0x100] = 24;
                }
                else if (e < -14)
                {
                    // Small numbers map to denorms.
                    _baseTable[i | 0x000] = (ushort)(0x0400 >> (-e - 14));
                    _baseTable[i | 0x100] = (ushort)((0x0400 >> (-e - 14)) | 0x8000);
                    _shiftTable[i | 0x000] = (byte)(-e - 1);
                    _shiftTable[i | 0x100] = (byte)(-e - 1);
                }
                else if (e <= 15)
                {
                    // Normal numbers just lose precision.
                    _baseTable[i | 0x000] = (ushort)((e + 15) << 10);
                    _baseTable[i | 0x100] = (ushort)(((e + 15) << 10) | 0x8000);
                    _shiftTable[i | 0x000] = 13;
                    _shiftTable[i | 0x100] = 13;
                }
                else if (e < 128)
                {
                    // Large numbers map to Infinity.
                    _baseTable[i | 0x000] = 0x7C00;
                    _baseTable[i | 0x100] = 0xFC00;
                    _shiftTable[i | 0x000] = 24;
                    _shiftTable[i | 0x100] = 24;
                }
                else
                {
                    // Infinity and NaN's stay Infinity and NaN's.
                    _baseTable[i | 0x000] = 0x7C00;
                    _baseTable[i | 0x100] = 0xFC00;
                    _shiftTable[i | 0x000] = 13;
                    _shiftTable[i | 0x100] = 13;
                }
            }
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the internally stored value to represent the instance.
        /// </summary>
        /// <remarks>Signed to get arithmetic rather than logical shifts.</remarks>
        public ushort Raw { get; private set; }

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the given <see cref="Half"/>.
        /// </summary>
        /// <param name="a">The <see cref="Half"/>.</param>
        /// <returns>The result.</returns>
        public static Half operator +(Half a)
        {
            return a;
        }

        /// <summary>
        /// Adds the first <see cref="Half"/> to the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Half"/>.</param>
        /// <param name="b">The second <see cref="Half"/>.</param>
        /// <returns>The addition result.</returns>
        public static Half operator +(Half a, Half b)
        {
            return (Half)((float)a + (float)b);
        }

        /// <summary>
        /// Negates the given <see cref="Half"/>.
        /// </summary>
        /// <param name="a">The <see cref="Half"/> to negate.</param>
        /// <returns>The negated result.</returns>
        public static Half operator -(Half a)
        {
            return new Half((ushort)(a.Raw ^ 0x8000));
        }

        /// <summary>
        /// Subtracts the first <see cref="Half"/> from the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Half"/>.</param>
        /// <param name="b">The second <see cref="Half"/>.</param>
        /// <returns>The subtraction result.</returns>
        public static Half operator -(Half a, Half b)
        {
            return (Half)((float)a - (float)b);
        }

        /// <summary>
        /// Multiplicates the first <see cref="Half"/> by the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Half"/>.</param>
        /// <param name="b">The second <see cref="Half"/>.</param>
        /// <returns>The multiplication result.</returns>
        public static Half operator *(Half a, Half b)
        {
            return (Half)((float)a * (float)b);
        }

        /// <summary>
        /// Divides the first <see cref="Half"/> through the second one.
        /// </summary>
        /// <param name="a">The first <see cref="Half"/>.</param>
        /// <param name="b">The second <see cref="Half"/>.</param>
        /// <returns>The division result.</returns>
        public static Half operator /(Half a, Half b)
        {
            return (Half)((float)a / (float)b);
        }

        /// <summary>
        /// Gets a value indicating whether the first specified <see cref="Half"/> is the same as the second
        /// specified <see cref="Half"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Half"/> to compare.</param>
        /// <param name="b">The second <see cref="Half"/> to compare.</param>
        /// <returns>true, if both <see cref="Half"/> are the same.</returns>
        public static bool operator ==(Half a, Half b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Gets a value indicating whether the first specified <see cref="Half"/> is not the same as the second
        /// specified <see cref="Half"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Half"/> to compare.</param>
        /// <param name="b">The second <see cref="Half"/> to compare.</param>
        /// <returns>true, if both <see cref="Half"/> are not the same.</returns>
        public static bool operator !=(Half a, Half b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="Half"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="Int32"/> value to represent in the new <see cref="Half"/>
        /// instance.</param>
        public static explicit operator Half(Int32 value)
        {
            return (Half)(float)value;
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="Half"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="Double"/> value to represent in the new <see cref="Half"/>
        /// instance.</param>
        public static explicit operator Half(Double value)
        {
            return (Half)(float)value;
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="Half"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="Single"/> value to represent in the new <see cref="Half"/>
        /// instance.</param>
        public static explicit operator Half(Single value)
        {
            uint uint32 = ((DWord)value).UInt32;
            return new Half((ushort)(_baseTable[(uint32 >> 23) & 0x01FF]
                + ((uint32 & 0x007FFFFF) >> _shiftTable[(uint32 >> 23) & 0x01FF])));
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="Double"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="Half"/> value to represent in the new <see cref="Double"/>
        /// instance.</param>
        public static implicit operator Double(Half value)
        {
            return (float)value;
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="Int32"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="Half"/> value to represent in the new <see cref="Int32"/>
        /// instance.</param>
        public static explicit operator Int32(Half value)
        {
            return (int)(float)value;
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> value to a <see cref="Single"/> instance.
        /// </summary>
        /// <param name="value">The <see cref="Half"/> value to represent in the new <see cref="Single"/>
        /// instance.</param>
        public static implicit operator Single(Half value)
        {
            DWord result = _mantissaTable[_offsetTable[value.Raw >> 10] + (((uint)value.Raw) & 0x03FF)]
                + _exponentTable[value.Raw >> 10];
            return result.Single;
        }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets a value indicating whether this <see cref="Half"/> is the same as the second specified
        /// <see cref="Half"/>.
        /// </summary>
        /// <param name="obj">The object to compare, if it is a <see cref="Half"/>.</param>
        /// <returns>true, if both <see cref="Half"/> are the same.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Half))
            {
                return false;
            }
            Half half = (Half)obj;
            return Equals(half);
        }

        /// <summary>
        /// Gets a hash code as an indication for object equality.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Raw;
        }

        /// <summary>
        /// Gets a string describing this <see cref="Half"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="Half"/>.</returns>
        public override string ToString()
        {
            return ((double)this).ToString();
        }

        /// <summary>
        /// Indicates whether the current <see cref="Half"/> is equal to another <see cref="Half"/>.
        /// </summary>
        /// <param name="other">A <see cref="Half"/> to compare with this <see cref="Half"/>.</param>
        /// <returns>true if the current <see cref="Half"/> is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Half other)
        {
            return Equals(Raw == other.Raw);
        }

    /*    /// <summary>
        /// Returns a value indicating whether the specified number evaluates to not a number (<see cref="NaN"/>).
        /// </summary>
        /// <param name="half">A half-precision floating-point number.</param>
        /// <returns><c>true</c> if value evaluates to not a number (<see cref="NaN"/>); otherwise <c>false</c>.</returns>
        public static bool IsNaN(Half half)
        {
            return (half.Raw & 0x7FFF) > PositiveInfinity.Raw;
        }

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative or positive infinity.
        /// </summary>
        /// <param name="half">A half-precision floating-point number.</param>
        /// <returns><c>true</c> if half evaluates to <see cref="PositiveInfinity"/> or <see cref="NegativeInfinity"/>;
        /// otherwise <c>false</c>.</returns>
        public static bool IsInfinity(Half half)
        {
            return (half.Raw & 0x7FFF) == PositiveInfinity.Raw;
        }

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative infinity.
        /// </summary>
        /// <param name="half">A half-precision floating-point number.</param>
        /// <returns><c>true</c> if half evaluates to <see cref="NegativeInfinity"/>; otherwise <c>false</c>.</returns>
        public static bool IsNegativeInfinity(Half half)
        {
            return half.Raw == NegativeInfinity.Raw;
        }

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to positive infinity.
        /// </summary>
        /// <param name="half">A half-precision floating-point number.</param>
        /// <returns><c>true</c> if half evaluates to <see cref="PositiveInfinity"/>; otherwise <c>false</c>.</returns>
        public static bool IsPositiveInfinity(Half half)
        {
            return half.Raw == PositiveInfinity.Raw;
        }*/
    }
}

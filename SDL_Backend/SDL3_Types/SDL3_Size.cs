using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SDL3
{
	namespace Unsafe
	{
		/// <summary>
		///	Semantic C# wrapper for SDL_/C size_t type values.
		///	
		/// Existing size_t alias is usually used for raw LibraryImport signatures. This wrapper
		/// and interop code should be used when the value carries explicit size_t meaning while
		/// still be convertible back to the ABI-safe nuint representation.
		/// </summary>
		public struct SDLSize : IEquatable<SDLSize>, IComparable<SDLSize>, IComparisonOperators<SDLSize, SDLSize, bool>, IAdditionOperators<SDLSize, SDLSize, SDLSize>,
								ISubtractionOperators<SDLSize, SDLSize, SDLSize>, IMultiplyOperators<SDLSize, SDLSize, SDLSize>, IDivisionOperators<SDLSize, SDLSize, SDLSize>,
								IModulusOperators<SDLSize, SDLSize, SDLSize>
		{
			private size_t value;

			public size_t Value
			{
				get
				{
					return value;
				}
			}

			public static SDLSize Zero
			{
				get
				{
					return new SDLSize(0);
				}
			}

			public static SDLSize One
			{
				get
				{
					return new SDLSize(1);
				}
			}

			public static SDLSize MaxValue
			{
				get
				{
					return new SDLSize(nuint.MaxValue);
				}
			}

			public bool IsZero
			{
				get
				{
					if (value == 0) {
						return true;
					}

					return false;
				}
			}

			public SDLSize(size_t value)
			{
				this.value = value;
			}

			public int CompareTo(SDLSize other)
			{
				return value.CompareTo(other.value);
			}

			public bool Equals(SDLSize other)
			{
				if (value == other.value) {
					return true;
				}

				return false;
			}

			public override bool Equals([NotNullWhen(true)] object? obj)
			{
				if (obj == null || obj.GetType() != typeof(SDLSize)) {
					return false;
				}

				return Equals((SDLSize)obj);
			}

			public override int GetHashCode()
			{
				return value.GetHashCode();
			}

			public override string ToString()
			{
				return value.ToString();
			}

			public int ToInt32Checked()
			{
				return checked((int)value);
			}

			public ulong ToUInt64()
			{
				return value;
			}

			public static bool TryAdd(SDLSize leftOpr, SDLSize rightOpr, out SDLSize result)
			{
				if (leftOpr.value > nuint.MaxValue - rightOpr.value) {
					result = Zero;

					return false;
				}

				result = new SDLSize(leftOpr.value + rightOpr.value);

				return true;
			}

			public static bool TryMultiply(SDLSize leftOpr, SDLSize rightOpr, out SDLSize result)
			{
				if (leftOpr.value == 0 || rightOpr.value == 0) {
					result = Zero;

					return true;
				}

				if (leftOpr.value > nuint.MaxValue / rightOpr.value) {
					result = Zero;

					return false;
				}

				result = new SDLSize(leftOpr.value * rightOpr.value);

				return true;
			}

			public static SDLSize operator +(SDLSize leftOpr, SDLSize rightOpr)
			{
				return new SDLSize(checked(leftOpr.value + rightOpr.value));
			}

			public static SDLSize operator -(SDLSize leftOpr, SDLSize rightOpr)
			{
				return new SDLSize(checked(leftOpr.value - rightOpr.value));
			}

			public static SDLSize operator *(SDLSize leftOpr, SDLSize rightOpr)
			{
				return new SDLSize(checked(leftOpr.value * rightOpr.value));
			}

			public static SDLSize operator /(SDLSize leftOpr, SDLSize rightOpr)
			{
				return new SDLSize(leftOpr.value / rightOpr.value);
			}

			public static SDLSize operator %(SDLSize leftOpr, SDLSize rightOpr)
			{
				return new SDLSize(leftOpr.value % rightOpr.value);
			}

			public void operator +=(SDLSize rightOpr)
			{
				value = checked(value + rightOpr.value);
			}

			public void operator +=(size_t rightOpr)
			{
				value = checked(value + rightOpr);
			}

			public void operator -=(SDLSize rightOpr)
			{
				value = checked(value - rightOpr.value);
			}

			public void operator -=(size_t rightOpr)
			{
				value = checked(value - rightOpr);
			}

			public void operator *=(SDLSize rightOpr)
			{
				value = checked(value * rightOpr.value);
			}

			public void operator *=(size_t rightOpr)
			{
				value = checked(value * rightOpr);
			}

			public void operator /=(SDLSize rightOpr)
			{
				value /= rightOpr.value;
			}

			public void operator /=(size_t rightOpr)
			{
				value /= rightOpr;
			}

			public void operator %=(SDLSize rightOpr)
			{
				value %= rightOpr.value;
			}

			public void operator %=(size_t rightOpr)
			{
				value %= rightOpr;
			}

			public void operator ++()
			{
				value = checked(value + 1);
			}

			public void operator --()
			{
				value = checked(value - 1);
			}

			public static bool operator ==(SDLSize leftOpr, SDLSize rightOpr)
			{
				return leftOpr.Equals(rightOpr);
			}

			public static bool operator !=(SDLSize leftOpr, SDLSize rightOpr)
			{
				return !leftOpr.Equals(rightOpr);
			}

			public static bool operator <(SDLSize leftOpr, SDLSize rightOpr)
			{
				return leftOpr.value < rightOpr.value;
			}

			public static bool operator >(SDLSize leftOpr, SDLSize rightOpr)
			{
				return leftOpr.value > rightOpr.value;	
			}

			public static bool operator <=(SDLSize leftOpr, SDLSize rightOpr)
			{
				return leftOpr.value <= rightOpr.value;
			}

			public static bool operator >=(SDLSize leftOpr, SDLSize rightOpr)
			{
				return leftOpr.value >= rightOpr.value;
			}

			public static implicit operator SDLSize(size_t value)
			{
				return new SDLSize(value);
			}

			public static explicit operator size_t(SDLSize Value)
			{
				return Value.value;
			}

			public static explicit operator int(SDLSize Value)
			{
				return checked((int)Value.value);
			}

			public static explicit operator uint(SDLSize Value)
			{
				return checked((uint)Value.value);
			}

			public static explicit operator ulong(SDLSize Value)
			{
				return Value.value;
			}
		}
	}
}
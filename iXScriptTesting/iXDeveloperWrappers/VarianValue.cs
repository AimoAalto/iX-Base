using Core.Api.DataSource;
using System;
using System.ComponentModel;

namespace Core.Api.DataSource
{
	public enum DataQuality
	{
		Unknown = 0,
		Good = 1,
		Bad = 2
	}
}

namespace Neo.ApplicationFramework.Interfaces
{
#pragma warning disable IDE0060 // Remove unused parameter
	public class VariantValue : object, IConvertible
	{
		#region variables

		object val;

		#endregion

		#region constructors

		public VariantValue(object value) { val = value; Quality = DataQuality.Unknown; }
		public VariantValue(object value, DataQuality quality) { val = value; Quality = quality; }

		#endregion

		#region properties

		public decimal Decimal { get { return val == null ? (ushort)0 : Convert.ToDecimal(val); } }
		public double Double { get { return val == null ? (ushort)0 : Convert.ToDouble(val); } }
		public string String { get { return val == null ? "" : Convert.ToString(val); } }
		public uint UInt { get { return val == null ? (ushort)0 : Convert.ToUInt16(val); } }
		public int Int { get { return val == null ? (int)0 : Convert.ToInt32(val); } }
		public ushort UShort { get { return val == null ? (ushort)0 : Convert.ToUInt16(val); } }
		public short Short { get { return val == null ? (short)0 : Convert.ToInt16(val); } }
		public bool Bool { get { return Convert.ToBoolean(val ?? false); } }
		public DataQuality Quality { get; set; }
		public object Value { get { return val; } set { val = value; Quality = DataQuality.Good; } }
		public object MaxValue { get; }
		public object MinValue { get; }
		public DateTime DateTime { get { return val == null ? new DateTime() : Convert.ToDateTime(val); } }
		[EditorBrowsable]
		public bool IsInteger { get { return val is int || val is uint || val is ushort || val is short; } }
		public void IncrementAnalog(int ival) { val = Convert.ToSingle(val); val = (float)val + (float)ival; }

		#endregion

		#region functions

		public override bool Equals(object instance)
		{
			bool ret;
			Type t = instance.GetType();
			if (t.Equals(typeof(byte))) { ret = (byte)instance == (byte)val; }
			else if (t.Equals(typeof(char))) { ret = (char)(byte)instance == (char)(byte)val; }
			else if (t.Equals(typeof(int))) { ret = (int)instance == (int)Convert.ToInt32(val); }
			else if (t.Equals(typeof(long))) { ret = (long)instance == (long)Convert.ToInt64(val); }
			else if (t.Equals(typeof(float))) { ret = Math.Abs((float)instance - (float)Convert.ToSingle(val)) < 0.0050; }
			else if (t.Equals(typeof(double))) { ret = Math.Abs((double)instance - (double)Convert.ToDouble(val)) < 0.0050; }
			else if (t.Equals(typeof(decimal))) { ret = Math.Abs((decimal)instance - (decimal)Convert.ToDecimal(val)) < (decimal)0.0050; }
			else { ret = instance == val; }
			return ret;
		}
		public bool EqualsWithQuality(object instance) { return Quality == DataQuality.Good ? Equals(instance) : false; }
		public override int GetHashCode() { return 0; }
		public bool IsValueInRange(object value) { return false; }
		public bool IsValueInRange(object value, bool catchException) { return false; }
		[EditorBrowsable]
		public override string ToString() { return val == null ? "" : val.ToString(); }

		public TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		public bool ToBoolean(IFormatProvider provider) { return Convert.ToBoolean(val ?? false); }
		public char ToChar(IFormatProvider provider) { return Convert.ToChar(val ?? (char)0); }
		public sbyte ToSByte(IFormatProvider provider) { return val == null ? (sbyte)0 : Convert.ToSByte(val); }
		public byte ToByte(IFormatProvider provider) { return val == null ? (byte)0 : Convert.ToByte(val); }
		public short ToInt16(IFormatProvider provider) { return val == null ? (short)0 : Convert.ToInt16(val); }
		public ushort ToUInt16(IFormatProvider provider) { return val == null ? (ushort)0 : Convert.ToUInt16(val); }
		public int ToInt32(IFormatProvider provider) { return val == null ? (int)0 : Convert.ToInt32(val); }
		public uint ToUInt32(IFormatProvider provider) { return val == null ? (uint)0 : Convert.ToUInt32(val); }
		public long ToInt64(IFormatProvider provider) { return val == null ? (long)0 : Convert.ToInt64(val); }
		public ulong ToUInt64(IFormatProvider provider) { return val == null ? (ulong)0 : Convert.ToUInt64(val); }
		public float ToSingle(IFormatProvider provider) { return val == null ? (Single)0 : Convert.ToSingle(val); }
		public double ToDouble(IFormatProvider provider) { return val == null ? (double)0 : Convert.ToDouble(val); }
		public decimal ToDecimal(IFormatProvider provider) { return val == null ? (decimal)0 : Convert.ToDecimal(val); }
		public DateTime ToDateTime(IFormatProvider provider) { return val == null ? new DateTime() : Convert.ToDateTime(val); }
		public string ToString(IFormatProvider provider) { return val == null ? "" : Convert.ToString(val); }
		public object ToType(Type conversionType, IFormatProvider provider) { return Convert.ChangeType(val, conversionType); }

		#endregion

		#region operators

		public static VariantValue operator /(VariantValue first, VariantValue second) { return new VariantValue((Convert.ToSingle(first.Value) / Convert.ToSingle(second.Value))); }
		public static VariantValue operator *(VariantValue first, VariantValue second) { return new VariantValue((Convert.ToSingle(first.Value) * Convert.ToSingle(second.Value))); }
		public static VariantValue operator +(VariantValue first, VariantValue second)
		{
			if (first == null || second == null) return null;
			if (first.Value == null || second.Value == null) return null;
			if (first.Value is string || second.Value is string) return first.Value.ToString() + second.Value.ToString();
			if (first.Value is short @short1 && second.Value is short @short2) return @short1 + @short2;
			if (first.Value is int @int1 && second.Value is int @int2) return @int1 + @int2;
			if (first.Value is long @long1 && second.Value is long @long2) return @long1 + @long2;
			return Convert.ToSingle(first.Value) + Convert.ToSingle(second.Value);
		}

		/*
		public static VariantValue operator -(VariantValue first) { throw new Exception("Under Construction"); }
		public static VariantValue operator -(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static VariantValue operator ++(VariantValue first) { throw new Exception("Under Construction"); }
		public static VariantValue operator --(VariantValue first) { throw new Exception("Under Construction"); }
		public static VariantValue operator *(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static VariantValue operator /(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static VariantValue operator &(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static VariantValue operator |(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static VariantValue operator ^(VariantValue first, VariantValue second { throw new Exception("Under Construction"); }
		public static VariantValue operator <<(VariantValue first, int second) { throw new Exception("Under Construction"); }
		public static VariantValue operator >>(VariantValue first, int second) { throw new Exception("Under Construction"); }
		public static bool operator ==(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static bool operator !=(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static VariantValue operator <(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		public static VariantValue operator >(VariantValue first, VariantValue second) { throw new Exception("Under Construction"); }
		*/
		#endregion

		#region check

		//
		// Summary:
		//     Not executed, since implicit bool casting operator is implemented, but needed
		//     by compiler for evaluation of binary expressions, e.g. AND, OR.
		//
		// Parameters:
		//   variantValue:
		//     Value to check if true.
		//
		// Returns:
		//     True if value is true.
		public static bool operator true(VariantValue variantValue) { return false; }
		//
		// Summary:
		//     Not executed, since implicit bool casting operator is implemented, but needed
		//     by compiler for evaluation of binary expressions, e.g. AND, OR.
		//
		// Parameters:
		//   variantValue:
		//     Value to check if false.
		//
		// Returns:
		//     True if value is false.
		public static bool operator false(VariantValue variantValue) { return false; }

		public static implicit operator VariantValue(double value) { return new VariantValue(value); }
		public static implicit operator string(VariantValue value) { return value == null ? string.Empty : Convert.ToString(value.Value); }
		public static implicit operator VariantValue(string value) { return new VariantValue(value); }
		public static implicit operator double(VariantValue value) { return value.Value == null ? (double)0 : Convert.ToDouble(value.Value); }
		public static implicit operator VariantValue(short value) { return new VariantValue(value); }
		public static implicit operator VariantValue(float value) { return new VariantValue(value); }
		public static implicit operator VariantValue(bool value) { return new VariantValue(value); }
		public static implicit operator DateTime(VariantValue value) { return (DateTime)value.Value == null ? new DateTime() : Convert.ToDateTime(value.Value); }
		public static implicit operator VariantValue(DateTime value) { return new VariantValue(value); }
		public static implicit operator uint(VariantValue value) { return value.Value == null ? (uint)0 : Convert.ToUInt32(value.Value); }
		public static implicit operator VariantValue(uint value) { return new VariantValue(value); }
		public static implicit operator ushort(VariantValue value) { return value.Value == null ? (ushort)0 : Convert.ToUInt16(value.Value); }
		public static implicit operator VariantValue(ushort value) { return new VariantValue(value); }
		public static implicit operator long(VariantValue value) { return value.Value == null ? (long)0 : Convert.ToInt64(value.Value); }
		public static implicit operator VariantValue(long value) { return new VariantValue(value); }
		public static implicit operator int(VariantValue value) { return value.Value == null ? (int)0 : Convert.ToInt32(value.Value); }
		public static implicit operator VariantValue(int value) { return new VariantValue(value); }
		public static implicit operator short(VariantValue value) { return value.Value == null ? (short)0 : Convert.ToInt16(value.Value); }
		public static implicit operator float(VariantValue value) { return value.Value == null ? 0 : (float)Convert.ToDouble(value.Value); }
		public static implicit operator bool(VariantValue value)
		{
			if (value == null) return false;
			if (value.Value == null) return false;
			if (value.Value is string @string) return (@string).Trim().ToLower().CompareTo("true") == 0;
			return Convert.ToBoolean(value.Value);
		}

		#endregion
	}
#pragma warning restore IDE0060 // Restore unused parameter
}
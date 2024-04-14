using Core.Api.DataSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neo.ApplicationFramework.Interfaces.Tests
{
	[TestClass()]
	public class VariantValueTests
	{
		[TestMethod()]
		public void VariantValueTest()
		{
			VariantValue v1 = new VariantValue(1);
			Assert.AreEqual(1, v1.Value, "VariantValue(1)");
			Assert.AreEqual(DataQuality.Unknown, v1.Quality, "VariantValue(1)");

			VariantValue v2 = new VariantValue("Testing...", DataQuality.Good);
			Assert.AreEqual("Testing...", v2.Value, "VariantValue(Testing...)");
			Assert.AreEqual(DataQuality.Good, v2.Quality, "VariantValue(Testing...)");
		}

		[TestMethod()]
		public void EqualsTest()
		{
			VariantValue v = new VariantValue((byte)'c');

			//v.Value = (byte)'c';
			Assert.AreEqual((byte)'c', v.Value, "VariantValue() - byte");
			Assert.AreEqual('c', (char)(byte)v.Value, "VariantValue() - byte");
			Assert.AreEqual(true, v.Equals((byte)'c'), "VariantValue().Equals - byte");

			v.Value = (short)-1;
			Assert.AreEqual((short)-1, v.Value, "VariantValue() - short");
			Assert.AreEqual(true, v.Equals(-1), "VariantValue().Equals - short");

			v.Value = (int)-100000;
			Assert.AreEqual(-100000, v.Value, "VariantValue() - int");
			Assert.AreEqual(true, v.Equals(-100000), "VariantValue().Equals - int");

			v.Value = (long)-100000000000;
			Assert.AreEqual(-100000000000, v.Value, "VariantValue() - long");
			Assert.AreEqual(true, v.Equals(-100000000000), "VariantValue().Equals - long");

			v.Value = (ushort)2;
			Assert.AreEqual((ushort)2, v.Value, "VariantValue() - ushort");
			Assert.AreEqual(true, v.Equals(2), "VariantValue().Equals - ushort");

			v.Value = (uint)100000;
			Assert.AreEqual((uint)100000, v.Value, "VariantValue() - uint");
			Assert.AreEqual(true, v.Equals(100000), "VariantValue().Equals - uint");

			v.Value = (ulong)100000000000;
			Assert.AreEqual((ulong)100000000000, v.Value, "VariantValue() - ulong");
			Assert.AreEqual(true, v.Equals(100000000000), "VariantValue().Equals - ulong");

			v.Value = (float)-1.123;
			Assert.AreEqual((float)-1.123, v.Value, "VariantValue() - float");
			Assert.AreEqual(true, v.Equals(-1.123), "VariantValue().Equals - float");

			v.Value = (double)-100000.123;
			Assert.AreEqual((double)-100000.123, v.Value, "VariantValue() - double");
			Assert.AreEqual(true, v.Equals(-100000.123), "VariantValue().Equals - double");

			v.Value = (decimal)-100000000000.123;
			Assert.AreEqual((decimal)-100000000000.123, v.Value, "VariantValue() - decimal");
			Assert.AreEqual(true, v.Equals(-100000000000.123), "VariantValue().Equals - decimal");

			v.Value = "Testing...";
			Assert.AreEqual("Testing...", v.Value, "VariantValue() - string");
			Assert.AreEqual(true, v.Equals("Testing..."), "VariantValue().Equals - string");
		}

		[TestMethod()]
		public void EqualsWithQualityTest()
		{
			VariantValue v = new VariantValue((byte)'c');
			Assert.AreEqual(false, v.EqualsWithQuality('c'), "Quality.Bad - byte");

			v.Value = (byte)'c'; // sets Quality ==> Good
			Assert.AreEqual(true, v.EqualsWithQuality((byte)'c'), "Quality.Good - byte");
		}

		[TestMethod()]
		public void ToStringTest()
		{
			VariantValue v = new VariantValue((byte)'c');

			//v.Value = (byte)'c';
			Assert.AreEqual("99", v.Value.ToString(), "VariantValue() - byte");

			v.Value = (short)-1;
			Assert.AreEqual("-1", v.Value.ToString(), "VariantValue() - short");

			v.Value = (int)-100000;
			Assert.AreEqual("-100000", v.Value.ToString(), "VariantValue() - int");

			v.Value = (long)-100000000000;
			Assert.AreEqual("-100000000000", v.Value.ToString(), "VariantValue() - long");

			v.Value = (ushort)1;
			Assert.AreEqual("1", v.Value.ToString(), "VariantValue() - ushort");

			v.Value = (uint)100000;
			Assert.AreEqual("100000", v.Value.ToString(), "VariantValue() - uint");

			v.Value = (ulong)100000000000;
			Assert.AreEqual("100000000000", v.Value.ToString(), "VariantValue() - ulong");

			v.Value = (float)-1.123;
			Assert.AreEqual("-1,123", v.Value.ToString(), "VariantValue() - float");

			v.Value = (double)-100000.123;
			Assert.AreEqual("-100000,123", v.Value.ToString(), "VariantValue() - double");

			v.Value = (decimal)-100000000000.123;
			Assert.AreEqual("-100000000000,123", v.Value.ToString(), "VariantValue() - decimal");

			v.Value = "Testing...";
			Assert.AreEqual("Testing...", v.Value.ToString(), "VariantValue() - string");
		}

		[TestMethod()]
		public void SumsTest()
		{
			VariantValue v1 = new VariantValue(1);
			VariantValue v2 = new VariantValue(2);

			v1.Value = v2.Value;
			Assert.AreEqual(v1.Value, v2.Value, "Sums() - set");

			v1.Value = 0;
			v1 += 3;
			Assert.AreEqual(true, v1.Equals(3), "Sums() - 3 : " + v1.Value);

			v1 = 0;
			v1 += 3.123;
			Assert.AreEqual(true, v1.Equals(3.123), "Sums() - 3.123 : " + v1.Value);

			v1 = 0;
			v1 -= 3.123;
			Assert.AreEqual(true, v1.Equals(-3.123), "Sums() - -3.123 : " + v1.Value);

			v1 = 1;
			v1 *= 3.123;
			Assert.AreEqual(true, v1.Equals(3.123), "Sums() - 3.123 : " + v1.Value);

			v1 = 10;
			v1 /= 4;
			Assert.AreEqual(true, v1.Equals(2.5), "Sums() - 2.5 : " + v1.Value);

			v1 = 0;
			v1++;
			Assert.AreEqual((short)1, v1.Value, "++ - set");

			v1--;
			Assert.AreEqual((short)0, v1.Value, "-- - set");

			v1 = "New";
			v2 = "String";

			v1 += v2;
			Assert.AreEqual("NewString", v1.Value, "Concatenate - set");
		}
	}
}
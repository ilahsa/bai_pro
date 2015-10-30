/*
 * IICAssert：
 *		Dump from class Assert in Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll
 *		
 * and change something		
 * 
 * for Common use, only in Debug Version
 * 
 * Gao Lei 2010-06-11
 * 
 * == Word Cup 2010 ==
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class IICAssert
	{
		[Conditional("DEBUG")]
		public static void AreEqual(object expected, object actual)
		{
			AreEqual(expected, actual, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual<T>(T expected, T actual)
		{
			AreEqual<T>(expected, actual, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(double expected, double actual, double delta)
		{
			AreEqual(expected, actual, delta, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(object expected, object actual, string message)
		{
			AreEqual(expected, actual, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(float expected, float actual, float delta)
		{
			AreEqual(expected, actual, delta, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual<T>(T expected, T actual, string message)
		{
			AreEqual<T>(expected, actual, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(string expected, string actual, bool ignoreCase)
		{
			AreEqual(expected, actual, ignoreCase, string.Empty, (object[])null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(double expected, double actual, double delta, string message)
		{
			AreEqual(expected, actual, delta, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(object expected, object actual, string message, params object[] parameters)
		{
			AreEqual<object>(expected, actual, message, parameters);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(float expected, float actual, float delta, string message)
		{
			AreEqual(expected, actual, delta, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual<T>(T expected, T actual, string message, params object[] parameters)
		{
			if (!object.Equals(expected, actual)) {
				string str = TracingHelper.FormatMessage("Assert.AreEqual Excepted:{0} != Actual: {1} ", expected, actual);
				HandleFail(str, message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture)
		{
			AreEqual(expected, actual, ignoreCase, culture, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(string expected, string actual, bool ignoreCase, string message)
		{
			AreEqual(expected, actual, ignoreCase, message, (object[])null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(double expected, double actual, double delta, string message, params object[] parameters)
		{
			if (Math.Abs((double)(expected - actual)) > delta) {
				string str = TracingHelper.FormatMessage("Assert.AreEqual Excepted:{0} - Actual:{1} > delta: {2}", expected, actual, delta);
				HandleFail(str, message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void AreEqual(float expected, float actual, float delta, string message, params object[] parameters)
		{
			if (Math.Abs((float)(expected - actual)) > delta) {
				string str = TracingHelper.FormatMessage("Assert.AreEqual Excepted:{0} - Actual:{1} > delta: {2}", expected, actual, delta);
				HandleFail(str, message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message)
		{
			AreEqual(expected, actual, ignoreCase, culture, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
		{
			AreEqual(expected, actual, ignoreCase, CultureInfo.InvariantCulture, message, parameters);
		}

		[Conditional("DEBUG")]
		public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
		{
			// CheckParameterNotNull(culture, "Assert.AreEqual", "culture", string.Empty, new object[0]);
			if (string.Compare(expected, actual, ignoreCase, culture) != 0) {
				string str = TracingHelper.FormatMessage("Assert.AreEqual Excepted:{0} != Actual: {1} {2}Culture: {3}", expected, actual, ignoreCase ? "IgnoreCase " : "", culture);
				HandleFail(str, message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual<T>(T notExpected, T actual)
		{
			AreNotEqual<T>(notExpected, actual, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(object notExpected, object actual)
		{
			AreNotEqual(notExpected, actual, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(double notExpected, double actual, double delta)
		{
			AreNotEqual(notExpected, actual, delta, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(object notExpected, object actual, string message)
		{
			AreNotEqual(notExpected, actual, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual<T>(T notExpected, T actual, string message)
		{
			AreNotEqual<T>(notExpected, actual, message, null);
		}

		public static void AreNotEqual(float notExpected, float actual, float delta)
		{
			AreNotEqual(notExpected, actual, delta, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(string notExpected, string actual, bool ignoreCase)
		{
			AreNotEqual(notExpected, actual, ignoreCase, string.Empty, (object[])null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(double notExpected, double actual, double delta, string message)
		{
			AreNotEqual(notExpected, actual, delta, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(float notExpected, float actual, float delta, string message)
		{
			AreNotEqual(notExpected, actual, delta, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message)
		{
			AreNotEqual(notExpected, actual, ignoreCase, message, (object[])null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual<T>(T notExpected, T actual, string message, params object[] parameters)
		{
			if (object.Equals(notExpected, actual)) {
				string str = TracingHelper.FormatMessage("Assert.AreEqual NotExcepted:{0} = Actual:{1}", notExpected, actual);
				HandleFail(str, message, parameters);			
			}
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(object notExpected, object actual, string message, params object[] parameters)
		{
			AreNotEqual<object>(notExpected, actual, message, parameters);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture)
		{
			AreNotEqual(notExpected, actual, ignoreCase, culture, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(double notExpected, double actual, double delta, string message, params object[] parameters)
		{
			if (Math.Abs((double)(notExpected - actual)) <= delta) {
				string str = TracingHelper.FormatMessage("Assert.AreEqual NotExcepted:{0} - Actual:{1} < Delta: {2}", notExpected, actual, delta);
				HandleFail(str, message, parameters);			
			}
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(float notExpected, float actual, float delta, string message, params object[] parameters)
		{
			if (Math.Abs((float)(notExpected - actual)) <= delta) {
				string str = TracingHelper.FormatMessage("Assert.AreEqual NotExcepted:{0} - Actual:{1} < Delta: {2}", notExpected, actual, delta);
				HandleFail(str, message, parameters);			
			}
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message)
		{
			AreNotEqual(notExpected, actual, ignoreCase, culture, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters)
		{
			AreNotEqual(notExpected, actual, ignoreCase, CultureInfo.InvariantCulture, message, parameters);
		}

		[Conditional("DEBUG")]
		public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
		{
			// CheckParameterNotNull(culture, "Assert.AreNotEqual", "culture", string.Empty, new object[0]);
			if (string.Compare(notExpected, actual, ignoreCase, culture) == 0) {
				string str = TracingHelper.FormatMessage("Assert.AreNotEqual Excepted:{0} != Actual: {1} {2}Culture: {3}", notExpected, actual, ignoreCase ? "IgnoreCase " : "", culture);
				HandleFail(str, message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void AreNotSame(object notExpected, object actual)
		{
			AreNotSame(notExpected, actual, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotSame(object notExpected, object actual, string message)
		{
			AreNotSame(notExpected, actual, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreNotSame(object notExpected, object actual, string message, params object[] parameters)
		{
			if (object.ReferenceEquals(notExpected, actual)) {
				string str = TracingHelper.FormatMessage("Assert.AreNotSame NotExcepted:{0} = Actual:{1}", notExpected, actual);
				HandleFail(str, message, parameters);			
			}
		}

		[Conditional("DEBUG")]
		public static void AreSame(object expected, object actual)
		{
			AreSame(expected, actual, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void AreSame(object expected, object actual, string message)
		{
			AreSame(expected, actual, message, null);
		}

		[Conditional("DEBUG")]
		public static void AreSame(object expected, object actual, string message, params object[] parameters)
		{
			if (!object.ReferenceEquals(expected, actual)) {
				string str = TracingHelper.FormatMessage("Assert.AreSame NotExcepted:{0} != Actual:{1}", expected, actual);
				HandleFail(str, message, parameters);			
			}
		}

		[Conditional("DEBUG")]
		public static void Fail()
		{
			Fail(string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void Fail(string message)
		{
			Fail(message, null);
		}

		[Conditional("DEBUG")]
		public static void Fail(string message, params object[] parameters)
		{
			HandleFail("Assert.Fail", message, parameters);
		}

		[Conditional("DEBUG")]
		public static void IsFalse(bool condition)
		{
			IsFalse(condition, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void IsFalse(bool condition, string message)
		{
			IsFalse(condition, message, null);
		}

		[Conditional("DEBUG")]
		public static void IsFalse(bool condition, string message, params object[] parameters)
		{
			if (condition) {
				HandleFail("Assert.IsFalse", message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void IsInstanceOfType(object value, Type expectedType)
		{
			IsInstanceOfType(value, expectedType, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void IsInstanceOfType(object value, Type expectedType, string message)
		{
			IsInstanceOfType(value, expectedType, message, null);
		}

		[Conditional("DEBUG")]
		public static void IsInstanceOfType(object value, Type expectedType, string message, params object[] parameters)
		{
			if (expectedType == null) {
				HandleFail("Assert.IsInstanceOfType expectedType == null", message, parameters);
			}
			if (!expectedType.IsInstanceOfType(value)) {
				string str = TracingHelper.FormatMessage("Assert.IsInstanceOfType {0} is not {1}", value, expectedType);
				HandleFail(str, message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void IsNotInstanceOfType(object value, Type wrongType)
		{
			IsNotInstanceOfType(value, wrongType, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void IsNotInstanceOfType(object value, Type wrongType, string message)
		{
			IsNotInstanceOfType(value, wrongType, message, null);
		}

		[Conditional("DEBUG")]
		public static void IsNotInstanceOfType(object value, Type wrongType, string message, params object[] parameters)
		{
			if (wrongType == null) {
				HandleFail("Assert.IsNotInstanceOfType expectedType == null", message, parameters);
			}
			if ((value != null) && wrongType.IsInstanceOfType(value)) {
				string str = TracingHelper.FormatMessage("Assert.IsNotInstanceOfType {0} is {1}", value, wrongType);
				HandleFail(str, message, parameters);
			}
		}
		[Conditional("DEBUG")]
		public static void IsNotNull(object value)
		{
			IsNotNull(value, string.Empty, null);
		}
		[Conditional("DEBUG")]
		public static void IsNotNull(object value, string message)
		{
			IsNotNull(value, message, null);
		}
		[Conditional("DEBUG")]
		public static void IsNotNull(object value, string message, params object[] parameters)
		{
			if (value == null) {
				HandleFail("Assert.IsNotNull", message, parameters);
			}
		}
		[Conditional("DEBUG")]
		public static void IsNull(object value)
		{
			IsNull(value, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void IsNull(object value, string message)
		{
			IsNull(value, message, null);
		}

		[Conditional("DEBUG")]
		public static void IsNull(object value, string message, params object[] parameters)
		{
			if (value != null) {
				HandleFail("Assert.IsNull", message, parameters);
			}
		}

		[Conditional("DEBUG")]
		public static void IsTrue(bool condition)
		{
			IsTrue(condition, string.Empty, null);
		}

		[Conditional("DEBUG")]
		public static void IsTrue(bool condition, string message)
		{
			IsTrue(condition, message, null);
		}
		[Conditional("DEBUG")]
		public static void IsTrue(bool condition, string message, params object[] parameters)
		{
			if (!condition) {
				HandleFail("Assert.IsTrue", message, parameters);
			}
		}

		[Conditional("DEBUG")]
		internal static void HandleFail(string info, string extra, params object[] args)
		{
			string s = TracingHelper.FormatMessage(extra, args);
			throw new IICAssertFailedException("Failed! " + info + " \'" + s + "\'");
		}
	}
}

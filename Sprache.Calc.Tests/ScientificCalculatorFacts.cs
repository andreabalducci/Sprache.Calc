﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sprache.Calc.Tests
{
	public class ScientificCalculatorFacts
	{
		private void ForEachCalculator(Action<ScientificCalculator> fact)
		{
			foreach (var calc in new[] { new ScientificCalculator(), new XtensibleCalculator() })
			{
				fact(calc);
			}
		}

		[Fact]
		public void HexadecimalNumbersStartWith0x()
		{
			ForEachCalculator(calc =>
			{
				Assert.Equal("abc", calc.Hexadecimal.Parse(" 0xabc "));
				Assert.Equal("123", calc.Hexadecimal.Parse("0X123"));
				Assert.Equal("CAFEBABE", calc.Hexadecimal.Parse("0xCAFEBABE"));
				Assert.Throws<ParseException>(() => calc.Hexadecimal.Parse("0xy"));
			});
		}

		[Fact]
		public void LongHexadecimalNumbersAreSupported()
		{
			ForEachCalculator(calc =>
			{
				Assert.Equal(0xABCul, calc.ConvertHexadecimal("abc"));
				Assert.Equal(0x123ul, calc.ConvertHexadecimal("123"));
				Assert.Equal(0xCAFEBABEul, calc.ConvertHexadecimal("CAFEBABE"));
				Assert.Throws<ParseException>(() => calc.ConvertHexadecimal("y"));
			});
		}

		[Fact]
		public void ConstantCanBeDecimalOrHexadecimal()
		{
			ForEachCalculator(calc =>
			{
				Assert.Equal((double)0xABC, calc.Constant.Parse("0xabc").Execute());
				Assert.Equal((double)0x123ul, calc.Constant.Parse(" 0X123").Execute());
				Assert.Equal((double)123, calc.Constant.Parse("123 ").Execute());
				Assert.Equal(010110100d, calc.Constant.Parse("010110100").Execute());
				Assert.Throws<ParseException>(() => calc.Constant.End().Parse("0x"));
			});
		}

		[Fact]
		public void IdentifierIsAWordStartingWithLetter()
		{
			ForEachCalculator(calc =>
			{
				Assert.Equal("abc", calc.Identifier.Parse(" abc "));
				Assert.Equal("Test123", calc.Identifier.Parse("Test123"));
				Assert.Throws<ParseException>(() => calc.Identifier.Parse("1"));
			});
		}

		[Fact]
		public void FunctionCallIsAnIdentifierFollowedByArgumentsInParentheses()
		{
			ForEachCalculator(calc =>
			{
				Assert.Equal(0d, calc.FunctionCall.Parse("Sin(0x0)").Execute());
				Assert.Equal(1d, calc.FunctionCall.Parse("Cos(0X0)").Execute());
				Assert.Equal(2.71828d, calc.FunctionCall.Parse("Min(3.14159, 2.71828)").Execute());
				Assert.Throws<ParseException>(() => calc.FunctionCall.Parse("(3)"));
			});
		}

		[Fact]
		public void ScientificCalculatorSupportsArithmeticOperationsIntermixedWithFunctions()
		{
			ForEachCalculator(calc =>
			{
				Assert.Equal(1d, calc.ParseExpression("Sin(3.141592653589793238462643383279/2)").Compile()());
				Assert.Equal(6, (int)calc.ParseExpression("Log10(1234567)^(Log(2.718281828459045)-Cos(3.141592653589793238462643383279/2))").Compile()());
				Assert.Throws<ParseException>(() => calc.ParseExpression("Log10()"));
			});
		}
	}
}

﻿using System;
using System.Diagnostics;
using Common.Shared.Min.Extensions;
using SramCommons.Models;
using SramComparer.Helpers;
using SramComparer.Properties;

namespace SramComparer.Services
{
	/// <summary>
	/// Base class for SRAM comparison. Needs an actual implementation for a specific game
	/// </summary>
	/// <typeparam name="TSramFile">The SRAM file structure</typeparam>
	/// <typeparam name="TSramGame">The SRAM game structure</typeparam>
	public abstract class SramComparerBase<TSramFile, TSramGame> : ISramComparer<TSramFile, TSramGame>
		where TSramFile : SramFile, ISramFile<TSramGame>
		where TSramGame : struct
	{
		protected IConsolePrinter ConsolePrinter { get; }

		protected SramComparerBase() : this(ServiceCollection.ConsolePrinter) { }
		protected SramComparerBase(IConsolePrinter consolePrinter) => ConsolePrinter = consolePrinter;

		/// <summary>Compares all games of SRAM structure</summary>
		/// <param name="currFile">The current SRAM file structure</param>
		/// <param name="compFile">The comparison SRAM file structure</param>
		/// <param name="options">The options to be used for all comparisons</param>
		/// <returns>Number of compared bytes changed</returns>
		public abstract int CompareSram(TSramFile currFile, TSramFile compFile, IOptions options);

		/// <summary>Compares all games of SRAM structure</summary>
		/// <param name="currGame">The current SRAM game structure</param>
		/// <param name="compGame">The comparison SRAM game structure</param>
		/// <param name="options">The options to be used for all comparisons</param>
		/// <returns>Number of compared bytes changed</returns>
		public abstract int CompareGame(TSramGame currGame, TSramGame compGame, IOptions options);

		/// <summary>
		/// Compares a single byte
		/// </summary>
		/// <param name="bufferName">The name of the compared buffer</param>
		/// <param name="bufferOffset">The buffer's offset at this byte is located</param>
		/// <param name="currValue">The current byte to be compared</param>
		/// <param name="compValue">The comparison byte to be compared</param>
		/// <param name="writeToConsole">Sets if any output should be written to console. Default is true</param>
		/// <returns>1 if the byte has changed, otherwise 0</returns>
		protected virtual int CompareByte(string bufferName, int bufferOffset, byte currValue, byte compValue, bool writeToConsole = true)
		{
			if (Equals(compValue, currValue)) return 0;

			var byteCount = BitConverter.GetBytes(currValue).Length;

			if (!writeToConsole) return byteCount;

			OnPrintBufferInfo(bufferName, bufferOffset, 2);
			OnPrintComparison(0, null, currValue, compValue);
			OnStatusBytesChanged(byteCount);

			return byteCount;
		}

		/// <summary>
		/// Compares a single 2-byte value (UShort)
		/// </summary>
		/// <param name="bufferName">The name of the compared buffer</param>
		/// <param name="bufferOffset">The buffer's offset at this ushort is located</param>
		/// <param name="currValue">The current ushort to be compared</param>
		/// <param name="compValue">The comparison ushort to be compared</param>
		/// <param name="writeToConsole">Sets if any output should be written to console. Default is true</param>
		/// <returns>2 if the ushort changed, otherwise 0</returns>
		protected virtual int CompareUShort(string bufferName, int bufferOffset, ushort currValue, ushort compValue, bool writeToConsole = true)
		{
			if (Equals(compValue, currValue)) return 0;

			var byteCount = BitConverter.GetBytes(currValue).Length;

			if (!writeToConsole) return byteCount;

			ConsoleHelper.EnsureMinConsoleWidth(175);
			OnPrintBufferInfo(bufferName, bufferOffset, 2);
			OnPrintComparison(0, null, currValue, compValue);
			OnStatusBytesChanged(byteCount);

			return byteCount;
		}

		/// <summary>
		/// Compares a single 2-byte value (UShort)
		/// </summary>
		/// <param name="bufferName">The name of the compared buffer</param>
		/// <param name="bufferOffset">The buffer's offset at this ushort is located</param>
		/// <param name="currValues">The current buffer's bytes to be compared</param>
		/// <param name="compValues">The comparison buffer's bytes to be compared</param>
		/// <param name="writeToConsole">Sets if any output should be written to console. Default is true</param>
		/// <param name="offsetNameCallback">An optional callback function from which the name of a specific offset can be returned</param>
		/// <returns>The amound of bytes changed</returns>
		protected virtual int CompareByteArray(string bufferName, int bufferOffset, ReadOnlySpan<byte> currValues, ReadOnlySpan<byte> compValues, bool writeToConsole = true, Func<int, string?>? offsetNameCallback = null)
		{
			var byteCount = 0;

			Debug.Assert(currValues.Length == compValues.Length);

			for (var offset = 0; offset < currValues.Length; offset++)
			{
				var currValue = currValues[offset];
				var compValue = compValues[offset];

				if (currValue == compValue) continue;

				if (byteCount == 0 && writeToConsole)
					OnPrintBufferInfo(bufferName, bufferOffset, compValues.Length);

				++byteCount;

				if (!writeToConsole) continue;

				string? offsetName = null;
				var tempOffsetName = offsetNameCallback?.Invoke(offset);
				if (tempOffsetName is not null)
					offsetName += $"{tempOffsetName}".PadRight(28);

				OnPrintComparison(offset, offsetName, currValue, compValue);
			}

			if (byteCount == 0 || !writeToConsole) return byteCount;

			OnStatusBytesChanged(byteCount);

			return byteCount;
		}

		protected virtual void OnPrintComparison(int offset, string? offsetName, ushort currValue, ushort compValue) => ConsolePrinter.PrintComparison(" ".Repeat(6), offset, offsetName, currValue, compValue);

		protected virtual void OnPrintComparison(int offset, string? offsetName, byte currValue, byte compValue) => ConsolePrinter.PrintComparison(" ".Repeat(6), offset, offsetName, currValue, compValue);

		protected virtual void OnPrintBufferInfo(string bufferName, int bufferOffset, int byteCount) => ConsolePrinter.PrintBufferInfo(bufferName, bufferOffset, byteCount);

		protected virtual void OnStatusBytesChanged(int byteCount)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(" ".Repeat(6) + Resources.StatusBytesChangedTemplate, byteCount);
			Console.ResetColor();
		}
	}
}
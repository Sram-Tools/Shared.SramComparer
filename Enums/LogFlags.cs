﻿using System;
using System.Text.Json.Serialization;
using Common.Shared.Min.Attributes;
using SRAM.Comparison.Helpers;
using SRAM.Comparison.Properties;

namespace SRAM.Comparison.Enums
{
	[DisplayNameLocalized(nameof(Resources.EnumFileWatchFlags), typeof(Resources))]
	[JsonConverter(typeof(JsonStringEnumObjectConverter))]
	[Flags]
	public enum LogFlags : uint
	{
		[DisplayNameLocalized(nameof(Resources.EnumLogExport), typeof(Resources))]
		Export = 0x1,
		[DisplayNameLocalized(nameof(Resources.EnumLogComparison), typeof(Resources))]
		Comparison = 0x2
	}
}
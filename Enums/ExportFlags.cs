﻿using System;
using Common.Shared.Min.Attributes;
using Res = SramComparer.Properties.Resources;

namespace SramComparer.Enums
{
	[Flags]
	public enum ExportFlags 
	{
		[DisplayNameLocalized(nameof(Res.EnumOpenExportFile), typeof(Res))]
		OpenFile = 0x1,

		[DisplayNameLocalized(nameof(Res.EnumSelectExportFile), typeof(Res))]
		SelectFile = 0x2,

		[DisplayNameLocalized(nameof(Res.EnumPromptExportFile), typeof(Res))]
		PromptName = 0x4,

		[DisplayNameLocalized(nameof(Res.EnumOverwriteCompFile), typeof(Res))]
		OverwriteComp = 0x8,

		[DisplayNameLocalized(nameof(Res.EnumDeleteCompFile), typeof(Res))]
		DeleteComp = 0x10,

		[DisplayNameLocalized(nameof(Res.EnumAppendToAppLogFile), typeof(Res))]
		AppendLog = 0x20
	}
}
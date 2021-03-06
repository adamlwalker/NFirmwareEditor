﻿using System;
using System.Collections.Generic;
using System.IO;
using NFirmware;
using NFirmwareEditor.Core;
using NLog;

namespace NFirmwareEditor.Storages
{
	internal class FirmwareDefinitionsStorage : IFileStorage<FirmwareDefinition>
	{
		private static readonly ILogger s_logger = LogManager.GetCurrentClassLogger();

		#region Implementation of IStorage
		public void Initialize()
		{
			var initEx = Safe.Execute(() => Paths.EnsureDirectoryExists(Paths.DefinitionsDirectory));
			if (initEx == null) return;

			s_logger.Warn(initEx, "An error occured during creating definitions directory '{0}'.", Paths.DefinitionsDirectory);
		}
		#endregion

		#region Implementation of IFileStorage<out FirmwareDefinition>
		public FirmwareDefinition TryLoad(string filePath)
		{
			if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

			try
			{
				using (var fs = File.OpenRead(filePath))
				{
					return Serializer.Read<FirmwareDefinition>(fs);
				}
			}
			catch (Exception ex)
			{
				s_logger.Warn(ex, "An error occured during reading definition file '{0}'.", filePath);
				return null;
			}
		}

		public IEnumerable<FirmwareDefinition> LoadAll()
		{
			var result = new List<FirmwareDefinition>();
			var files = Directory.GetFiles(Paths.DefinitionsDirectory, Consts.DefinitionFileExtension, SearchOption.AllDirectories);
			foreach (var filePath in files)
			{
				var definition = TryLoad(filePath);
				if (definition != null) result.Add(definition);
			}
			return result;
		}
		#endregion
	}
}

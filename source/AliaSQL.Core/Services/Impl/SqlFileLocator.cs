using System;
using System.Collections.Generic;
using System.IO;
using AliaSQL.Core;

namespace AliaSQL.Core.Services.Impl
{
	
	public class SqlFileLocator : ISqlFileLocator
	{
		private IFileSystem _fileSystem;

		public SqlFileLocator(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

	    public SqlFileLocator():this(new FileSystem())
	    {
	        
	    }

	    public string[] GetSqlFilenames(string scriptBaseFolder, string scriptFolder)
		{
			List<string> list = new List<string>();

			string folder = Path.Combine(scriptBaseFolder, scriptFolder);
			string[] sqlFiles = _fileSystem.GetAllFilesWithExtensionWithinFolder(folder, "sql");

			foreach (string sqlFilename in sqlFiles)
			{
				list.Add(sqlFilename);
			}
            list.Sort(Comparison);
		    return list.ToArray();
		}

	    private int Comparison(string x, string y)
	    {
	        return x.CompareTo(y);
	    }
	}
}
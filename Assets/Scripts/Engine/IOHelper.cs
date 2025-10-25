using System;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>Helper class for IO operations that C# can't do natively</summary>
public static class IOHelper
{
	[DllImport("shell32.dll", CharSet = CharSet.Auto)]
	private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOP);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct SHFILEOPSTRUCT
	{
		public IntPtr hwnd;
		public FileOperationType wFunc;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string pFrom;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string pTo;
		public FileOperationFlags fFlags;
		public bool fAnyOperationsAborted;
		public IntPtr hNameMappings;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpszProgressTitle;
	}

	[Flags]
	private enum FileOperationFlags : ushort
	{
		FOF_ALLOWUNDO = 0x0040, // move to recycle
		FOF_NOCONFIRMATION = 0x0010, // no confirmation dialogue
		FOF_SILENT = 0x0004, // no progression dialogue
		FOF_NOERRORUI = 0x0400 // no error UI
	}

	private enum FileOperationType : uint
	{
		FO_MOVE = 0x0001,
		FO_COPY = 0x0002,
		FO_DELETE = 0x0003,
		FO_RENAME = 0x0004,
	}

	private static GeneralSettings _settings;
	private static GeneralSettings settings
	{
		get
		{
			if (_settings == null)
				_settings = GeneralSettings.Get();

			return _settings;
		}
	}

	public static void DeleteFolder(DirectoryInfo dir, string errorFormat)
	{
		SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT
		{
			wFunc = FileOperationType.FO_DELETE,
			pFrom = dir.FullName + '\0' + '\0', // double null end is required
			fFlags = FileOperationFlags.FOF_ALLOWUNDO | FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_SILENT,
			hwnd = IntPtr.Zero,
			pTo = null,
			fAnyOperationsAborted = false,
			hNameMappings = IntPtr.Zero,
			lpszProgressTitle = null
		};

		int result = SHFileOperation(ref shf);

		if (result != 0)
			GeneralManager.PopError(string.Format(errorFormat, dir.Name, result));
	}

	public static void CopyFolder(string name, string from, string to, string errorFormat, Action OnDone)
	{
		SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT
		{
			wFunc = FileOperationType.FO_COPY,
			pFrom = from + '\0' + '\0',
			pTo = to + '\0' + '\0',
			fFlags = FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_SILENT | FileOperationFlags.FOF_NOERRORUI,
			hwnd = IntPtr.Zero,
			fAnyOperationsAborted = false,
			hNameMappings = IntPtr.Zero,
			lpszProgressTitle = null,
		};

		int result = SHFileOperation(ref shf);

		if (result == 0)
			OnDone?.Invoke();
		else
			GeneralManager.PopError(string.Format(errorFormat, name, result));
	}

	public static void DeleteFile(string path)
	{
		//
	}
}
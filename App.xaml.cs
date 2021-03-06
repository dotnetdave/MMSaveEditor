﻿using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace MMSaveEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string LocalLowFolderPath;

        public static App Instance;

        public App()
        {
            Instance = this;

            Guid localLowId = new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16");
            LocalLowFolderPath = GetKnownFolderPath(localLowId);
#if !DEBUG
            // Uncomment the following after testing to see that NBug is working as configured
            NBug.Settings.ReleaseMode = true;
            NBug.Settings.WriteLogToDisk = false;
#else
            NBug.Settings.WriteLogToDisk = false;
#endif
            NBug.Settings.ProcessingException += Settings_ProcessingException;
            AppDomain.CurrentDomain.UnhandledException += NBug.Handler.UnhandledException;
            Current.DispatcherUnhandledException += NBug.Handler.DispatcherUnhandledException;

            Localisation.LoadData();
        }

        private void Settings_ProcessingException(Exception exception, NBug.Core.Reporting.Info.Report report)
        {
            if (View.MainWindow.Instance?.OpenFilePath != null)
            {
                NBug.Settings.AdditionalReportFiles.Add(View.MainWindow.Instance.OpenFilePath);
            }
        }

        string GetKnownFolderPath(Guid knownFolderId)
        {
            IntPtr pszPath = IntPtr.Zero;
            try
            {
                int hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
                if (hr >= 0)
                    return Marshal.PtrToStringAuto(pszPath);
                throw Marshal.GetExceptionForHR(hr);
            }
            finally
            {
                if (pszPath != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pszPath);
            }
        }

        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}

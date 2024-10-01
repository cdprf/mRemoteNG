﻿using Microsoft.Win32;
using System.Runtime.Versioning;
using mRemoteNG.App.Info;
using mRemoteNG.Tools.WindowsRegistry;

namespace mRemoteNG.Config.Settings.Registry
{
    [SupportedOSPlatform("windows")]
    public static class CommonRegistrySettings
    {
        #region general update registry settings

        /// <summary>
        /// Indicates whether searching for updates is allowed. If false, there is no way to update directly from mRemoteNG.
        /// </summary>
        /// <remarks>
        /// Default value is true, which allows check for updates.
        /// </remarks>
        public static bool AllowCheckForUpdates { get; }

        /// <summary>
        /// Indicates whether automatic search for updates is allowed.
        /// </summary>
        /// <remarks>
        /// Default value is true, which allows check for updates automatically.
        /// </remarks>
        public static bool AllowCheckForUpdatesAutomatical { get; }

        /// <summary>
        /// Indicates whether a manual search for updates is allowed.
        /// </summary>
        /// <remarks>
        /// The default value is true, enabling the manual check for updates.
        /// </remarks>
        public static bool AllowCheckForUpdatesManual { get; }


        #endregion

        #region general credential registry settings

        /// <summary>
        /// Setting that indicates whether exporting passwords is allowed.
        /// </summary>
        public static bool AllowExportPasswords { get; }

        /// <summary>
        /// Setting that indicates whether exporting usernames is allowed.
        /// </summary>
        public static bool AllowExportUsernames { get; }

        /// <summary>
        /// Setting that indicates whether saving passwords in connections is allowed.
        /// </summary>
        public static bool AllowSavePasswords { get; }

        /// <summary>
        /// Setting that indicates whether saving in connections usernames is allowed.
        /// </summary>
        public static bool AllowSaveUsernames { get; }

        #endregion

        static CommonRegistrySettings()
        {
            IRegistryRead regValueUtility = new WinRegistry();
            RegistryHive hive = WindowsRegistryInfo.Hive;

            #region update registry settings

            string updateSubkey = WindowsRegistryInfo.Update;

            AllowCheckForUpdates = regValueUtility.GetBoolValue(hive, updateSubkey, nameof(AllowCheckForUpdates), true);
            AllowCheckForUpdatesAutomatical = regValueUtility.GetBoolValue(hive, updateSubkey, nameof(AllowCheckForUpdatesAutomatical), AllowCheckForUpdates);
            AllowCheckForUpdatesManual = regValueUtility.GetBoolValue(hive, updateSubkey, nameof(AllowCheckForUpdatesManual), AllowCheckForUpdates);
            

            #endregion

            #region credential registry settings

            string credentialSubkey = WindowsRegistryInfo.Credential;

            AllowExportPasswords = regValueUtility.GetBoolValue(hive, credentialSubkey, nameof(AllowExportPasswords), true);
            AllowExportUsernames = regValueUtility.GetBoolValue(hive, credentialSubkey, nameof(AllowExportUsernames), true);
            AllowSavePasswords = regValueUtility.GetBoolValue(hive, credentialSubkey, nameof(AllowSavePasswords), true);
            AllowSaveUsernames = regValueUtility.GetBoolValue(hive, credentialSubkey, nameof(AllowSaveUsernames), true);

            #endregion
        }
    }
}
﻿using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;

namespace ClearComponentCache
{
    internal sealed class ClearCache
    {
        private ClearCache(OleMenuCommandService commandService, AsyncPackage package)
        {
            ServiceProvider = package;

            var commandID = new CommandID(PackageGuids.guidClearCachePackageCmdSet, PackageIds.ClearCacheId);
            var button = new MenuCommand(DeleteCacheFolder, commandID);
            commandService.AddCommand(button);
        }

        public static ClearCache Instance { get; private set; }

        private AsyncPackage ServiceProvider { get; }

        public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ClearCache(commandService, package);
        }

        private async void DeleteCacheFolder(object sender, EventArgs e)
        {
            if (!UserWantsToProceed())
                return;

            var folder = Path.Combine(ServiceProvider.UserLocalDataPath, "ComponentModelCache");

            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                var shell = await ServiceProvider.GetServiceAsync(typeof(SVsShell)) as IVsShell4;
                Assumes.Present(shell);

                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);

                Directory.Delete(folder, true);
            }
        }

        private bool UserWantsToProceed()
        {
            return MessageBox.Show(Resources.Text.promptText, Vsix.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}

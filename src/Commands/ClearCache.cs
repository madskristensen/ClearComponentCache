using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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

            string folder = await GetFolderPath();

            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                var shell = await ServiceProvider.GetServiceAsync(typeof(SVsShell)) as IVsShell4;
                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);

                Directory.Delete(folder, true);
            }
        }

        private bool UserWantsToProceed()
        {
            return MessageBox.Show(Resources.Text.promptText, Vsix.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private async System.Threading.Tasks.Task<string> GetFolderPath()
        {
            var shell = await ServiceProvider.GetServiceAsync(typeof(SVsShell)) as IVsShell;

            object root;

            // Gets the version number with the /rootsuffix. Example: "14.0Exp"
            if (shell.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out root) == VSConstants.S_OK)
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string version = Path.GetFileName(root.ToString());

                return Path.Combine(appData, "Microsoft\\VisualStudio", version, "ComponentModelCache");
            }

            return null;
        }
    }
}

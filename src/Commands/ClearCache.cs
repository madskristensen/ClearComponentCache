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
        private ClearCache(Package package)
        {
            ServiceProvider = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var commandID = new CommandID(PackageGuids.guidClearCachePackageCmdSet, PackageIds.ClearCacheId);
            var button = new MenuCommand(DeleteCacheFolder, commandID);
            commandService.AddCommand(button);
        }

        public static ClearCache Instance { get; private set; }

        private IServiceProvider ServiceProvider { get; }

        public static void Initialize(Package package)
        {
            Instance = new ClearCache(package);
        }

        private void DeleteCacheFolder(object sender, EventArgs e)
        {
            if (!UserWantsToProceed())
                return;

            string folder = GetFolderPath();

            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                IVsShell4 shell = (IVsShell4)ServiceProvider.GetService(typeof(SVsShell));
                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);

                Directory.Delete(folder, true);
            }
        }

        private bool UserWantsToProceed()
        {
            return MessageBox.Show(Text.promptText, Vsix.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private string GetFolderPath()
        {
            var shell = (IVsShell)ServiceProvider.GetService(typeof(SVsShell));
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

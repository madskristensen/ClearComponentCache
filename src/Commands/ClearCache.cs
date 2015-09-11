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
        private readonly Package package;

        private ClearCache(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.guidClearCachePackageCmdSet, PackageIds.ClearCacheId);
                var menuItem = new MenuCommand(DeleteCacheFolder, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        public static ClearCache Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ClearCache(package);
        }

        private void DeleteCacheFolder(object sender, EventArgs e)
        {
            string prompt = "This will clear the MEF component cache and restart Visual Studio.\r\n\r\nDo you wish to continue?";
            var result = MessageBox.Show(prompt, "Clear MEF Component Cache", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
                return;
            string folder = GetFolderPath();

            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                IVsShell4 shell = (IVsShell4)ServiceProvider.GetService(typeof(SVsShell));
                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);

                Directory.Delete(folder, true);
            }
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

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace ClearComponentCache
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidClearCachePackageString)]
    public sealed class ClearCachePackage : Package
    {
        protected override void Initialize()
        {
            ClearCache.Initialize(this);
            base.Initialize();
        }
    }
}

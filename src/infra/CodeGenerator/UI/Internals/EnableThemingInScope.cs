using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace CodeGenerator.UI.Internals;

[SuppressUnmanagedCodeSecurity]
internal class EnableThemingInScope : IDisposable
{
    private const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;
    private static ACTCTX enableThemingActivationContext;
    private static nint hActCtx;
    private static bool contextCreationSucceeded;
    private uint cookie;

    public EnableThemingInScope()
    {
        this.cookie = 0;
        {
            if (EnsureActivateContextCreated())
            {
                if (!ActivateActCtx(hActCtx, out this.cookie))
                {
                    this.cookie = 0;
                }
            }
        }
    }

    void IDisposable.Dispose() => this.Dispose(true);

    // All the pinvoke goo...
    [DllImport("Kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    private static extern nint CreateActCtx(ref ACTCTX actctx);

    [DllImport("Kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    private static extern bool ActivateActCtx(nint hActCtx, out uint lpCookie);

    [DllImport("Kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    private static extern bool DeactivateActCtx(uint dwFlags, uint lpCookie);

    private void Dispose(bool disposing)
    {
        if (this.cookie != 0)
        {
            if (DeactivateActCtx(0, this.cookie))
            // deactivation succeeded...
            {
                this.cookie = 0;
            }
        }
    }

    private static bool EnsureActivateContextCreated()
    {
        lock (typeof(EnableThemingInScope))
        {
            if (!contextCreationSucceeded)
            {
                string assemblyLoc = null;

                var fiop = new FileIOPermission(PermissionState.None)
                {
                    AllFiles = FileIOPermissionAccess.PathDiscovery
                };
                fiop.Assert();
                try
                {
                    assemblyLoc = typeof(object).Assembly.Location;
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }

                string manifestLoc = null;
                string installDir = null;
                if (assemblyLoc is not null)
                {
                    installDir = Path.GetDirectoryName(assemblyLoc);
                    const string manifestName = "XPThemes.manifest";
                    manifestLoc = Path.Combine(installDir, manifestName);
                }

                if (manifestLoc is not null && installDir is not null)
                {
                    enableThemingActivationContext = new ACTCTX
                    {
                        cbSize = Marshal.SizeOf(typeof(ACTCTX)),
                        lpSource = manifestLoc,

                        // Set the lpAssemblyDirectory to the install
                        // directory to prevent Win32 Side by Side from
                        // looking for comctl32 in the application
                        // directory, which could cause a bogus dll to be
                        // placed there and open a security hole.
                        lpAssemblyDirectory = installDir,
                        dwFlags = ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID
                    };

                    // This will fail gracefully if file specified
                    // by manifestLoc doesn't exist.
                    hActCtx = CreateActCtx(ref enableThemingActivationContext);
                    contextCreationSucceeded = hActCtx != new nint(-1);
                }
            }

            // If we return false, we'll try again on the next call into
            // EnsureActivateContextCreated(), which is fine.
            return contextCreationSucceeded;
        }
    }

    ~EnableThemingInScope()
    {
        this.Dispose(false);
    }

    private struct ACTCTX
    {
        public int cbSize;
        public uint dwFlags;
        public string lpApplicationName;
        public string lpAssemblyDirectory;
        public string lpResourceName;
        public string lpSource;
        public ushort wLangId;
        public ushort wProcessorArchitecture;
    }
}

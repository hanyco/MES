using System.Windows;
using System.Windows.Interop;

using CodeGenerator.UI.Dialogs;

using DialogCommon = System.Windows.Forms.CommonDialog;
using ResultDialog = System.Windows.Forms.DialogResult;

namespace CodeGenerator.UI.Internals;

public abstract class CommonDialog<TDialog, TThis> : IDisposable
    where TDialog : DialogCommon, new()
    where TThis : CommonDialog<TDialog, TThis>
{
    private bool _disposedValue;

    protected CommonDialog()
        => this.Dialog = this.Create();

    protected TDialog Dialog { get; private set; }

    protected virtual TDialog Create()
        => new();

    protected void CheckDisposition()
    {
        if (!this._disposedValue)
        {
            return;
        }

        throw new ObjectDisposedException(typeof(TThis).Name);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this.OnDispose();
                this.Dialog.Dispose();
                this.Dialog = null!;
            }

            this._disposedValue = true;
        }
    }
    protected virtual void OnDispose() { }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public DialogResult ShowDialog()
    {
        var result = this.Dialog.ShowDialog();
        return result.ToDialogResult();
    }
}

internal static class Extensions
{
    extension(ResultDialog @this)
    {
        public DialogResult ToDialogResult()
            => (DialogResult)(int)@this;
    }
}
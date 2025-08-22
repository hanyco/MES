using System.Windows;
using System.Windows.Controls;

using CodeGenerator.Application.Settings;
using CodeGenerator.UI.Dialogs;

namespace CodeGenerator.Designer.UI.Controls;

public partial class SettingsControl : UserControl
{
    private Settings _settings = default!;

    public SettingsControl() =>
        this.InitializeComponent();

    private async void OnLoad(object sender, RoutedEventArgs e)
    {
        await Settings.Load();
        this.LoadFromSettings();
    }

    private async void OnSave(object sender, RoutedEventArgs e)
    {
        this.SaveToSettings();
        await Settings.Save();
        await Settings.Load();
        this.LoadFromSettings();
        TaskDialog.Info("Settings saved successfully.");
    }

    private void SaveToSettings()
    {
    this._settings.Folders.PagesPath = this.PagesPathBox.Text;
    this._settings.Folders.ComponentsPath = this.ComponentsPathBox.Text;
    this._settings.Folders.ViewModelsPath = this.ViewModelsPathBox.Text;
    this._settings.Folders.ControllersPath = this.ControllersPathBox.Text;
    this._settings.Folders.ApplicationPath = this.ApplicationPathBox.Text;
    this._settings.Folders.ApplicationDtosPath = this.DtosPathBox.Text;
    this._settings.Folders.RepositoriesPath = this.RepositoriesPathBox.Text;
    this._settings.Folders.DefaultRoot = this.RootPathBox.Text;
    this._settings.CodeConfigs.RootNameSpace = this.RootNamespaceBox.Text;
    }

    private void LoadFromSettings()
    {
    this._settings = Reflection.Copy(Settings.Default);
    this.PagesPathBox.Text = this._settings.Folders.PagesPath;
    this.ComponentsPathBox.Text = this._settings.Folders.ComponentsPath;
    this.ViewModelsPathBox.Text = this._settings.Folders.ViewModelsPath;
    this.ControllersPathBox.Text = this._settings.Folders.ControllersPath;
    this.ApplicationPathBox.Text = this._settings.Folders.ApplicationPath;
    this.DtosPathBox.Text = this._settings.Folders.ApplicationDtosPath;
    this.RepositoriesPathBox.Text = this._settings.Folders.RepositoriesPath;
    this.RootPathBox.Text = this._settings.Folders.DefaultRoot;
    this.RootNamespaceBox.Text = this._settings.CodeConfigs.RootNameSpace;
    }

    private void OnBrowseRootPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.RootPathBox.Text = path;
        }
    }

    private void OnBrowseDtosPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.DtosPathBox.Text = path;
        }
    }

    private void OnBrowsePagesPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.PagesPathBox.Text = path;
        }
    }

    private void OnBrowseComponentsPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ComponentsPathBox.Text = path;
        }
    }

    private void OnBrowseViewModelsPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ViewModelsPathBox.Text = path;
        }
    }

    private void OnBrowseControllersPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ControllersPathBox.Text = path;
        }
    }

    private void OnBrowseApplicationPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.ApplicationPathBox.Text = path;
        }
    }

    private void OnBrowseRepositoriesPath(object sender, RoutedEventArgs e)
    {
        (var result, var path) = FolderBrowserDialog.Show();
        if (result == DialogResult.OK)
        {
            this.RepositoriesPathBox.Text = path;
        }
    }

    private void OnReset(object sender, RoutedEventArgs e)
    {
        if (TaskDialog.AskWithWarning("Are you sure you want to reset settings?") != TaskDialogResult.Yes)
        {
            return;
        }
        Settings.Reset();
        this.LoadFromSettings();
    }
}

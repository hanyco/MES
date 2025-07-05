using System;
using System.Windows;

namespace CodeGenerator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ResourceDictionary? _currentTheme;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        if (this.Resources.MergedDictionaries.Count > 0)
        {
            _currentTheme = this.Resources.MergedDictionaries[0];
            this.OnThemeApplied("LightTheme.xaml");
        }
    }

    public void UseLightTheme() => this.ApplyTheme("LightTheme.xaml");

    public void UseDarkTheme() => this.ApplyTheme("DarkTheme.xaml");

    private void ApplyTheme(string themeFile)
    {
        if (_currentTheme is not null)
        {
            this.Resources.MergedDictionaries.Remove(_currentTheme);
        }

        var dict = new ResourceDictionary
        {
            Source = new Uri($"Designer/UI/Styles/{themeFile}", UriKind.Relative)
        };

        this.Resources.MergedDictionaries.Insert(0, dict);
        _currentTheme = dict;
        this.OnThemeApplied(themeFile);
    }

    partial void OnThemeApplied(string themeFile);
}

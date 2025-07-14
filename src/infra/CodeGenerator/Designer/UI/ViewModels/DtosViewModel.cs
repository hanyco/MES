using System.ComponentModel;
using System.Windows.Input;

using CodeGenerator.Application.Domain;
using CodeGenerator.Application.Services;
using CodeGenerator.Designer.UI.Common;

namespace CodeGenerator.Designer.UI.ViewModels;

public class DtosViewModel : INotifyPropertyChanged
{
    private readonly IDtoService _dtoService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DtosViewModel(IDtoService dtoService)
    {
        this._dtoService = dtoService;

        this.RefreshCommand = new RelayCommand(async _ => await this.LoadDtosAsync());
        this.AddCommand = new RelayCommand(_ => this.AddDto());
        this.SaveCommand = new RelayCommand(async _ => await this.SaveDtoAsync(), _ => this.SelectedDto != null);
        this.DeleteCommand = new RelayCommand(async _ => await this.DeleteDtoAsync(), _ => this.SelectedDto != null);
        this.GenerateCodeCommand = new RelayCommand(_ => this.GenerateCode(), _ => this.SelectedDto != null);
        this.LoadColumnsCommand = new RelayCommand(_ => this.LoadColumns(), _ => this.SelectedTable != null);

        // initial load
        _ = this.LoadDtosAsync();
        this.LoadTables();
    }

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand GenerateCodeCommand { get; }
    public ICommand LoadColumnsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SaveCommand { get; }

    public Dto? SelectedDto
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                this.OnPropertyChanged(nameof(this.SelectedDto));
                ((RelayCommand)this.SaveCommand).RaiseCanExecuteChanged();
                ((RelayCommand)this.DeleteCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string? SelectedTable { get; set; }

    private void AddDto()
    {
        var newDto = new Dto();
        this.SelectedDto = newDto;
    }

    private async Task DeleteDtoAsync(CancellationToken cancellationToken = default)
    {
        if (this.SelectedDto == null)
        {
            return;
        }

        await this._dtoService.Delete(this.SelectedDto.Id, cancellationToken).ThrowOnFail(cancellationToken: cancellationToken).End();
        await this.LoadDtosAsync(cancellationToken);
    }

    private void GenerateCode()
    {
        if (this.SelectedDto == null)
        {
            return;
        }

        _ = this._dtoService.GenerateCodes(this.SelectedDto);
    }

    private void LoadColumns()
    {
        if (this.SelectedTable == null)
        {
            return;
        }
    }

    private Task LoadDtosAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    private void LoadTables()
    {
    }

    private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async Task SaveDtoAsync(CancellationToken cancellationToken = default)
    {
        if (this.SelectedDto == null)
        {
            return;
        }

        if (this.SelectedDto.Id == default)
        {
            _ = await this._dtoService.Insert(this.SelectedDto, cancellationToken).ThrowOnFail(cancellationToken: cancellationToken);
        }
        else
        {
            _ = await this._dtoService.Update(this.SelectedDto.Id, this.SelectedDto, cancellationToken).ThrowOnFail(cancellationToken: cancellationToken);
        }

        await this.LoadDtosAsync(cancellationToken);
    }
}
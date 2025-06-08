using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

using CodeGenerator.Application.Cqrs;
using CodeGenerator.Application.Cqrs.Commands;
using CodeGenerator.Application.Domain;
using CodeGenerator.Application.Services;
using Library.CodeGenLib.Models;
using CodeGenerator.Designer.UI.Common;

using MediatR;

namespace CodeGenerator.Designer.UI.ViewModels;

public class DtosPageViewModel : INotifyPropertyChanged
{
    private readonly IMediator _mediator;
    private readonly DtoService _dtoService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DtosPageViewModel(IMediator mediator, DtoService dtoService)
    {
        this._mediator = mediator;
        this._dtoService = dtoService;
        this.Dtos = [];
        this.Tables = [];
        this.Columns = [];

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
    public ObservableCollection<DtoDefinition> Dtos { get; }

    public ObservableCollection<string> Tables { get; }
    public ObservableCollection<FieldDefinition> Columns { get; }

    public string? SelectedTable { get; set; }

    public ICommand RefreshCommand { get; }

    public ICommand SaveCommand { get; }
    public ICommand GenerateCodeCommand { get; }
    public ICommand LoadColumnsCommand { get; }

    public DtoDefinition? SelectedDto
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

    private void AddDto()
    {
        var newDto = new DtoDefinition
        {
            Id = Guid.NewGuid(),
            Name = "NewDto",
            Namespace = "MyNamespace"
        };
        this.Dtos.Add(newDto);
        this.SelectedDto = newDto;
    }

    private async Task DeleteDtoAsync()
    {
        if (this.SelectedDto == null)
        {
            return;
        }

        await this._mediator.Send(new DeleteDtoCommand(this.SelectedDto.Id));
        await this.LoadDtosAsync();
    }

    private async Task LoadDtosAsync()
    {
        var dtos = await this._mediator.Send(new GetAllDtosQuery());
        this.Dtos.Clear();
        foreach (var dto in dtos)
        {
            this.Dtos.Add(dto);
        }

        this.SelectedDto = this.Dtos.Count > 0 ? this.Dtos[0] : null;
        this.OnPropertyChanged(nameof(this.Dtos));
    }

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async Task SaveDtoAsync()
    {
        if (this.SelectedDto == null)
        {
            return;
        }

        if (this.SelectedDto.Id == default)
        {
            this.SelectedDto = await this._mediator.Send(new CreateDtoCommand(this.SelectedDto));
        }
        else
        {
            this.SelectedDto = await this._mediator.Send(new UpdateDtoCommand(this.SelectedDto));
        }

        await this.LoadDtosAsync();
    }

    private void LoadTables()
    {
        this.Tables.Clear();
        foreach (var t in this._dtoService.GetTables())
        {
            this.Tables.Add(t);
        }
        this.SelectedTable = this.Tables.FirstOrDefault();
    }

    private void LoadColumns()
    {
        if (this.SelectedTable == null)
        {
            return;
        }

        this.Columns.Clear();
        foreach (var c in this._dtoService.GetColumns(this.SelectedTable))
        {
            this.Columns.Add(c);
        }

        if (this.SelectedDto != null)
        {
            this.SelectedDto.Fields = [.. this.Columns];
        }
    }

    private void GenerateCode()
    {
        if (this.SelectedDto == null)
        {
            return;
        }

        var code = this._dtoService.GenerateDtoCode(this.SelectedDto);
        System.Windows.Clipboard.SetText(code);
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

using CodeGenerator.Application.Domain;
using CodeGenerator.Designer.UI.Common;
using CodeGenerator.Application.Services;

namespace CodeGenerator.Designer.UI.ViewModels;

public class DtosPageViewModel : INotifyPropertyChanged
{
    private readonly DtoService _service;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DtosPageViewModel(DtoService service)
    {
        this._service = service;
        this.Dtos = [];

        this.RefreshCommand = new RelayCommand(async _ => await this.LoadDtosAsync());
        this.AddCommand = new RelayCommand(_ => this.AddDto());
        this.SaveCommand = new RelayCommand(async _ => await this.SaveDtoAsync(), _ => this.SelectedDto != null);
        this.DeleteCommand = new RelayCommand(async _ => await this.DeleteDtoAsync(), _ => this.SelectedDto != null);

        // initial load
        _ = this.LoadDtosAsync();
    }

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ObservableCollection<DtoDefinition> Dtos { get; }

    public ICommand RefreshCommand { get; }

    public ICommand SaveCommand { get; }

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
            Id = 0,
            Guid = Guid.NewGuid(),
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

        await this._service.Delete(this.SelectedDto.Id);
        await this.LoadDtosAsync();
    }

    private async Task LoadDtosAsync()
    {
        var dtos = await this._service.GetAll();
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
            var id = await this._service.Insert(this.SelectedDto);
            this.SelectedDto.Id = id;
        }
        else
        {
            await this._service.Update(this.SelectedDto);
        }

        await this.LoadDtosAsync();
    }
}
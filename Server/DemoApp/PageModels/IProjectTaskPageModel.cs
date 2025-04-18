using CommunityToolkit.Mvvm.Input;
using DemoApp.Models;

namespace DemoApp.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}
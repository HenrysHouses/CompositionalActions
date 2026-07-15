#nullable enable
namespace HH.Input.Contracts
{
    using HH.Actors.Contracts;

    public interface IInputBinding
    {
        bool IsEnabled { get; }

        bool IsSwappable { get; }

        IActionInput Action { get; set; }

        void SetInput(IActionInput action);

        void BindInput(IDefaultActor actor);

        void RemoveInputBinding();

        void Start();

        void Update();

        void Cancel();
    }
}

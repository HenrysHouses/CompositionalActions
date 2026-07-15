#nullable enable
namespace HH.Input.Networking
{
    using Fusion;
    using HH.Actors.Contracts;

    public interface INetworkedInputBinding
    {
        bool IsEnabled { get; }

        bool IsSwappable { get; }

        INetworkedActionInput Action { get; set; }

        void SetInput(INetworkedActionInput action);

        void BindInput(NetworkBehaviourId id, ActorCallback.ActorCallbackSource actorCallback);

        void RemoveInputBinding();

        void Start();

        void Update();

        void Cancel();
    }
}

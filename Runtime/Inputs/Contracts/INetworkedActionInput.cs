#nullable enable
namespace HH.Input.Networking
{
    using HH.Input.Contracts;

    public interface INetworkedActionInput
    {
        void Start(NetworkedInputState input);

        void Update(NetworkedInputState input);

        void Cancel(NetworkedInputState input);

        void Failed(NetworkedInputState input);
    }
}

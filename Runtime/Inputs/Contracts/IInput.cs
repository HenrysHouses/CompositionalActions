#nullable enable
namespace HH.Input.Contracts
{
    public interface IActionInput
    {
        void Start(InputCallback inputCallback);

        void Update(InputCallback inputCallback);

        void Cancel(InputCallback inputCallback);

        void Failed(InputCallback inputCallback);
    }
}

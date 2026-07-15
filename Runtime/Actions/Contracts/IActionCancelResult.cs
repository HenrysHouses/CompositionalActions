#nullable enable
namespace HH.CompositionalActions.Contracts
{
    public interface IActionCancelResult : IActionResult
    {
        void HandleCancellation(IActionContext context, IActionResult history);
    }
}

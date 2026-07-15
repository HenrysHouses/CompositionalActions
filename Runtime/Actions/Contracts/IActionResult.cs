#nullable enable

namespace HH.CompositionalActions.Contracts
{
    using System.Collections;

    public interface IActionResult : IEnumerator
    {
        bool HasReturnValues => GetType() != typeof(EmptyActionResult);

        IActionResult? ChainedResult { get; }

        IActionResult AddResultChain(IActionResult result);

        bool TryGetResult<T>(out IActionResult result) where T : IActionResult;

        bool TryGetResults<T>(out T[] results) where T : IActionResult;
    }
}

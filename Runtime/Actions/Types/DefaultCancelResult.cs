#nullable enable
namespace HH.CompositionalActions.Contracts
{
    using System.Collections;
    using System.Collections.Generic;

    public class DefaultCancelResult : IActionCancelResult
    {
        public DefaultCancelResult()
        {
        }

        public object? Current => Routine?.Current;

        public IActionResult? ChainedResult { get; protected set; }

        public IEnumerator? Routine { get; protected set; }

        public IActionResult AddResultChain(IActionResult result)
        {
            ChainedResult = result;
            return this;
        }

        public void HandleCancellation(IActionContext context, IActionResult history)
        {
        }

        public bool MoveNext()
        {
            return Routine != null && Routine.MoveNext();
        }

        public void Reset()
        {
            Routine?.Reset();
        }

        public bool TryGetResult<T>(out IActionResult result) where T : IActionResult
        {
            if (this is T)
            {
                result = this;
                return true;
            }

            // reached the end of the chain
            if (ChainedResult is null)
            {
                result = new EmptyActionResult();
                return false;
            }

            // first element matching in the result chain
            if (ChainedResult is T ofType)
            {
                result = ofType;
                return true;
            }

            // recursively checking the chain
            if (!ChainedResult.TryGetResult<T>(out IActionResult nestedResult))
            {
                result = new EmptyActionResult();
                return false;
            }

            result = nestedResult;
            return true;
        }

        public bool TryGetResults<T>(out T[] results) where T : IActionResult
        {
            List<T> matchingResults = new List<T>(10);

            // has no history
            if (ChainedResult is null)
            {
                results = matchingResults.ToArray();
                return false;
            }

            if (ChainedResult is T ofType)
            {
                matchingResults.Add(ofType);
            }

            if (!ChainedResult.TryGetResults(out T[] nestedResults))
            {
                foreach (T nestedOfType in nestedResults)
                {
                    matchingResults.Add(nestedOfType);
                }
            }

            results = matchingResults.ToArray();
            return matchingResults.Count > 0;
        }
    }
}

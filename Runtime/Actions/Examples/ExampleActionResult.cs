#nullable enable

namespace HH.CompositionalActions.Examples
{
    using System;
    using HH.CompositionalActions.Contracts;
    using HH.Actors.Contracts;

    internal class ExampleActionResult : EmptyActionResult
    {
        private readonly IDefaultActor? exampleStorage;
        private int? timesTriggered;

        public ExampleActionResult(IDefaultActor? storeThis) : base()
        {
            exampleStorage = storeThis;
            timesTriggered = null;
        }

        public int Storage
        {
            get
            {
                if (exampleStorage == null)
                {
                    // ? Handle null exceptions here. We have our own specialized Exceptions for this type of error in HH.Debugging.

                    // # see example:
                    // throw PropertyNullException.FromClass(nameof(initializedDareID), this);

                    throw new NullReferenceException();
                }

                return Storage;
            }
        }

        public int TimesTriggeredDuringThisAction
        {
            get
            {
                if (timesTriggered == null)
                {
                    // ? Handle null exceptions here. We have our own specialized Exceptions for this type of error in HH.Debugging.

                    // # see example:
                    // throw PropertyNullException.FromClass(nameof(initializedDareID), this);

                    throw new NullReferenceException();
                }

                return timesTriggered.Value;
            }
        }

        public void IncrementTriggers()
        {
            timesTriggered += 1;
        }
    }
}

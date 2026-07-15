namespace HH.CompositionalActions.Networking
{
    using HH.Actors.Contracts;
    using HH.Input.Contracts;

    public struct BaseContextFactoryData
    {
        public ActorCallback ActorCallback { get; set; }

        public InputCallback InputCallback { get; set; }
    }
}

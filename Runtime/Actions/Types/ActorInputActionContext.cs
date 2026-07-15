#nullable enable
namespace HH.CompositionalActions.Contracts
{
    using HH.Actors.Contracts;
    using HH.Input.Contracts;

    public class ActorInputActionContext : IActionContext
    {
        public ActorInputActionContext(IDefaultActor initiatingActor, IDefaultActor executingActor, ActorCallback actorCallback, InputCallback inputCallback)
        {
            InitiatingActor = initiatingActor;
            ExecutingActor = executingActor;
            ActorCallback = actorCallback;
            InputCallback = inputCallback;
        }

        public IDefaultActor InitiatingActor { get; private set; }

        public IDefaultActor ExecutingActor { get; private set; }

        public ActorCallback ActorCallback { get; }

        public InputCallback InputCallback { get; }
    }
}

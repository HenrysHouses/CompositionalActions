#nullable enable
namespace HH.CompositionalActions.Contracts
{
    using HH.Actors.Contracts;

    public class ActionContext : IActionContext
    {
        public ActionContext(IDefaultActor initiatingActor, IDefaultActor executingActor, ActorCallback actorCallback)
        {
            InitiatingActor = initiatingActor;
            ExecutingActor = executingActor;
            ActorCallback = actorCallback;
        }

        public IDefaultActor InitiatingActor { get; private set; }

        public IDefaultActor ExecutingActor { get; private set; }

        public ActorCallback ActorCallback { get; }
    }
}

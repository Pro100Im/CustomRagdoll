namespace Ragdoll
{
    public abstract class BaseRagdollState
    {
        protected BaseRagdoll _context;

        public BaseRagdollState(BaseRagdoll context)
        {
            _context = context;
        }

        public abstract void Enter();

        public abstract void Execute();
    }
}

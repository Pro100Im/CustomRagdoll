using UnityEngine;

namespace Ragdoll
{
    public class InactiveRagdollState : BaseRagdollState
    {
        public InactiveRagdollState(BaseRagdoll context) : base(context)
        {
        }

        public override void Enter()
        {
            _context.DisableRagdoll();
            _context.LockCharacter(false);
        }

        public override void Execute()
        {
            
        }
    }
}
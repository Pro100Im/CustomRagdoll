using UnityEngine;

namespace Ragdoll
{
    public class InactiveRagdollState : BaseRagdollState
    {
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
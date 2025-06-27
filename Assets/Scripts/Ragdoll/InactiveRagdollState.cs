using UnityEngine;

namespace Ragdoll
{
    public class InactiveRagdollState : BaseRagdollState
    {
        public override void Enter()
        {
            _context.DisableRagdoll();
        }

        public override void Execute()
        {
            
        }
    }
}
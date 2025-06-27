using System.Linq;
using UnityEngine;

namespace Ragdoll
{
    public class FallingRagdollState : BaseRagdollState
    {
        private float _stableTime;
        private const float _requiredStableDuration = 1.5f;
        private const float _magnitudeThreshold = 0.1f;

        public override void Enter()
        {
            _stableTime = 0f;
            _context.EnableRagdoll();
        }

        public override void Execute()
        {
            if(IsRagdollStable())
            {
                _stableTime += Time.fixedDeltaTime;

                if(_stableTime >= _requiredStableDuration)
                    GetUp();
            }
            else
                _stableTime = 0f;
        }

        private bool IsRagdollStable()
        {
            foreach(var rb in _context.RagdollRigidbodies.Skip(1))
            {
                if(rb.linearVelocity.sqrMagnitude > _magnitudeThreshold)
                    return false;
            }

            return true;
        }

        private void GetUp()
        {
            _context.TransitionTo(new GettingUpRagdollState());
        }
    }
}
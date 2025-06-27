using UnityEngine;

namespace Ragdoll
{
    public class GettingUpRagdollState : BaseRagdollState
    {
        public override void Enter()
        {
            var hipsBone = _context.HipsBone;
            var characterTransform = _context.transform;
            var animator = _context.Animator;

            AlignPositionToHips(hipsBone, characterTransform);
            GettingUp(animator);

            _context.DisableRagdoll();
        }

        public override void Execute()
        {
            if(_context.Animator.GetCurrentAnimatorStateInfo(0).IsName(_context.GettingUpAnim) == false)
            {
                _context.TransitionTo(new InactiveRagdollState());
            }
        }

        private void GettingUp(Animator animator)
        {
            animator.Rebind();
            animator.Play(_context.GettingUpAnim);
            animator.Update(0f);
            animator.enabled = true;
        }

        private void AlignRotationToHips(Transform hipsBone, Transform characterTransform)
        {
            var originalHipsPosition = hipsBone.position;
            var originalHipsRotation = hipsBone.rotation;
            var desiredDirection = hipsBone.up * -1;
            desiredDirection.y = 0;
            desiredDirection.Normalize();

            var fromToRotation = Quaternion.FromToRotation(characterTransform.forward, desiredDirection);
            characterTransform.rotation *= fromToRotation;

            hipsBone.position = originalHipsPosition;
            hipsBone.rotation = originalHipsRotation;
        }

        private void AlignPositionToHips(Transform hipsBone, Transform characterTransform)
        {
            Vector3 originalHipsPosition = hipsBone.position;
            characterTransform.position = hipsBone.position;

            //Vector3 positionOffset = _standUpBoneTransforms[0].Position;
            //positionOffset.y = 0;
            //positionOffset = transform.rotation * positionOffset;
            //transform.position -= positionOffset;

            //if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
            //{
            //    transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            //}

            hipsBone.position = originalHipsPosition;
        }
    }
}
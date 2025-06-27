using UnityEngine;

namespace Ragdoll
{
    public class GettingUpRagdollState : BaseRagdollState
    {
        private Transform _hipsBone;
        private Transform _characterTransform;

        public override void Enter()
        {
            _hipsBone = _context.Animator.GetBoneTransform(HumanBodyBones.Hips);
            _characterTransform = _context.transform;

            AlignPositionToHips();

            _context.GettingUp();
            _context.DisableRagdoll();
        }

        public override void Execute()
        {
            if(_context.Animator.GetCurrentAnimatorStateInfo(0).IsName(_context.GettingUpAnim) == false)
            {
                Debug.LogWarning("Trans To In");
                _context.TransitionTo(new InactiveRagdollState());
            }
        }

        private void AlignRotationToHips()
        {
            var originalHipsPosition = _hipsBone.position;
            var originalHipsRotation = _hipsBone.rotation;
            var desiredDirection = _hipsBone.up * -1;
            desiredDirection.y = 0;
            desiredDirection.Normalize();

            var fromToRotation = Quaternion.FromToRotation(_characterTransform.forward, desiredDirection);
            _characterTransform.rotation *= fromToRotation;

            _hipsBone.position = originalHipsPosition;
            _hipsBone.rotation = originalHipsRotation;
        }

        private void AlignPositionToHips()
        {
            Vector3 originalHipsPosition = _hipsBone.position;
            _characterTransform.position = _hipsBone.position;

            //Vector3 positionOffset = _standUpBoneTransforms[0].Position;
            //positionOffset.y = 0;
            //positionOffset = transform.rotation * positionOffset;
            //transform.position -= positionOffset;

            //if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
            //{
            //    transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            //}

            _hipsBone.position = originalHipsPosition;
        }
    }
}
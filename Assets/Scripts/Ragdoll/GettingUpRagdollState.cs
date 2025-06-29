
namespace Ragdoll
{
    public class GettingUpRagdollState : BaseRagdollState
    {
        public GettingUpRagdollState(BaseRagdoll context) : base(context)
        {
        }

        public override void Enter()
        {
            GettingUp();

            _context.DisableRagdoll();
        }

        public override void Execute()
        {
            if(_context.Animator.GetCurrentAnimatorStateInfo(0).IsName(GetStandUpAnim()) == false)
            {
                _context.TransitionTo(typeof(InactiveRagdollState));
            }
        }

        private void GettingUp()
        {
            var animator = _context.Animator;

            animator.Rebind();
            animator.Play(GetStandUpAnim(), 0 ,0);
            animator.enabled = true;
        }

        private string GetStandUpAnim() => _context.IsFacingUp ? _context.BackGetUpAnim : _context.FaceGetUpAnim;
    }
}

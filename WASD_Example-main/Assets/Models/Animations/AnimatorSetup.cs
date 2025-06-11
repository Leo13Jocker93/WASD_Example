using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimatorSetup : MonoBehaviour
{
    [Header("Animator Setup")]
    public string controllerName = "PlayerAnimatorController";
    public AnimationClip idleClip;
    public AnimationClip walkClip;
    public AnimationClip runClip;
    public AnimationClip jumpClip;

    [ContextMenu("Create Animator Controller")]
    void CreateAnimator()
    {
        string path = "Assets/" + controllerName + ".controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine sm = layer.stateMachine;

        // Crear parámetros
        controller.AddParameter("VelocityX", AnimatorControllerParameterType.Float);
        controller.AddParameter("VelocityZ", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);

        // Crear Blend Tree
        AnimatorState blendTreeState = sm.AddState("Locomotion");
        BlendTree blendTree;
        blendTreeState.motion = blendTree = new BlendTree();
        blendTree.name = "LocomotionBlendTree";
        blendTree.blendType = BlendTreeType.FreeformDirectional2D;
        blendTree.blendParameter = "VelocityX";
        blendTree.blendParameterY = "VelocityZ";

        blendTree.useAutomaticThresholds = false;

        if (idleClip != null)
            blendTree.AddChild(idleClip, new Vector2(0, 0));
        if (walkClip != null)
            blendTree.AddChild(walkClip, new Vector2(0, 1));
        if (runClip != null)
            blendTree.AddChild(runClip, new Vector2(0, 2));

        // Salto: desde cualquier estado
        if (jumpClip != null)
        {
            AnimatorState jumpState = sm.AddState("Jump");
            jumpState.motion = jumpClip;
            AnimatorStateTransition jumpTransition = sm.AddAnyStateTransition(jumpState);
            jumpTransition.AddCondition(AnimatorConditionMode.IfNot, 0, "IsGrounded");
            jumpTransition.hasExitTime = false;
            jumpTransition.duration = 0.1f;

            // Regreso al blend tree
            AnimatorStateTransition returnTransition = jumpState.AddTransition(blendTreeState);
            returnTransition.AddCondition(AnimatorConditionMode.If, 0, "IsGrounded");
            returnTransition.hasExitTime = true;
            returnTransition.exitTime = 0.8f;
            returnTransition.duration = 0.25f;
        }

        Debug.Log("Animator Controller created at: " + path);
    }
}

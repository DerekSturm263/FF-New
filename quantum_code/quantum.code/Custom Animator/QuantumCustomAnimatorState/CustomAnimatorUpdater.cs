using Photon.Deterministic;
using System;
using System.Collections.Generic;

namespace Quantum.Custom.Animator
{
  public unsafe class AnimatorUpdater
  {
    List<AnimatorRuntimeBlendData> _blendList;
    List<AnimatorMotion> _motionList;

    public AnimatorUpdater()
    {
      _blendList = new List<AnimatorRuntimeBlendData>();
      _motionList = new List<AnimatorMotion>();
    }

    public void Update(Frame f)
    {
      var animatorsIterator = f.Unsafe.GetComponentBlockIterator<CustomAnimator>();

      foreach (var (entity, animator) in animatorsIterator)
      {
        if (animator->freeze)
          continue;

        CustomAnimatorGraph ag = f.FindAsset<CustomAnimatorGraph>(animator->animatorGraph.Id);
        
        if (animator->animatorGraph.Id.Equals(default) == false)
        {
          ag.UpdateGraphState(f, &entity, animator, f.DeltaTime * animator->speed);
          
          if (ag.rootMotion)
          {
            _blendList.Clear();
            _motionList.Clear();

            AnimatorFrame deltaFrame = ag.CalculateRootMotion(f, animator, _blendList, _motionList);
            FP deltaRot = -deltaFrame.rotationY;
            FPVector2 deltaMovement = new FPVector2(deltaFrame.position.X, deltaFrame.position.Z);

            PhysicsBody2D* physicsBody;
            var hasPhysicsBody = f.Unsafe.TryGetPointer(entity, out physicsBody);

            Transform2D* transform;
            var hasTransform = f.Unsafe.TryGetPointer(entity, out transform);
            
            if (hasPhysicsBody && ag.rootMotionAppliesPhysics && physicsBody != null && physicsBody->Enabled)
            {
              FPVector2 velocity = deltaMovement / f.DeltaTime;
              FP angularVelocity = deltaRot / f.DeltaTime;
              physicsBody->Velocity = velocity;

              if (!physicsBody->FreezeRotation)
                physicsBody->AngularVelocity = angularVelocity;
            }
            else if (hasTransform)
            {
              FP currentYRot = transform->Rotation;//radians
              FP newRotation = currentYRot + deltaRot;
              while (newRotation < -FP.Pi) newRotation += FP.PiTimes2;
              while (newRotation > FP.Pi) newRotation += -FP.PiTimes2;
              deltaMovement = FPVector2.Rotate(deltaMovement, newRotation);

              transform->Position += deltaMovement;
              transform->Rotation = newRotation;
            }
          }
        }
      }
    }
  }
}

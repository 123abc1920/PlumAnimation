using System;
using PlumJsonAnimator.Common.Constants;
using PlumJsonAnimator.Models.SkeletonNameSpace;

namespace PlumJsonAnimator.Models.Common
{
    /// <summary>
    /// Transform modes types. Not IKeyFrameTypes, IKeyFrameTypes are only for storage information about transformations
    /// </summary>
    public enum TransformModesTypes
    {
        NO = 0,
        TRANSLATE,
        ROTATE,
        SCALE,
        SHEAR,
    }

    /// <summary>
    /// Mode for no transformations
    /// </summary>
    public abstract class Mode
    {
        public TransformModesTypes type;
        public string name = "";

        protected GlobalState globalState;

        public Mode(GlobalState globalState)
        {
            this.globalState = globalState;
        }

        public abstract void ClearMode();

        public abstract void Transform(Bone bone, double a, double b);
    }

    class NoMode : Mode
    {
        public NoMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.NO;
            name = "";
        }

        public override void ClearMode() { }

        public override void Transform(Bone bone, double a, double b)
        {
            return;
        }
    }

    class TransformMode : Mode
    {
        public TransformMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.TRANSLATE;
            name = "transform";
        }

        public override void ClearMode() { }

        public override void Transform(Bone bone, double x, double y)
        {
            bone.Move(x, y);
            if (this.globalState.setBasePos == false)
            {
                var animation = this.globalState.CurrentProject?.GetCurrentAnimation();
                if (animation != null && !animation.IsRun && bone.IsBone == true)
                {
                    animation.TranslateBone(bone, bone.X, bone.Y);
                }
            }
        }
    }

    class RotateMode : Mode
    {
        private class Point
        {
            public double x;
            public double y;

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public RotateMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.ROTATE;
            name = "rotate";
        }

        public override void ClearMode() { }

        public override void Transform(Bone bone, double x, double y)
        {
            double xx = x - bone.X;
            Point av = new Point(xx, y - bone.Y);
            Point bv = new Point(10, 0);

            double dot = av.x * bv.x + av.y * bv.y;
            double det = av.x * bv.y - av.y * bv.x;
            double angleRad = Math.Atan2(det, dot);
            double angleDeg = angleRad * 180 / Math.PI;

            bone.Rotate(-angleDeg);
            if (this.globalState.setBasePos == false)
            {
                var animation = this.globalState.CurrentProject?.GetCurrentAnimation();
                if (animation != null && !animation.IsRun && bone.IsBone == true)
                {
                    animation.RotateBone(bone, bone.A);
                }
            }
        }
    }

    class ScaleMode : Mode
    {
        private double? startX = null;
        private double? startY = null;

        public ScaleMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.SCALE;
            name = "scale";
        }

        public override void ClearMode()
        {
            startX = null;
            startY = null;
        }

        public override void Transform(Bone bone, double x, double y)
        {
            if (startX == null || startY == null)
            {
                startX = x;
                startY = y;
                return;
            }
            startX = x;
            startY = y;
            bone.Scale(x, y);
        }
    }

    class ShearMode : Mode
    {
        private double? startX = null;
        private double? startY = null;
        private double startShearX = 0;
        private double startShearY = 0;
        private bool isHorizontalLocked = false;
        private bool isVerticalLocked = false;

        private const double Sensitivity = 0.5;
        private const double LockThreshold = 5.0;

        public ShearMode(GlobalState globalState)
            : base(globalState)
        {
            type = TransformModesTypes.SHEAR;
            name = "shear";
        }

        public override void ClearMode()
        {
            startX = null;
            startY = null;
            isHorizontalLocked = false;
            isVerticalLocked = false;
        }

        public override void Transform(Bone bone, double x, double y)
        {
            if (startX == null || startY == null)
            {
                startX = x;
                startY = y;
                startShearX = bone.ShearX;
                startShearY = bone.ShearY;
                return;
            }

            double deltaX = x - startX.Value;
            double deltaY = y - startY.Value;

            if (!isHorizontalLocked && !isVerticalLocked)
            {
                if (Math.Abs(deltaX) > LockThreshold)
                {
                    isHorizontalLocked = true;
                }
                else if (Math.Abs(deltaY) > LockThreshold)
                {
                    isVerticalLocked = true;
                }
            }

            double newShearX = startShearX;
            double newShearY = startShearY;

            if (isHorizontalLocked)
            {
                newShearY = startShearY + deltaX * Sensitivity;
            }
            else if (isVerticalLocked)
            {
                newShearX = startShearX + deltaY * Sensitivity;
            }

            newShearX = Math.Clamp(newShearX, -89.0, 89.0);
            newShearY = Math.Clamp(newShearY, -89.0, 89.0);

            bone.Shear(newShearX, newShearY);

            if (this.globalState.setBasePos == false)
            {
                var animation = this.globalState.CurrentProject?.GetCurrentAnimation();
                if (animation != null && !animation.IsRun && bone.IsBone == true)
                {
                    animation.ShearBone(bone, bone.ShearX, bone.ShearY);
                }
            }
        }
    }
}

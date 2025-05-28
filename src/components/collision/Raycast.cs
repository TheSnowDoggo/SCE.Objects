using System.Diagnostics;
using System.Security.Cryptography;

namespace SCE
{
    public class Raycast : Collider
    {
        public float Length { get; set; }

        public Vector2 Direction { get; set; } = Vector2.Right;

        public Vector2 Offset { get; set; }
        
        public float Gradient()
        {
            return Direction.Y / Direction.X;
        }

        public Vector2 GlobalPosition()
        {
            return Holder.GlobalPosition + Offset;
        }

        public Vector2 GlobalEndPosition()
        {
            return GlobalPosition() + (Direction.SafeNormalize() * Length);
        }

        public Rect2D GlobalArea()
        {
            return new(GlobalPosition(), GlobalEndPosition());
        }

        public override bool CollidesWith(Collider other)
        {
            var dir = Direction.Magnitude();
            if (dir == 0)
            {
                return false;
            }

            if (other is Raycast rc)
            {
                if (Length == 0 || rc.Length == 0)
                {
                    return false;
                }

                var a1 = GlobalArea();
                var a2 = rc.GlobalArea();

                if (Direction.X == 0 || rc.Direction.X == 0)
                {
                    var x1 = a1.Left;
                    var x2 = a2.Left;
                    // Both Vertical
                    if (Direction.X == rc.Direction.X)
                    {
                        return x1 == x2 && !(a1.Bottom <= a2.Top || a1.Top >= a2.Bottom);
                    }
                    else
                    {
                        var isOther = rc.Direction.X == 0;

                        var m = isOther ? Gradient() : rc.Gradient();
                        var c = isOther ? MathUtils.LineSolveC(a1.Left, a1.Top, m) : MathUtils.LineSolveC(a2.Left, a2.Top, m);
                        var k = isOther ? x2 : x1;

                        var y = MathUtils.LineVerticalY(m, c, k);
                        return y >= a1.Top && y <= a1.Bottom;
                    }
                }

                float m1 = Gradient();
                var c1 = MathUtils.LineSolveC(a1.Left, a1.Top, m1);

                float m2 = rc.Gradient();
                var c2 = MathUtils.LineSolveC(a2.Left, a2.Top, m2);

                var x = MathUtils.LineIntersectX(m1, c1, m2, c2);
                return x >= a1.Left && x <= a1.Right;
            }

            if (other is CircleCollider cc)
            {
                var cPos = cc.GlobalPosition();

                var a = GlobalArea();

                if (a.Start.DistanceFrom(cPos) <= cc.Radius || a.End.DistanceFrom(cPos) <= cc.Radius)
                {
                    return true;
                }

                // Vertical
                if (Direction.X == 0)
                {
                    var x = a.Left;
                    foreach (var sol in MathUtils.CircleSolveY(x, cPos.X, cPos.Y, cc.Radius))
                    {
                        if (sol >= a.Top && sol <= a.Bottom)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    float m = Direction.Y / Direction.X;
                    float c = MathUtils.LineSolveC(a.Left, a.Top, m);

                    foreach (var sol in MathUtils.CircleLineSolveX(cPos.X, cPos.Y, cc.Radius, m, c))
                    {
                        if (sol >= a.Left && sol <= a.Right)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            if (other is BoxCollider bc)
            {
                var bcArea = bc.GlobalCollisionArea();

                var a = GlobalArea();

                if (bcArea.Contains(new Vector2(a.Left, a.Top)) || bcArea.Contains(new Vector2(a.Right, a.Bottom)))
                {
                    return false;
                }

                // Vertical
                if (Direction.X == 0)
                {
                    return a.Left >= bcArea.Left && a.Left <= bcArea.Right && !(a.Bottom <= bcArea.Top || a.Top >= bcArea.Bottom);
                }

                var m = Gradient();
                var c = MathUtils.LineSolveC(a.Left, a.Top, m);

                float y, x;

                y = MathUtils.LineVerticalY(m, c, bcArea.Left);
                if (y >= a.Top && y <= a.Bottom)
                {
                    return true;
                }

                y = MathUtils.LineVerticalY(m, c, bcArea.Right);
                if (y >= a.Top && y <= a.Bottom)
                {
                    return true;
                }

                x = MathUtils.LineIntersectX(m, c, 0, bcArea.Top);
                if (x >= a.Left && x <= a.Right)
                {
                    return true;
                }

                x = MathUtils.LineIntersectX(m, c, 0, bcArea.Bottom);
                if (x >= a.Left && x <= a.Right)
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}

using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;

namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class ShapeFuncs
    {
        public static double Area(this Shape shape) => shape switch
        {
            Shape.Circle c => Math.PI * c.Radius * c.Radius,
            Shape.Square s => s.SideLength * s.SideLength,
            Shape.Rectangle r => r.Width * r.Height,
            Shape.Triangle t => 0.5 * t.Base * t.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(shape), "Unknown shape type")
        };
    }
}

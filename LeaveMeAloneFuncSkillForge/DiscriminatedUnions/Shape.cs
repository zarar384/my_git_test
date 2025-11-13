namespace LeaveMeAloneFuncSkillForge.DiscriminatedUnions
{
    public abstract record Shape
    {
        public sealed record Circle(double Radius) : Shape;
        public sealed record Square(double SideLength) : Shape;
        public sealed record Rectangle(double Width, double Height) : Shape;
        public sealed record Triangle(double Base, double Height) : Shape;
    }
}

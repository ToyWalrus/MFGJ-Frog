namespace Utils
{
    public class MathHelpers
    {
        public static bool FloatEquals(float a, float b, float epsilon = .005f)
        {
            return System.Math.Abs(a - b) < epsilon;
        }
    }
}
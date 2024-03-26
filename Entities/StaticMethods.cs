namespace Inewi_Console.Entities
{
    internal class StaticMethods
    {
        public static string? GetChoice()
        {
            return Console.ReadLine();
        }

        public static int StringToIntExceptZero(string? value)
        {
            try
            {
                int num = int.Parse(value);
                return num;
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}

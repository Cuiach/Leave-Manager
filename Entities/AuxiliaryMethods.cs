﻿namespace Inewi_Console.Entities
{
    internal class AuxiliaryMethods
    {
        public static int ToInt(string? value)
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
        public static int GetId() 
        {
            Console.WriteLine("Insert id");
            var idAsString = (Console.ReadLine() ?? "0");
            int idOrZero;
            bool _ = int.TryParse(idAsString, out idOrZero);
            return idOrZero;
        }
        public static bool IsValidDate(string input)
        {
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result.ToString("yyyy-MM-dd") == input;
            }
            else
            {
                return false;
            }
        }
    }
}

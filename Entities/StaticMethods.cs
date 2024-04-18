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
        public static int CountLeaveLength(Leave leave)
        {
            return (leave.DateTo - leave.DateFrom).Days + 1;
        }
        public static void ShowMenu()
        {
            Console.WriteLine("1 Add employee");
            Console.WriteLine("2 Display all employees");
            Console.WriteLine("3 Search employee");
            Console.WriteLine("4 Remove employee");
            Console.WriteLine("5 Add employee's leave");
            Console.WriteLine("6 Display all leaves (you can add D - ON DEMAND only or/and add E - for 1 employee only: 6, 6D, 6E, 6ED)");
            Console.WriteLine("7 Remove leave");
            Console.WriteLine("8 Edit leave");
            Console.WriteLine("9 Edit employee's settings");
            Console.WriteLine(" If you want to see this menu insert 'm'");
            Console.WriteLine(" To exit insert 'x'");
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

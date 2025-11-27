using System.Reflection;

namespace PrintTester
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello World");


            string template = "^XA ^FO50,50^ADN,36,20^FDName: {NAME}^FS ^FO50,100^ADN,36,20^FDDate: {DATE}^FS ^XZ";


            var parameters = new Dictionary<string, string>
            {
                ["NAME"] = "Test Label",
                ["DATE"] = DateTime.Now.ToString()
            };

            string printerName = "";
            Console.Write("Enter Printer Name:");
            printerName = Console.ReadLine();


            
            bool ok =Print.Printx(template, printerName);

            Console.WriteLine(ok ? "Printed successfully!" : "Print failed!");
            Console.ReadLine();

        }
    }
}
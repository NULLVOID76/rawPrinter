using System.Reflection;
using System.Threading.Tasks;
using LabelPrintHelper;
namespace PrintTester
{
    public class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Hello World");

            IPrinterService printer = new PrinterService();

            string template = await File.ReadAllTextAsync("WarehouseTemplate.zpl");


            var parameters = new Dictionary<string, string>
            {
                ["MODELNAME"] = "Redmi A3x/4G/4GB/64GB",
                ["COLOR"] = "Midnight Black",
                ["IDSKU"] = "MZB0I5AIN",
                ["CBQTY"] = "480",
                ["PALLET_NO"] = "P24048RN6CI5BA00014",
                ["DATE"] = DateTime.Now.ToString("dd-MM-yyyy"),
                ["TIME"] = DateTime.Now.ToString("HH:mm:ss"),
                ["FGCODE"] = "9510C3Y2E0021"
            };

            string printerName = "";
            Console.Write("Enter Printer Name:");
            printerName = Console.ReadLine();


            try
            {
                //bool ok = printer.Print(template, printerName);
                bool ok= await printer.PrintAsync(printerName, template, parameters);
                //bool ok = printer.Print(printerName, template, parameters);

                Console.WriteLine(ok ? "Printed successfully!" : "Print failed!");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Print failed!!!!");
                Console.WriteLine(ex.Message);
            }

        }
    }
}
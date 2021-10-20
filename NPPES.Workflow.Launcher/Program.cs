using System;
using NPPES.Loader;

namespace NPPES.Workflow.Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            NPPESLoader loader = new NPPESLoader();
            loader.Run();
            Console.ReadLine();
        }

    }
}

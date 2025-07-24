namespace PcTools
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            //MainForm.MachineCheck();
            Application.Run(new MainForm());
        }
    }
}
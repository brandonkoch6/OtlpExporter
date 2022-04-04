namespace OtlpExporter
{
    /// <summary>
    /// Main entry point for exporter.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Invoke with dotnet run
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            OtlpExporter.Run("http://localhost:4317", "grpc");
        }
    }
}

using System;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OtlpExporter
{
    internal static class OtlpExporter
    {
        internal static object Run(string endpoint, string protocol)
        {
            return RunWithActivitySource(endpoint, protocol);
        }

        private static object RunWithActivitySource(string endpoint, string protocol)
        {
            // Adding the OtlpExporter creates a GrpcChannel.
            // This switch must be set before creating a GrpcChannel when calling an insecure gRPC service.
            //
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var otlpExportProtocol = ToOtlpExportProtocol(protocol);

            // Enable OpenTelemetry for the sources "Samples.SampleServer" and "Samples.SampleClient"
            // and use OTLP exporter.
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                    .AddSource("Samples.SampleClient", "Samples.SampleServer")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("otlp-test"))
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(endpoint);
                    })
                    .Build();

            // The above line is required only in Applications
            // which decide to use OpenTelemetry.
            using (var sample = new SampleTelemetry())
            {
                sample.Start();

                System.Console.WriteLine("Traces are being created and exported " +
                    "to the OpenTelemetry Collector in the background. " +
                    "Press ENTER to stop.");
                System.Console.ReadLine();
            }

            return null;
        }

        public enum OtlpExportProtocol : byte
        {
            Grpc,
            HttpProtobuf
        }

        private static OtlpExportProtocol? ToOtlpExportProtocol(string protocol) =>
            protocol.Trim().ToLower() switch
            {
                "grpc" => OtlpExportProtocol.Grpc,
                "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
                _ => null,
            };
    }
}

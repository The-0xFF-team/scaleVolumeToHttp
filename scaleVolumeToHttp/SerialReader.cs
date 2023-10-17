using System.Globalization;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace scaleVolumeToHttp;

public class SerialReader : BackgroundService
{
    private readonly ILogger<SerialReader> _logger;
    private const double Threshold = -100d;

    public SerialReader(ILogger<SerialReader> logger)
    {
        _logger = logger;
        Readings = new List<Reading>();
    }

    public List<Reading> Readings { get; set; }
    public double Consumed { get; set; }
    public bool GlassIsPresent { get; private set; } = true;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serialPort = new SerialPort
        {
            BaudRate = 115200,
            PortName = "COM3",
            ReadTimeout = 5000,
            WriteTimeout = 5000
        };


        serialPort.Open();
        using var reader = new StreamReader(serialPort.BaseStream);
        while (serialPort.IsOpen)
        {
            var rawData = await reader.ReadLineAsync(stoppingToken);
            var data = Regex.Match(rawData, "average:\t(.*)").Groups[1].Value;
            _logger.LogInformation(data);
            if (!string.IsNullOrEmpty(data))
            {
                var volumeInMl = double.Parse(data, CultureInfo.InvariantCulture);
                GlassIsPresent = volumeInMl > Threshold;
                Readings.Add(new Reading(volumeInMl));
            }

            var valuesAroundThreshold = GetValuesAroundThreshold();
            Consumed = valuesAroundThreshold.Sum(_ => _.Previous - _.Next);
        }
    }

    private IEnumerable<ValuesAroundThreshold> GetValuesAroundThreshold()
    {
        var result = new List<ValuesAroundThreshold>();

        for (var i = 0; i < Readings.Count; i++)
        {
            var nextValue = Readings.Skip(i).FirstOrDefault(r => r.VolumeInMl > Threshold)?.VolumeInMl ?? 0d;
            if (Readings[i].VolumeInMl <= Threshold && i-1 >=0 && Readings[i - 1].VolumeInMl > Threshold)
            {
                var values = new ValuesAroundThreshold(Readings[i - 1].VolumeInMl, nextValue);
                result.Add(values);
            }
        }
        return result;
    }

    private record ValuesAroundThreshold(double Previous, double Next);
}
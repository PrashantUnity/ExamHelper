using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;

namespace ExamHelper.Worker;


public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient;
    private readonly string EndPointUrl = "https://pkwltrp1-7192.inc1.devtunnels.ms/api/Image/upload";
    private readonly string envVariableUri = "PrashantUnityServiceUriLoaction";
    private readonly string envVariableDelay = "PrashantUnityServiceDelayTime";
    private int delaytime = 10000;

    static int height = 1080;
    static int width = 1920;
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Service started");
        // Read the value of the environment variable if needed
        // EndPointUrl = Environment.GetEnvironmentVariable(envVariableUri);
        // delaytime = Convert.ToInt32(Environment.GetEnvironmentVariable(envVariableDelay));
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _httpClient.Dispose();
        _logger.LogInformation("Service stopped");
        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Taking screenshot at {DateTime.Now}");
            try
            {
                await CaptureAndUploadScreenshot();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while capturing and uploading screenshot");
            }
            await Task.Delay(delaytime, stoppingToken);
        }
    }

    async Task CaptureAndUploadScreenshot()
    {
        using var screenshot = CaptureScreen();
        using var stream = new MemoryStream();
        screenshot.Save(stream, ImageFormat.Png);
        stream.Seek(0, SeekOrigin.Begin);
        await UploadToWebAPI(stream);
    }

    Bitmap CaptureScreen()
    {
        var bitmap = new Bitmap(width, height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
        }
        return bitmap;
    }

    async Task UploadToWebAPI(MemoryStream stream)
    {
        using var content = new MultipartFormDataContent();
        var filePath = "Image.png";
        var imageData = stream.ToArray();
        var imageContent = new ByteArrayContent(imageData);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        content.Add(imageContent, "file", Path.GetFileName(filePath));
        var response = await _httpClient.PostAsync(EndPointUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Image uploaded successfully. Server response: {responseContent}");
        }
        else
        {
            _logger.LogError($"Error in UploadToWebAPI Method. Status code: {response.StatusCode}");
        }
    }
}

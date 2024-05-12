using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;

string EndPointUrl = "https://pkwltrp1-7192.inc1.devtunnels.ms/api/Image/upload";
while (true)
{
    //TakeScreenShot();
    await CaptureAndUploadScreenshot(EndPointUrl);
    Thread.Sleep(5000);
}
static async Task CaptureAndUploadScreenshot(string endPointUrl)
{
    try
    {
        Bitmap screenshot = CaptureScreen();
        using var stream = new MemoryStream();
        screenshot.Save(stream, ImageFormat.Png);
        stream.Seek(0, SeekOrigin.Begin);
        await UploadToWebAPI(stream, endPointUrl);
        screenshot.Dispose();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

static Bitmap CaptureScreen()
{
    // Capture the screen
    Bitmap bitmap = new(1920, 1080);
    using (Graphics graphics = Graphics.FromImage(bitmap))
    {
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
    }
    return bitmap;
}

static async Task UploadToWebAPI(MemoryStream stream, string endPointUrl)
{
    try
    {
        using var client = new HttpClient();
        using var content = new MultipartFormDataContent();
        var filePath = "Image.png";
        byte[] imageData = stream.ToArray();
        var imageContent = new ByteArrayContent(imageData);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        content.Add(imageContent, "file", Path.GetFileName(filePath));
        var response = await client.PostAsync(endPointUrl, content);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Image uploaded successfully. Server response: {responseContent}");
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}
static void WithMain(string[] args)
{
    Dictionary<string, string> arguments = ParseArguments(args);

    if (arguments.ContainsKey("-uri"))
    {
        string uri = arguments["-uri"];
    }
    else
    {
        Console.WriteLine("Invalid arguments. Usage: MyConsoleApp.exe -uri <uri> -path <path>");
    }
}

static Dictionary<string, string> ParseArguments(string[] args)
{
    Dictionary<string, string> arguments = new Dictionary<string, string>();

    for (int i = 0; i < args.Length - 1; i += 2)
    {
        if (args[i].StartsWith("-") && (i + 1) < args.Length)
        {
            string key = args[i];
            string value = args[i + 1];
            arguments[key] = value;
        }
    }

    return arguments;
}


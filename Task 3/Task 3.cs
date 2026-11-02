using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        await ApiFetch();
    }

    static async Task ApiFetch()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Read from .env file
                string baseUrl = GetEnvironmentVariableFromDotEnv("API_BASE_URL") ?? "";
                string url = baseUrl;
                
                HttpResponseMessage response = await client.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"HTTP error! Status: {response.StatusCode}");

                string json = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(json);
                
                Console.WriteLine("First Post Title: " + doc.RootElement[0].GetProperty("title").GetString());
                
                doc.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    
    // Code to pull straight from .env file
    static string GetEnvironmentVariableFromDotEnv(string variableName)
    {
        try
        {
            // Look for .env file in the current directory
            string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Task 3/.env");
            
            if (File.Exists(envFilePath))
            {
                string[] lines = File.ReadAllLines(envFilePath);
                
                foreach (string line in lines)
                {
                    if (line.StartsWith(variableName + "="))
                    {
                        return line.Substring(variableName.Length + 1);
                    }
                }
            }
        }
        catch
        {
            // If there's any error reading the file, just return null
        }
        
        return null;
    }// Task 3 – Simulate a real-world API call using JSONPlaceholder
// Endpoint: https://jsonplaceholder.typicode.com/posts
// Requirements:
//   - GET request
//   - Parse JSON and extract the "title" field
//   - Error handling
//   - Store base URL in .env and make sure .gitignore excludes it

using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        await ApiFetch();
    }

    static async Task ApiFetch()
    {
        try
        {
            // 1. Read base URL from .env
            string baseUrl = GetEnvironmentVariableFromDotEnv("API_BASE_URL");

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new Exception("API_BASE_URL is missing or empty in the .env file.");
            }

            Console.WriteLine($"Calling API: {baseUrl}");

            using (HttpClient client = new HttpClient())
            {
                // Optional: set a timeout so the program does not hang forever
                client.Timeout = TimeSpan.FromSeconds(15);

                HttpResponseMessage response = await client.GetAsync(baseUrl);

                // 2. Error handling for HTTP status codes
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"HTTP error! Status: {(int)response.StatusCode} {response.StatusCode}");
                }

                string json = await response.Content.ReadAsStringAsync();

                // 3. Parse JSON
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    JsonElement root = doc.RootElement;

                    // The endpoint returns an array of posts
                    if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                    {
                        throw new Exception("Unexpected JSON structure: expected a non-empty array of posts.");
                    }

                    // Extract and print titles (showing first 5 for readability)
                    Console.WriteLine("\n--- Post Titles ---");
                    int count = 0;
                    foreach (JsonElement post in root.EnumerateArray())
                    {
                        if (post.TryGetProperty("title", out JsonElement titleElement))
                        {
                            string title = titleElement.GetString() ?? "(null title)";
                            Console.WriteLine($"{++count}. {title}");

                            // Stop after 5 titles so the console is not flooded
                            if (count >= 5) break;
                        }
                    }

                    Console.WriteLine($"\n(Showing first {count} titles. Total posts available: {root.GetArrayLength()})");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            // Network / connection problems
            Console.WriteLine("Network error: " + ex.Message);
        }
        catch (TaskCanceledException)
        {
            // Timeout
            Console.WriteLine("Error: The request timed out.");
        }
        catch (JsonException ex)
        {
            // Invalid JSON
            Console.WriteLine("JSON parsing error: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Any other unexpected error
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    /// <summary>
    /// Reads a simple KEY=VALUE pair from a .env file located next to the executable
    /// or in the current working directory. Falls back gracefully if the file is missing.
    /// </summary>
    static string GetEnvironmentVariableFromDotEnv(string variableName)
    {
        try
        {
            // Try several common locations so the program works no matter where it is run from
            string[] possiblePaths =
            {
                Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                Path.Combine(AppContext.BaseDirectory, ".env"),
                Path.Combine(Directory.GetCurrentDirectory(), "Task 3", ".env"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "Task 3", ".env")
            };

            foreach (string envFilePath in possiblePaths)
            {
                if (!File.Exists(envFilePath)) continue;

                foreach (string line in File.ReadAllLines(envFilePath))
                {
                    string trimmed = line.Trim();

                    // Skip empty lines and comments
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;

                    if (trimmed.StartsWith(variableName + "=", StringComparison.OrdinalIgnoreCase))
                    {
                        // Return everything after the first '='
                        return trimmed.Substring(variableName.Length + 1).Trim();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Warning while reading .env: " + ex.Message);
        }

        return null;
    }
}
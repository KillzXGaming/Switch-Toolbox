using System.Diagnostics;
using System.IO.Compression;

// Set the URL, name, and filename of the file to download
string downloadURL = @"https://github.com/KillzXGaming/Switch-Toolbox/releases/download/Latest/Toolbox-Latest.zip";
string downloadFilename = $"Toolbox-Latest.zip";

// Get the current directory and check if the "-h" or "--hashes" option was passed as an argument
string currentDir = Environment.CurrentDirectory;
bool keepHashes = args.Contains("-h") || args.Contains("--hashes");

// Create a new HttpClient and download the file from the specified URL
Console.WriteLine($"Downloading {downloadURL}...");
using HttpClient httpClient = new();
HttpResponseMessage response = httpClient.GetAsync(downloadURL).Result;

try
{ 
    response.EnsureSuccessStatusCode(); 
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to download update!\n{ex}");
}

// Save the downloaded file to disk
Console.WriteLine($"Downloaded {downloadFilename}, saving to disk...");
byte[] content = response.Content.ReadAsByteArrayAsync().Result;
File.WriteAllBytes(downloadFilename, content);

// Extract the contents of the downloaded zip file if it's newer
using (ZipArchive archive = ZipFile.OpenRead(downloadFilename))
{
    if (updateNeeded(archive))
    {
        Console.WriteLine($"Extracting contents of {downloadFilename}...");
        // Loop through all the entries in the zip archive
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            // Filter out any folders
            if (!entry.FullName.EndsWith("/") &&
                // Ignore updater
                !(entry.Name.Equals("updater.exe", StringComparison.OrdinalIgnoreCase) ||
              entry.Name.Equals("updater.dll", StringComparison.OrdinalIgnoreCase) ||
              entry.Name.Equals("updater.pdb", StringComparison.OrdinalIgnoreCase)) &&
              // Skip the "Hashes" folder if the "-h" or "--hashes" option was passed
              !(keepHashes && entry.FullName.StartsWith("Hashes/", StringComparison.OrdinalIgnoreCase)))
            {
                // Construct the target path for the extracted file and extract it
                string targetPath = Path.Combine(currentDir, entry.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                entry.ExtractToFile(targetPath, overwrite: true);
            }
        }
    }
}

// Zip not needed anymore
File.Delete(downloadFilename + ".zip");

// If the "-b" or "--boot" option was passed, launch the extracted application
if (args.Contains("-b") || args.Contains("--boot"))
{
    Console.WriteLine("Booting...");
    Process.Start(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "Toolbox.exe"));
}

if (args.Contains("-e") || args.Contains("--exit"))
{
    Console.WriteLine("\nPress any key to continue...");
    Console.Read();
}    

bool updateNeeded(ZipArchive archive)
{
    // Get the ZipArchiveEntry for the Toolbox.exe file
    ZipArchiveEntry entry = archive.GetEntry("Toolbox.exe");

    if (entry != null)
    {
        // Construct the target path for the extracted file
        string targetPath = Path.Combine(currentDir, entry.FullName);

        // Check if the file already exists
        if (File.Exists(targetPath))
        {
            // Get the last modified time of the file in the zip archive and on disk
            DateTime lastWriteTimeZip = entry.LastWriteTime.DateTime;
            DateTime lastWriteTimeFile = File.GetLastWriteTime(targetPath);

            // If the modified time of the file in the zip archive is greater than the modified time on disk,
            // an update is needed
            if (lastWriteTimeZip > lastWriteTimeFile)
            {
                return true;
            }
            // The file is already up-to-date
            Console.WriteLine($"{entry.Name} is up-to-date, skipping extraction.");
            return false;
        }
        // The file doesn't exist on disk, so an update is needed
        return true;
    }

    // If the Toolbox.exe file isn't found in the archive, assume an update is needed
    return true;
}
using System.Diagnostics;

Console.WriteLine("Oracle Cloud - Instance create ...");
await SendLineNotify("Start create VM ....");
while (true) {
    try {
        string workDirPath = "/root/oci-arm-host-capacity/";
        //* Create your Process
        Process process = new Process();
        process.StartInfo.FileName = "php";

        // Need to Edit path
        process.StartInfo.WorkingDirectory = workDirPath;
        process.StartInfo.Arguments = workDirPath + "index.php .env";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        //* Set ONLY ONE handler here.
        process.ErrorDataReceived += new DataReceivedEventHandler(ErrorOutputHandler);
        //* Start process
        process.Start();
        //* Read one element asynchronously
        process.BeginErrorReadLine();
        //* Read the other one synchronously
        string output = process.StandardOutput.ReadToEnd();

        var currDate = DateTime.Now.ToString(" dd-MM-yyyy HH:mm:ss");
        Console.WriteLine(currDate);
        Console.WriteLine(output);
        process.WaitForExit();

        // Need to Edit path
        File.AppendAllText(workDirPath + "create-vm.log",output);

        if (!output.Contains("Out of host capacity.")) {
            await SendLineNotify(output);
        }

    } catch (Exception ex) {
        Console.WriteLine("ERROR: " + ex.Message);
    }
    Thread.Sleep(60001);
}

static void ErrorOutputHandler(object sendingProcess, DataReceivedEventArgs outLine) {
    if (!string.IsNullOrEmpty(outLine.Data))
        Console.WriteLine("ERROR:" + outLine.Data);
}


static async Task SendLineNotify(string message) {
    // Need to Edit Line token
    string lineToken = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
    string uri = "https://notify-api.line.me/api/notify";
    HttpClient client = new HttpClient();

    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + lineToken);
    StringContent httpContent = new StringContent("message=" + message, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

    var stringTask = client.PostAsync(uri, httpContent);

    var msg = await stringTask;
    Console.Write(msg);
}

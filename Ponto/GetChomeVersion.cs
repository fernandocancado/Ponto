using System.Diagnostics;

namespace Ponto
{
    public class GetChomeVersion
    {
        public static string GetChromeVersion()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = "/c reg query \"HKEY_CURRENT_USER\\Software\\Google\\Chrome\\BLBeacon\" /v version",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Verifica se a saída contém a palavra "version" e extrai o valor correspondente
                foreach (var line in output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Contains("version"))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        return parts[^1]; // Retorna a última parte, que deve ser a versão
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

    }
}

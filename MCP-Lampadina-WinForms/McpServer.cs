using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace McpLampadinaWinForms
{
    public class McpServer
    {
        private HttpListener _listener;
        private Thread _listenerThread;
        private bool _isRunning;
        private LampadinaForm _form;

        public McpServer(LampadinaForm form)
        {
            _form = form;
        }

        public void Start(int port = 5000)
        {
            if (_isRunning) return;

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://localhost:{port}/");
                _listener.Start();
                _isRunning = true;

                _listenerThread = new Thread(ListenForRequests)
                {
                    IsBackground = true
                };
                _listenerThread.Start();

                Console.WriteLine($"MCP Server avviato su porta {port}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore avvio server MCP: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            _listener?.Stop();
            _listener?.Close();
            _listenerThread?.Join(1000);
        }

        private void ListenForRequests()
        {
            while (_isRunning)
            {
                try
                {
                    var context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        Console.WriteLine($"Errore listener: {ex.Message}");
                    }
                }
            }
        }

        private void ProcessRequest(object state)
        {
            var context = (HttpListenerContext)state;
            var request = context.Request;
            var response = context.Response;

            try
            {
                _form.LogDebug($"🌐 {request.HttpMethod} {request.Url.AbsolutePath} da {request.RemoteEndPoint}");

                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "OPTIONS")
                {
                    _form.LogDebug("✅ Risposta OPTIONS CORS");
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                string responseText = "";

                if (request.Url.AbsolutePath == "/mcp")
                {
                    if (request.HttpMethod == "POST")
                    {
                        string requestBody = "";
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            requestBody = reader.ReadToEnd();
                        }

                        _form.LogDebug($"📨 Body ricevuto: {requestBody.Substring(0, Math.Min(200, requestBody.Length))}...");
                        responseText = ProcessMcpRequest(requestBody);
                        _form.LogDebug($"📤 Risposta inviata: {responseText.Substring(0, Math.Min(100, responseText.Length))}...");
                    }
                    else if (request.HttpMethod == "GET")
                    {
                        _form.LogDebug("🔍 Richiesta GET info server");
                        responseText = GetServerInfo();
                    }
                }
                else
                {
                    _form.LogDebug($"❌ Path non trovato: {request.Url.AbsolutePath}");
                    response.StatusCode = 404;
                    responseText = "Not Found";
                }

                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore processing request: {ex.Message}");
                try
                {
                    response.StatusCode = 500;
                    response.Close();
                }
                catch { }
            }
        }

        private string GetServerInfo()
        {
            return @"{
                ""jsonrpc"": ""2.0"",
                ""result"": {
                    ""protocolVersion"": ""2024-11-05"",
                    ""capabilities"": {
                        ""tools"": {}
                    },
                    ""serverInfo"": {
                        ""name"": ""mcp-lampadina-winforms"",
                        ""version"": ""1.0.0""
                    }
                }
            }";
        }

        private string ProcessMcpRequest(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return CreateErrorResponse(-32600, "Invalid Request", "Request body is empty");
                }

                var jsonStart = requestBody.IndexOf('{');
                var jsonEnd = requestBody.LastIndexOf('}');
                if (jsonStart == -1 || jsonEnd == -1)
                {
                    return CreateErrorResponse(-32700, "Parse error", "Invalid JSON");
                }

                var jsonContent = requestBody.Substring(jsonStart, jsonEnd - jsonStart + 1);

                if (jsonContent.Contains("\"method\":\"tools/list\""))
                {
                    return GetToolsList();
                }
                else if (jsonContent.Contains("\"method\":\"tools/call\""))
                {
                    return ProcessToolCall(jsonContent);
                }
                else if (jsonContent.Contains("\"method\":\"initialize\""))
                {
                    return GetInitializeResponse();
                }
                else
                {
                    return CreateErrorResponse(-32601, "Method not found", "The requested method is not supported");
                }
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(-32603, "Internal error", ex.Message);
            }
        }

        private string GetInitializeResponse()
        {
            return @"{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""result"": {
                    ""protocolVersion"": ""2024-11-05"",
                    ""capabilities"": {
                        ""tools"": {}
                    },
                    ""serverInfo"": {
                        ""name"": ""mcp-lampadina-winforms"",
                        ""version"": ""1.0.0""
                    }
                }
            }";
        }

        private string GetToolsList()
        {
            return @"{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""result"": {
                    ""tools"": [
                        {
                            ""name"": ""lampadina_stato"",
                            ""description"": ""Ottiene lo stato attuale della lampadina (accesa/spenta, colore, luminosità)"",
                            ""inputSchema"": {
                                ""type"": ""object"",
                                ""properties"": {},
                                ""required"": []
                            }
                        },
                        {
                            ""name"": ""lampadina_toggle"",
                            ""description"": ""Accende o spegne la lampadina"",
                            ""inputSchema"": {
                                ""type"": ""object"",
                                ""properties"": {},
                                ""required"": []
                            }
                        },
                        {
                            ""name"": ""lampadina_colore"",
                            ""description"": ""Cambia il colore della lampadina"",
                            ""inputSchema"": {
                                ""type"": ""object"",
                                ""properties"": {
                                    ""colore"": {
                                        ""type"": ""string"",
                                        ""description"": ""Il colore in formato esadecimale (es: #ff0000 per rosso, #00ff00 per verde)"",
                                        ""pattern"": ""^#[0-9a-fA-F]{6}$""
                                    }
                                },
                                ""required"": [""colore""]
                            }
                        },
                        {
                            ""name"": ""lampadina_luminosita"",
                            ""description"": ""Regola la luminosità della lampadina"",
                            ""inputSchema"": {
                                ""type"": ""object"",
                                ""properties"": {
                                    ""luminosita"": {
                                        ""type"": ""number"",
                                        ""description"": ""Livello di luminosità da 0 a 100"",
                                        ""minimum"": 0,
                                        ""maximum"": 100
                                    }
                                },
                                ""required"": [""luminosita""]
                            }
                        },
                        {
                            ""name"": ""lampadina_preset"",
                            ""description"": ""Applica un preset di colore alla lampadina"",
                            ""inputSchema"": {
                                ""type"": ""object"",
                                ""properties"": {
                                    ""preset"": {
                                        ""type"": ""string"",
                                        ""description"": ""Nome del preset colore"",
                                        ""enum"": [""bianco"", ""rosso"", ""verde"", ""blu"", ""giallo"", ""magenta"", ""ciano"", ""arancione""]
                                    }
                                },
                                ""required"": [""preset""]
                            }
                        }
                    ]
                }
            }";
        }

        private string ProcessToolCall(string jsonContent)
        {
            try
            {
                string toolName = ExtractJsonValue(jsonContent, "name");
                string arguments = ExtractJsonValue(jsonContent, "arguments");

                string result = "";
                switch (toolName)
                {
                    case "lampadina_stato":
                        result = GetLampadinaStato();
                        break;
                    case "lampadina_toggle":
                        result = ToggleLampadina();
                        break;
                    case "lampadina_colore":
                        string colore = ExtractJsonValue(arguments, "colore");
                        result = CambiaColore(colore);
                        break;
                    case "lampadina_luminosita":
                        string luminositaStr = ExtractJsonValue(arguments, "luminosita");
                        if (int.TryParse(luminositaStr, out int luminosita))
                        {
                            result = CambiaLuminosita(luminosita);
                        }
                        else
                        {
                            result = "❌ Errore: luminosità non valida";
                        }
                        break;
                    case "lampadina_preset":
                        string preset = ExtractJsonValue(arguments, "preset");
                        result = ApplicaPreset(preset);
                        break;
                    default:
                        result = $"❌ Errore: strumento '{toolName}' non riconosciuto";
                        break;
                }

                return $@"{{
                    ""jsonrpc"": ""2.0"",
                    ""id"": 1,
                    ""result"": {{
                        ""content"": [
                            {{
                                ""type"": ""text"",
                                ""text"": ""{EscapeJsonString(result)}""
                            }}
                        ]
                    }}
                }}";
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(-32603, "Internal error", ex.Message);
            }
        }

        private string ExtractJsonValue(string json, string key)
        {
            string searchKey = $"\"{key}\":";
            int startIndex = json.IndexOf(searchKey);
            if (startIndex == -1) return "";

            startIndex += searchKey.Length;
            while (startIndex < json.Length && char.IsWhiteSpace(json[startIndex]))
                startIndex++;

            if (startIndex >= json.Length) return "";

            if (json[startIndex] == '"')
            {
                startIndex++;
                int endIndex = json.IndexOf('"', startIndex);
                if (endIndex == -1) return "";
                return json.Substring(startIndex, endIndex - startIndex);
            }
            else if (json[startIndex] == '{')
            {
                int braceCount = 1;
                int endIndex = startIndex + 1;
                while (endIndex < json.Length && braceCount > 0)
                {
                    if (json[endIndex] == '{') braceCount++;
                    else if (json[endIndex] == '}') braceCount--;
                    endIndex++;
                }
                return json.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
                int endIndex = startIndex;
                while (endIndex < json.Length && json[endIndex] != ',' && json[endIndex] != '}' && json[endIndex] != ']')
                    endIndex++;
                return json.Substring(startIndex, endIndex - startIndex).Trim();
            }
        }

        private string EscapeJsonString(string str)
        {
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private string GetLampadinaStato()
        {
            if (_form.InvokeRequired)
            {
                return (string)_form.Invoke(new Func<string>(() =>
                {
                    string statoTesto = _form.IsAccesa ? "🟢 Accesa" : "🔴 Spenta";
                    return $"🔌 Stato della lampadina:\\n• Stato: {statoTesto}\\n• Colore: {_form.Colore}\\n• Luminosità: {_form.Luminosita}%";
                }));
            }
            else
            {
                string statoTesto = _form.IsAccesa ? "🟢 Accesa" : "🔴 Spenta";
                return $"🔌 Stato della lampadina:\\n• Stato: {statoTesto}\\n• Colore: {_form.Colore}\\n• Luminosità: {_form.Luminosita}%";
            }
        }

        private string ToggleLampadina()
        {
            if (_form.InvokeRequired)
            {
                return (string)_form.Invoke(new Func<string>(() =>
                {
                    _form.ToggleLampadina();
                    return _form.IsAccesa ? "✅ Lampadina accesa" : "✅ Lampadina spenta";
                }));
            }
            else
            {
                _form.ToggleLampadina();
                return _form.IsAccesa ? "✅ Lampadina accesa" : "✅ Lampadina spenta";
            }
        }

        private string CambiaColore(string colore)
        {
            if (string.IsNullOrEmpty(colore) || !colore.StartsWith("#") || colore.Length != 7)
            {
                return "❌ Errore: colore non valido. Usa formato esadecimale (es: #ff0000)";
            }

            if (_form.InvokeRequired)
            {
                return (string)_form.Invoke(new Func<string>(() =>
                {
                    _form.CambiaColore(colore);
                    return $"🎨 Colore cambiato a {colore}";
                }));
            }
            else
            {
                _form.CambiaColore(colore);
                return $"🎨 Colore cambiato a {colore}";
            }
        }

        private string CambiaLuminosita(int luminosita)
        {
            if (luminosita < 0 || luminosita > 100)
            {
                return "❌ Errore: luminosità deve essere tra 0 e 100";
            }

            if (_form.InvokeRequired)
            {
                return (string)_form.Invoke(new Func<string>(() =>
                {
                    _form.CambiaLuminosita(luminosita);
                    return $"💡 Luminosità regolata al {luminosita}%";
                }));
            }
            else
            {
                _form.CambiaLuminosita(luminosita);
                return $"💡 Luminosità regolata al {luminosita}%";
            }
        }

        private string ApplicaPreset(string preset)
        {
            var presets = new Dictionary<string, string>
            {
                {"bianco", "#ffffff"},
                {"rosso", "#ff0000"},
                {"verde", "#00ff00"},
                {"blu", "#0000ff"},
                {"giallo", "#ffff00"},
                {"magenta", "#ff00ff"},
                {"ciano", "#00ffff"},
                {"arancione", "#ff6600"}
            };

            if (!presets.ContainsKey(preset.ToLower()))
            {
                return $"❌ Errore: preset '{preset}' non riconosciuto. Preset disponibili: {string.Join(", ", presets.Keys)}";
            }

            string colore = presets[preset.ToLower()];

            if (_form.InvokeRequired)
            {
                return (string)_form.Invoke(new Func<string>(() =>
                {
                    _form.CambiaColore(colore);
                    return $"🎯 Preset \"{preset}\" applicato ({colore})";
                }));
            }
            else
            {
                _form.CambiaColore(colore);
                return $"🎯 Preset \"{preset}\" applicato ({colore})";
            }
        }

        private string CreateErrorResponse(int code, string message, string data = null)
        {
            string errorJson = $@"{{
                ""code"": {code},
                ""message"": ""{EscapeJsonString(message)}""";

            if (!string.IsNullOrEmpty(data))
            {
                errorJson += $@",
                ""data"": ""{EscapeJsonString(data)}""";
            }

            errorJson += "}";

            return $@"{{
                ""jsonrpc"": ""2.0"",
                ""id"": 1,
                ""error"": {errorJson}
            }}";
        }
    }
}
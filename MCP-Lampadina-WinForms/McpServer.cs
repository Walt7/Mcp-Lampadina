using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace McpLampadinaWinForms
{
    public class McpServer
    {
        private HttpListener _listener;
        private Thread _listenerThread;
        private volatile bool _isRunning;
        private readonly LampadinaForm _form;
        private readonly object _lockObject = new object();

        private const string PROTOCOL_VERSION = "2025-06-18";
        private const string SERVER_NAME = "mcp-lampadina-winforms";
        private const string SERVER_VERSION = "1.0.0";
        private const int THREAD_JOIN_TIMEOUT = 5000;

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
            lock (_lockObject)
            {
                if (!_isRunning) return;
                _isRunning = false;
            }

            try
            {
                _listener?.Stop();
                _listener?.Close();
                _listenerThread?.Join(THREAD_JOIN_TIMEOUT);
            }
            catch (Exception ex)
            {
                _form.LogDebug($"⚠️ Errore durante stop server: {ex.Message}");
            }
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
            string responseText = "";
            int statusCode = 200;
            string requestId = "null"; // Valore di default

            try
            {
                _form.LogDebug($"🌐 {request.HttpMethod} {request.Url.AbsolutePath} da {request.RemoteEndPoint}");

                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                response.ContentType = "application/json";

                if (request.HttpMethod == "OPTIONS")
                {
                    _form.LogDebug("✅ Risposta OPTIONS CORS");
                    statusCode = 200;
                    responseText = ""; // Nessun corpo per OPTIONS
                    return; // Uscita anticipata, il finally si occuperà della chiusura
                }

                string requestBody = ReadRequestBody(request);
                // Estrai l'ID il prima possibile
                if (!string.IsNullOrEmpty(requestBody))
                {
                    string extractedId = ExtractJsonValue(requestBody, "id");
                    if (!string.IsNullOrEmpty(extractedId))
                    {
                        requestId = extractedId;
                    }
                }

                if (request.Url.AbsolutePath == "/mcp")
                {
                    if (request.HttpMethod == "POST")
                    {
                        if (string.IsNullOrEmpty(requestBody))
                        {
                            statusCode = 400;
                            responseText = CreateErrorResponse(requestId, -32600, "Invalid Request", "Request body is empty");
                        }
                        else
                        {
                            _form.LogDebug($"📨 Body ricevuto: {requestBody}");
                            responseText = ProcessMcpRequest(requestBody);
                            _form.LogDebug($"📤 Risposta inviata: {responseText}");
                        }
                    }
                    else if (request.HttpMethod == "GET")
                    {
                        _form.LogDebug("🔍 Richiesta GET info server");
                        responseText = GetServerInfo("null"); // GET non ha un ID di richiesta
                    }
                }
                else
                {
                    _form.LogDebug($"❌ Path non trovato: {request.Url.AbsolutePath}");
                    statusCode = 404;
                    responseText = "Not Found";
                    response.ContentType = "text/plain"; // 404 non è JSON-RPC
                }
            }
            catch (Exception ex)
            {
                _form.LogDebug($"❌ Errore elaborazione richiesta: {ex.Message}");
                statusCode = 500;
                responseText = CreateErrorResponse(requestId, -32603, "Internal error", ex.Message);
            }
            finally
            {
                // Blocco centralizzato per inviare e chiudere la risposta.
                // Questo garantisce una chiusura pulita in ogni scenario (successo, errore, OPTIONS).
                try
                {
                    response.StatusCode = statusCode;
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                        response.ContentLength64 = buffer.Length;
                        response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                    response.OutputStream.Close(); // Chiudi prima lo stream...
                    response.Close();              // ...e poi la risposta.
                }
                catch (Exception ex)
                {
                    // Se anche la chiusura fallisce, logga l'errore ma non fare altro
                    // per evitare eccezioni non gestite nel thread pool.
                    _form.LogDebug($"⚠️ Errore durante la chiusura della risposta: {ex.Message}");
                }
            }
        }
        // MODIFICATO: Aggiunto parametro 'id'
        private string GetServerInfo(string id)
        {
            return $@"{{
                ""jsonrpc"": ""2.0"",
                ""id"": {FormatJsonId(id)},
                ""result"": {{
                    ""protocolVersion"": ""{PROTOCOL_VERSION}"",
                    ""capabilities"": {{
                        ""tools"": {{}}
                    }},
                    ""serverInfo"": {{
                        ""name"": ""{SERVER_NAME}"",
                        ""version"": ""{SERVER_VERSION}""
                    }}
                }}
            }}";
        }

        private string ProcessMcpRequest(string requestBody)
        {
            string id = ExtractJsonValue(requestBody, "id"); // MODIFICATO: Estrae l'ID
            if (string.IsNullOrEmpty(id))
            {
                id = "null"; // Default a null se non trovato (notifica o richiesta malformata)
            }

            try
            {
                string method = ExtractJsonValue(requestBody, "method");
                _form.LogDebug($"🔍 Metodo estratto: '{method}', ID: {id}");

                switch (method)
                {
                    case "tools/list":
                        return GetToolsList(id); // MODIFICATO: Passa l'ID
                    case "tools/call":
                        return ProcessToolCall(requestBody, id); // MODIFICATO: Passa l'ID
                    case "initialize":
                        return GetInitializeResponse(id); // MODIFICATO: Passa l'ID
                    default:
                        // MODIFICATO: Passa l'ID
                        return CreateErrorResponse(id, -32601, "Method not found", $"Method '{method}' is not supported");
                }
            }
            catch (Exception ex)
            {
                // MODIFICATO: Passa l'ID
                return CreateErrorResponse(id, -32603, "Internal error", ex.Message);
            }
        }

        // MODIFICATO: Aggiunto parametro 'id'
        private string GetInitializeResponse(string id)
        {
            return $@"{{
                ""jsonrpc"": ""2.0"",
                ""id"": {FormatJsonId(id)},
                ""result"": {{
                    ""protocolVersion"": ""{PROTOCOL_VERSION}"",
                    ""capabilities"": {{
                        ""tools"": {{}}
                    }},
                    ""serverInfo"": {{
                        ""name"": ""{SERVER_NAME}"",
                        ""version"": ""{SERVER_VERSION}""
                    }}
                }}
            }}";
        }

        // MODIFICATO: Aggiunto parametro 'id'
        private string GetToolsList(string id)
        {
            return $@"{{
                ""jsonrpc"": ""2.0"",
                ""id"": {FormatJsonId(id)},
                ""result"": {{
                    ""tools"": [
                        {{
                            ""name"": ""lampadina_stato"",
                            ""description"": ""Ottiene lo stato attuale della lampadina (accesa/spenta, colore, luminosità)"",
                            ""inputSchema"": {{
                                ""type"": ""object"",
                                ""properties"": {{}},
                                ""required"": []
                            }}
                        }},
                        {{
                            ""name"": ""lampadina_toggle"",
                            ""description"": ""Accende o spegne la lampadina"",
                            ""inputSchema"": {{
                                ""type"": ""object"",
                                ""properties"": {{}},
                                ""required"": []
                            }}
                        }},
                        {{
                            ""name"": ""lampadina_colore"",
                            ""description"": ""Cambia il colore della lampadina"",
                            ""inputSchema"": {{
                                ""type"": ""object"",
                                ""properties"": {{
                                    ""colore"": {{
                                        ""type"": ""string"",
                                        ""description"": ""Il colore in formato esadecimale (es: #ff0000 per rosso, #00ff00 per verde)"",
                                        ""pattern"": ""^#[0-9a-fA-F]{{6}}$""
                                    }}
                                }},
                                ""required"": [""colore""]
                            }}
                        }},
                        {{
                            ""name"": ""lampadina_luminosita"",
                            ""description"": ""Regola la luminosità della lampadina"",
                            ""inputSchema"": {{
                                ""type"": ""object"",
                                ""properties"": {{
                                    ""luminosita"": {{
                                        ""type"": ""number"",
                                        ""description"": ""Livello di luminosità da 0 a 100"",
                                        ""minimum"": 0,
                                        ""maximum"": 100
                                    }}
                                }},
                                ""required"": [""luminosita""]
                            }}
                        }},
                        {{
                            ""name"": ""lampadina_preset"",
                            ""description"": ""Applica un preset di colore alla lampadina"",
                            ""inputSchema"": {{
                                ""type"": ""object"",
                                ""properties"": {{
                                    ""preset"": {{
                                        ""type"": ""string"",
                                        ""description"": ""Nome del preset colore"",
                                        ""enum"": [""bianco"", ""rosso"", ""verde"", ""blu"", ""giallo"", ""magenta"", ""ciano"", ""arancione""]
                                    }}
                                }},
                                ""required"": [""preset""]
                            }}
                        }}
                    ]
                }}
            }}";
        }

        // MODIFICATO: Aggiunto parametro 'id'
        private string ProcessToolCall(string requestBody, string id)
        {
            try
            {
                string toolName = ExtractNestedJsonValue(requestBody, "params", "name");
                string argumentsJson = ExtractNestedJsonValue(requestBody, "params", "arguments");

                string result;
                switch (toolName)
                {
                    case "lampadina_stato":
                        result = GetLampadinaStato();
                        break;
                    case "lampadina_toggle":
                        result = ToggleLampadina();
                        break;
                    case "lampadina_colore":
                        result = CambiaColore(ExtractJsonValue(argumentsJson, "colore"));
                        break;
                    case "lampadina_luminosita":
                        result = CambiaLuminosita(ParseIntSafe(ExtractJsonValue(argumentsJson, "luminosita")));
                        break;
                    case "lampadina_preset":
                        result = ApplicaPreset(ExtractJsonValue(argumentsJson, "preset"));
                        break;
                    default:
                        result = $"❌ Errore: strumento '{toolName}' non riconosciuto";
                        break;
                }

                // MODIFICATO: Utilizza l'ID passato
                return $@"{{
                    ""jsonrpc"": ""2.0"",
                    ""id"": {FormatJsonId(id)},
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
                // MODIFICATO: Passa l'ID
                return CreateErrorResponse(id, -32603, "Internal error", ex.Message);
            }
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

        #region Helper Methods

        private string ReadRequestBody(HttpListenerRequest request)
        {
            try
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private void SafeCloseResponse(HttpListenerResponse response, int statusCode, string responseText = "")
        {
            try
            {
                response.StatusCode = statusCode;
                if (!string.IsNullOrEmpty(responseText))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                    response.ContentLength64 = buffer.Length;
                    response.ContentType = "application/json";
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                response.Close();
            }
            catch { }
        }

        // AGGIUNTO: Metodo per formattare correttamente l'ID JSON
        private string FormatJsonId(string id)
        {
            if (string.IsNullOrEmpty(id) || id == "null")
            {
                return "null";
            }
            // Controlla se l'id è un valore numerico (intero o con decimali)
            double num;
            if (double.TryParse(id, out num))
            {
                return id; // Se è un numero, non aggiungere virgolette
            }
            // Altrimenti, trattalo come una stringa
            return $"\"{EscapeJsonString(id)}\"";
        }

        private string TruncateString(string input, int maxLength)
        {
            return input?.Length > maxLength ? input.Substring(0, maxLength) : input ?? "";
        }

        private int ParseIntSafe(string value)
        {
            return int.TryParse(value, out int result) ? result : -1;
        }

        private string ExtractJsonValue(string json, string key)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(key))
                return "";

            // Pattern per valori stringa tra virgolette: "key":"value"
            string pattern = $@"""{key}""\s*:\s*""([^""]*)""";
            var match = Regex.Match(json, pattern);
            if (match.Success)
                return match.Groups[1].Value;

            // Pattern per valori numerici/booleani/null non tra virgolette: "key":value
            // Questo catturerà numeri (123, -10.5), booleani (true, false) e null
            pattern = $@"""{key}""\s*:\s*(-?\d+(\.\d+)?|true|false|null)";
            match = Regex.Match(json, pattern);
            if (match.Success)
                return match.Groups[1].Value;

            return "";
        }

        private string ExtractNestedJsonValue(string json, string parentKey, string childKey)
        {
            if (string.IsNullOrEmpty(json))
                return "";

            // Pattern più robusto per trovare l'oggetto genitore, anche se contiene altri oggetti
            string parentPattern = $@"""{parentKey}""\s*:\s*({{((?>[^{{}}]+|{{(?<c>)|}}(?<-c>))*(?(c)(?!)))}})";
            var parentMatch = Regex.Match(json, parentPattern, RegexOptions.Singleline);
            if (!parentMatch.Success)
                return "";

            string parentValue = parentMatch.Groups[1].Value;
            return ExtractJsonValue(parentValue, childKey);
        }

        private string EscapeJsonString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return str.Replace("\\", "\\\\")
                     .Replace("\"", "\\\"")
                     .Replace("\r", "\\r")
                     .Replace("\n", "\\n")
                     .Replace("\t", "\\t");
        }

        // MODIFICATO: Cambiata la firma per includere l'ID
        private string CreateErrorResponse(string id, int code, string message, string data = null)
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
                ""id"": {FormatJsonId(id)},
                ""error"": {errorJson}
            }}";
        }

        #endregion
    }
}
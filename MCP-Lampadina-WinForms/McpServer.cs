using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
                _listener.Prefixes.Add($"http://localhost:{port}/"); // Usa '*' per accettare connessioni non solo da localhost
                _listener.Start();
                _isRunning = true;

                _listenerThread = new Thread(ListenForRequests)
                {
                    IsBackground = true
                };
                _listenerThread.Start();

                _form.LogDebug($"🔌 MCP Server attivo su porta {port}");
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

        // Modello Sincrono: più stabile, elimina race condition
        private void ListenForRequests()
        {
            while (_isRunning)
            {
                try
                {
                    var context = _listener.GetContext();
                    ProcessRequest(context);
                }
                catch (HttpListenerException)
                {
                    // Ignora eccezione che avviene normalmente durante lo Stop()
                    if (_isRunning) _form.LogDebug("⚠️ Errore HttpListener, riprovo...");
                }
                catch (Exception ex)
                {
                    if (_isRunning) _form.LogDebug($"❌ Errore critico nel listener: {ex.Message}");
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string responseText = null;
            int statusCode = 200;
            string requestId = "null";

            try
            {
                // ========== MODIFICA CHIAVE ==========
                // Forza la connessione a rimanere aperta per la richiesta successiva.
                // Questo è il punto cruciale che risolve il crash del client.
                response.KeepAlive = true;
                // =====================================

                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                _form.LogDebug($"🌐 {request.HttpMethod} {request.Url.AbsolutePath} da {request.RemoteEndPoint}");

                if (request.HttpMethod == "OPTIONS")
                {
                    _form.LogDebug("✅ Risposta OPTIONS CORS (preflight)");
                    statusCode = 204;
                    return;
                }

                string requestBody = ReadRequestBody(request);
                if (!string.IsNullOrEmpty(requestBody))
                {
                    string extractedId = ExtractJsonValue(requestBody, "id");
                    if (!string.IsNullOrEmpty(extractedId)) requestId = extractedId;
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
                            //_form.LogDebug($"📨 Body ricevuto: {requestBody}");
                            responseText = ProcessMcpRequest(requestBody);

                            if (responseText == null)
                            {
                                statusCode = 204; // Notifica ricevuta
                            }
                            else
                            {
                                //_form.LogDebug($"📤 Risposta inviata: {responseText}");
                                response.ContentType = "application/json";
                            }
                        }
                    }
                }
                else
                {
                    statusCode = 404;
                    responseText = "Not Found";
                    response.ContentType = "text/plain";
                }
            }
            catch (Exception ex)
            {
                _form.LogDebug($"❌ Errore elaborazione richiesta: {ex.Message} \n{ex.StackTrace}");
                statusCode = 500;
                responseText = CreateErrorResponse(requestId, -32603, "Internal error", ex.Message);
                response.ContentType = "application/json";
            }
            finally
            {
                try
                {
                    response.StatusCode = statusCode;

                    if (statusCode != 204)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(responseText ?? "");
                        response.ContentLength64 = buffer.Length;
                        if (buffer.Length > 0)
                        {
                            response.OutputStream.Write(buffer, 0, buffer.Length);
                        }
                    }
                    response.Close();
                }
                catch (Exception ex)
                {
                    _form.LogDebug($"⚠️ Errore durante la finalizzazione della risposta: {ex.Message}");
                    try { context.Response.Abort(); } catch { }
                }
            }
        }

        private string ProcessMcpRequest(string requestBody)
        {
            bool isNotification = !Regex.IsMatch(requestBody, @"""id""\s*:");
            string method = ExtractJsonValue(requestBody, "method");

            if (isNotification)
            {
                _form.LogDebug($"🔔 Notifica ricevuta: '{method}'. Rispondo HTTP 204 No Content.");
                return null; // Segnala al chiamante di non inviare corpo JSON
            }

            string id = ExtractJsonValue(requestBody, "id");
            if (string.IsNullOrEmpty(id)) id = "null";

            _form.LogDebug($"🔍 Metodo estratto: '{method}', ID: {id}");

            switch (method)
            {
                case "tools/list":
                    return GetToolsList(id);
                case "tools/call":
                    return ProcessToolCall(requestBody, id);
                case "initialize":
                    return GetInitializeResponse(id);
                default:
                    return CreateErrorResponse(id, -32601, "Method not found", $"Method '{method}' is not supported");
            }
        }

        // ... TUTTI GLI ALTRI METODI (GetToolsList, ProcessToolCall, helpers, etc.) RIMANGONO INVARIATI ...
        // Li includo qui sotto per completezza, non contengono modifiche.

        private string GetInitializeResponse(string id)
        {
            return $@"{{""jsonrpc"":""2.0"",""id"":{FormatJsonId(id)},""result"":{{""protocolVersion"":""{PROTOCOL_VERSION}"",""capabilities"":{{""tools"":{{}}}},""serverInfo"":{{""name"":""{SERVER_NAME}"",""version"":""{SERVER_VERSION}""}}}}}}";
        }

        private string GetToolsList(string id)
        {
            return $@"{{""jsonrpc"":""2.0"",""id"":{FormatJsonId(id)},""result"":{{""tools"":[{{""name"":""lampadina_stato"",""description"":""Ottiene lo stato attuale della lampadina (accesa/spenta, colore, luminosità)"",""inputSchema"":{{""type"":""object"",""properties"":{{}},""required"":[]}}}},{{""name"":""lampadina_toggle"",""description"":""Accende o spegne la lampadina"",""inputSchema"":{{""type"":""object"",""properties"":{{}},""required"":[]}}}},{{""name"":""lampadina_colore"",""description"":""Cambia il colore della lampadina"",""inputSchema"":{{""type"":""object"",""properties"":{{""colore"":{{""type"":""string"",""description"":""Il colore in formato esadecimale (es: #ff0000 per rosso, #00ff00 per verde)"",""pattern"":""^#[0-9a-fA-F]{{6}}$""}}}},""required"":[""colore""]}}}},{{""name"":""lampadina_luminosita"",""description"":""Regola la luminosità della lampadina"",""inputSchema"":{{""type"":""object"",""properties"":{{""luminosita"":{{""type"":""number"",""description"":""Livello di luminosità da 0 a 100"",""minimum"":0,""maximum"":100}}}},""required"":[""luminosita""]}}}},{{""name"":""lampadina_preset"",""description"":""Applica un preset di colore alla lampadina"",""inputSchema"":{{""type"":""object"",""properties"":{{""preset"":{{""type"":""string"",""description"":""Nome del preset colore"",""enum"":[""bianco"",""rosso"",""verde"",""blu"",""giallo"",""magenta"",""ciano"",""arancione""]}}}},""required"":[""preset""]}}}}]}}}}";
        }

        private string ProcessToolCall(string requestBody, string id)
        {
            try
            {
                string toolName = ExtractNestedJsonValue(requestBody, "params", "name");
                string argumentsJson = ExtractNestedJsonValue(requestBody, "params", "arguments");

                string result;
                switch (toolName)
                {
                    case "lampadina_stato": result = GetLampadinaStato(); break;
                    case "lampadina_toggle": result = ToggleLampadina(); break;
                    case "lampadina_colore": result = CambiaColore(ExtractJsonValue(argumentsJson, "colore")); break;
                    case "lampadina_luminosita": result = CambiaLuminosita(ParseIntSafe(ExtractJsonValue(argumentsJson, "luminosita"))); break;
                    case "lampadina_preset": result = ApplicaPreset(ExtractJsonValue(argumentsJson, "preset")); break;
                    default: result = $"❌ Errore: strumento '{toolName}' non riconosciuto"; break;
                }

                return $@"{{""jsonrpc"":""2.0"",""id"":{FormatJsonId(id)},""result"":{{""content"":[{{""type"":""text"",""text"":""{EscapeJsonString(result)}""}}]}}}}";
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(id, -32603, "Internal error", ex.Message);
            }
        }

        private string GetLampadinaStato()
        {
            if (_form.InvokeRequired) { return (string)_form.Invoke(new Func<string>(GetLampadinaStato)); }
            string statoTesto = _form.IsAccesa ? "🟢 Accesa" : "🔴 Spenta";
            return $"🔌 Stato della lampadina:\\n• Stato: {statoTesto}\\n• Colore: {_form.Colore}\\n• Luminosità: {_form.Luminosita}%";
        }

        private string ToggleLampadina()
        {
            if (_form.InvokeRequired) { return (string)_form.Invoke(new Func<string>(ToggleLampadina)); }
            _form.ToggleLampadina();
            return _form.IsAccesa ? "✅ Lampadina accesa" : "✅ Lampadina spenta";
        }

        private string CambiaColore(string colore)
        {
            if (string.IsNullOrEmpty(colore) || !colore.StartsWith("#") || colore.Length != 7) { return "❌ Errore: colore non valido. Usa formato esadecimale (es: #ff0000)"; }
            if (_form.InvokeRequired) { return (string)_form.Invoke(new Func<string>(() => CambiaColore(colore))); }
            _form.CambiaColore(colore);
            return $"🎨 Colore cambiato a {colore}";
        }

        private string CambiaLuminosita(int luminosita)
        {
            if (luminosita < 0 || luminosita > 100) { return "❌ Errore: luminosità deve essere tra 0 e 100"; }
            if (_form.InvokeRequired) { return (string)_form.Invoke(new Func<string>(() => CambiaLuminosita(luminosita))); }
            _form.CambiaLuminosita(luminosita);
            return $"💡 Luminosità regolata al {luminosita}%";
        }

        private string ApplicaPreset(string preset)
        {
            var presets = new Dictionary<string, string> { { "bianco", "#ffffff" }, { "rosso", "#ff0000" }, { "verde", "#00ff00" }, { "blu", "#0000ff" }, { "giallo", "#ffff00" }, { "magenta", "#ff00ff" }, { "ciano", "#00ffff" }, { "arancione", "#ff6600" } };
            if (!presets.ContainsKey(preset.ToLower())) { return $"❌ Errore: preset '{preset}' non riconosciuto. Preset disponibili: {string.Join(", ", presets.Keys)}"; }
            string colore = presets[preset.ToLower()];
            if (_form.InvokeRequired) { return (string)_form.Invoke(new Func<string>(() => ApplicaPreset(preset))); }
            _form.CambiaColore(colore);
            return $"🎯 Preset \"{preset}\" applicato ({colore})";
        }

        #region Helper Methods
        private string ReadRequestBody(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding)) { return reader.ReadToEnd(); }
        }
        private string FormatJsonId(string id)
        {
            if (string.IsNullOrEmpty(id) || id == "null") { return "null"; }
            double num; if (double.TryParse(id, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out num)) { return id; }
            return $"\"{EscapeJsonString(id)}\"";
        }
        private int ParseIntSafe(string value) { int.TryParse(value, out int result); return result; }
        private string ExtractJsonValue(string json, string key)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(key)) return "";
            string pattern = $@"""{key}""\s*:\s*""([^""]*)"""; var match = Regex.Match(json, pattern); if (match.Success) return match.Groups[1].Value;
            pattern = $@"""{key}""\s*:\s*(-?\d+(\.\d+)?|true|false|null)"; match = Regex.Match(json, pattern); if (match.Success) return match.Groups[1].Value;
            return "";
        }
        private string ExtractNestedJsonValue(string json, string parentKey, string childKey)
        {
            if (string.IsNullOrEmpty(json)) return "";
            string parentPattern = $@"""{parentKey}""\s*:\s*({{((?>[^{{}}]+|{{(?<c>)|}}(?<-c>))*(?(c)(?!)))}})"; var parentMatch = Regex.Match(json, parentPattern, RegexOptions.Singleline); if (!parentMatch.Success) return "";
            string parentValue = parentMatch.Groups[1].Value; return ExtractJsonValue(parentValue, childKey);
        }
        private string EscapeJsonString(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t");
        }
        private string CreateErrorResponse(string id, int code, string message, string data = null)
        {
            string errorJson = $@"{{""code"":{code},""message"":""{EscapeJsonString(message)}""";
            if (!string.IsNullOrEmpty(data)) { errorJson += $@", ""data"":""{EscapeJsonString(data)}"""; }
            errorJson += "}";
            return $@"{{""jsonrpc"":""2.0"",""id"":{FormatJsonId(id)},""error"":{errorJson}}}";
        }
        #endregion
    }
}
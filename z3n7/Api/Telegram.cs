using System;
using System.Linq;
using System.Runtime.CompilerServices;

using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7
{
    /// <summary>
    /// Sends messages to a Telegram chat topic through the Bot API (<c>sendMessage</c> over
    /// <c>NetHttp</c>).
    /// Missing token, chat and topic are read from the <c>_api</c> table, row <c>id = 'tg_logger'</c>:
    /// <c>apikey</c> and <c>extra</c> = <c>{chat}/{topic}</c>.
    /// </summary>
    public class Telegram
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Logger _log;
        private string _token;
        private string _group;
        private string _topic;
        private readonly NetHttp _http;

        /// <summary>Creates a client.</summary>
        /// <param name="log">
        /// Logger for progress. <c>SendMarkdown</c>, <c>SendCommitsSummary</c> and splitting in
        /// <c>SendLongMessage</c> call it without a null check.
        /// </param>
        /// <param name="token">Bot token.</param>
        /// <param name="group">Chat id.</param>
        /// <param name="topic">Message id of the topic to reply to.</param>
        public Telegram(IZennoPosterProjectModel project, Logger log = null, string token = null, string group = null, string topic = null)
        {
            _project = project;

            _log = log;
            _token = token;
            _group = group;
            _topic = topic;
            _http = new NetHttp(project, log);
            LoadCreds();
        }

        private void LoadCreds()
        {
            if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_group) || string.IsNullOrEmpty(_topic))
            {
                var creds = _project.DbGetColumns("apikey, extra", "_api", where: "id = 'tg_logger'", log:true);
                if (string.IsNullOrEmpty(_token)) _token = creds["apikey"];
                if (string.IsNullOrEmpty(_group)) _group = creds["extra"].Split('/')[0];
                if (string.IsNullOrEmpty(_topic)) _topic = creds["extra"].Split('/')[1];
            }
        }

        private string GetMessageLink(string response)
        {
            try
            {
                // Парсим JSON ответ для получения message_id
                var match = System.Text.RegularExpressions.Regex.Match(response, @"""message_id"":(\d+)");
                if (match.Success)
                {
                    string messageId = match.Groups[1].Value;
                    // Формируем ссылку на сообщение
                    return $"https://t.me/c/{_group.Replace("-100", "")}/{messageId}";
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Sends the <c>failReport</c> variable as a MarkdownV2 message when it is set; otherwise a success
        /// line with the project name and <c>acc0</c> as hashtags.
        /// </summary>
        public void Report()
        {
            string time = _project.ExecuteMacro(DateTime.Now.ToString("MM-dd HH:mm"));
            string report = "";

            if (!string.IsNullOrEmpty(_project.Variables["failReport"].Value))
            {
                string encodedFailReport = Uri.EscapeDataString(_project.Variables["failReport"].Value);
                string failUrl = $"https://api.telegram.org/bot{_token}/sendMessage?chat_id={_group}&text={encodedFailReport}&reply_to_message_id={_topic}&parse_mode=MarkdownV2";
                _http.GET(failUrl);
            }
            else
            {
                report = $"✅️ [{time}]{_project.Name}";
                string successReport = $"✅️  \\#{_project.Name.EscapeMarkdown()} \\#{_project.Variables["acc0"].Value} \n";
                string encodedReport = Uri.EscapeDataString(successReport);
                string url = $"https://api.telegram.org/bot{_token}/sendMessage?chat_id={_group}&text={encodedReport}&reply_to_message_id={_topic}&parse_mode=MarkdownV2";
                _http.GET(url);
            }
            string toLog = $"✔️ All jobs done. Elapsed: {_project.TimeElapsed()}s \n███ ██ ██  ██ █  █  █  ▓▓▓ ▓▓ ▓▓  ▓  ▓  ▓  ▒▒▒ ▒▒ ▒▒ ▒  ▒  ░░░ ░░  ░░ ░ ░ ░ ░ ░ ░  ░  ░  ░   ░   ░   ░    ░    ░    ░     ░        ░          ░";
        }

        /// <summary>Sends a message with Markdown parsing.</summary>
        /// <param name="message">Text.</param>
        /// <param name="useMarkdownV2">Use MarkdownV2 instead of Markdown.</param>
        /// <param name="disableWebPagePreview">Disable link previews.</param>
        /// <param name="replyToTopic">Post into the topic.</param>
        /// <param name="log">Not used.</param>
        /// <returns>
        /// The link <c>https://t.me/c/{chat}/{messageId}</c> on success; otherwise the Telegram answer or <c>❌
        /// Exception: …</c>.
        /// </returns>
        public string SendMarkdown(string message, bool useMarkdownV2 = false, bool disableWebPagePreview = true, bool replyToTopic = true, bool log = false)
        {
            _log.Send($"Sending markdown message (length: {message.Length})");

            try
            {
                string processedMessage = message;
                string encodedMessage = Uri.EscapeDataString(processedMessage);
                string parseMode = useMarkdownV2 ? "MarkdownV2" : "Markdown";
                string url = $"https://api.telegram.org/bot{_token}/sendMessage" +
                            $"?chat_id={_group}" +
                            $"&text={encodedMessage}" +
                            $"&parse_mode={parseMode}" +
                            $"&disable_web_page_preview={disableWebPagePreview.ToString().ToLower()}";

                if (replyToTopic && !string.IsNullOrEmpty(_topic))
                {
                    url += $"&reply_to_message_id={_topic}";
                }

                string response = _http.GET(url);
                
                if (response.Contains("\"ok\":true"))
                {
                    string messageLink = GetMessageLink(response);
                    _log.Send($"✅ Message sent: {messageLink}");
                    return messageLink;
                }
                else
                {
                    _log.Send($"⚠️ Telegram API error: {response}");
                    return response;
                }
            }
            catch (Exception ex)
            {
                string error = $"❌ Exception: {ex.Message}";
                _log.Send(error);
                return error;
            }
        }

        /// <summary>
        /// Sends a text summary as plain text: drops a leading <c>---</c> block and replaces <c>## </c> and
        /// <c># </c> headings with emoji markers.
        /// </summary>
        /// <param name="summary">Text.</param>
        /// <param name="log">Not used.</param>
        public string SendCommitsSummary(string summary, bool log = false)
        {
            _log.Send("Sending commits summary");
            string formatted = PrepareCommitsSummary(summary);
            return SendLongMessage(formatted, useMarkdown: false, log: log);
        }

        private string PrepareCommitsSummary(string summary)
        {
            if (summary.Contains("---"))
            {
                int startIndex = summary.IndexOf("---");
                int endIndex = summary.IndexOf("---", startIndex + 3);
                if (endIndex > startIndex)
                {
                    summary = summary.Substring(endIndex + 3).Trim();
                }
            }

            summary = summary
                .Replace("## ", "📌 ")
                .Replace("# ", "🔹 ");

            return summary;
        }

        /// <summary>
        /// Sends a message, split into parts of up to 4000 characters at paragraph or line boundaries; stops at
        /// the first failed part.
        /// </summary>
        /// <param name="message">Text.</param>
        /// <param name="useMarkdown">Send with Markdown parsing.</param>
        /// <param name="log">Not used.</param>
        /// <returns>Comma-separated links of the parts, or the error of the failed part.</returns>
        public string SendLongMessage(string message, bool useMarkdown = false, bool log = false)
        {
            const int maxLength = 4000;

            if (message.Length <= maxLength)
            {
                return useMarkdown 
                    ? SendMarkdown(message, useMarkdownV2: false, log: log)
                    : SendPlainText(message, log: log);
            }

            _log.Send($"Message too long ({message.Length} chars), splitting...");

            string[] parts = SplitMessage(message, maxLength);
            var results = new System.Collections.Generic.List<string>();
            
            for (int i = 0; i < parts.Length; i++)
            {
                _log.Send($"Sending part {i + 1}/{parts.Length}");
                
                string result = useMarkdown 
                    ? SendMarkdown(parts[i], useMarkdownV2: false, log: log)
                    : SendPlainText(parts[i], log: log);
                
                results.Add(result);

                // Если это ошибка (не ссылка), прерываем отправку
                if (!result.StartsWith("https://"))
                {
                    _log.Send($"Failed to send part {i + 1}, stopping");
                    break;
                }

                if (i < parts.Length - 1)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }

            // Возвращаем все ссылки через запятую или последнюю ошибку
            if (results.All(r => r.StartsWith("https://")))
            {
                return string.Join(", ", results);
            }
            else
            {
                return results.Last(r => !r.StartsWith("https://"));
            }
        }

        private string SendPlainText(string message, bool log = false)
        {
            try
            {
                string encodedMessage = Uri.EscapeDataString(message);
                
                string url = $"https://api.telegram.org/bot{_token}/sendMessage" +
                            $"?chat_id={_group}" +
                            $"&text={encodedMessage}";

                if (!string.IsNullOrEmpty(_topic))
                {
                    url += $"&reply_to_message_id={_topic}";
                }

                string response = _http.GET(url);
                
                if (response.Contains("\"ok\":true"))
                {
                    string messageLink = GetMessageLink(response);
                    return messageLink;
                }
                else
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                return $"❌ Exception: {ex.Message}";
            }
        }

        private string[] SplitMessage(string message, int maxLength)
        {
            var parts = new System.Collections.Generic.List<string>();
            string[] paragraphs = message.Split(new[] { "\n\n" }, StringSplitOptions.None);
            string currentPart = "";
            
            foreach (string paragraph in paragraphs)
            {
                if (currentPart.Length + paragraph.Length + 2 > maxLength)
                {
                    if (!string.IsNullOrEmpty(currentPart))
                    {
                        parts.Add(currentPart.Trim());
                        currentPart = "";
                    }
                    
                    if (paragraph.Length > maxLength)
                    {
                        string[] lines = paragraph.Split('\n');
                        foreach (string line in lines)
                        {
                            if (currentPart.Length + line.Length + 1 > maxLength)
                            {
                                parts.Add(currentPart.Trim());
                                currentPart = line + "\n";
                            }
                            else
                            {
                                currentPart += line + "\n";
                            }
                        }
                    }
                    else
                    {
                        currentPart = paragraph + "\n\n";
                    }
                }
                else
                {
                    currentPart += paragraph + "\n\n";
                }
            }
            
            if (!string.IsNullOrEmpty(currentPart))
            {
                parts.Add(currentPart.Trim());
            }
            
            return parts.ToArray();
        }
    }

}

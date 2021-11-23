using HealthChecks.UI.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<TContent> As<TContent>(this HttpResponseMessage response) where TContent : UIHealthReport
        {
            if (response != null)
            {
                var body = await response.Content
                    .ReadAsStringAsync();

                if (body != null)
                {
                    var content = JsonConvert.DeserializeObject<TContent>(body);

                    if (content != null)
                    {
                        if (content is UIHealthReport report)
                        {
                            if(report.Entries == null || report.Entries.Count == 0)
                            {
                                Dictionary<string, UIHealthReportEntry> entries = new();
                                entries.Add("endpoint", new UIHealthReportEntry() { 
                                    Status = UIHealthStatus.Unhealthy,
                                    Description = body
                                });
                                return (TContent)new UIHealthReport(entries, new TimeSpan());
                            }
                        }
                        return content;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Response message can't be deserialized as {typeof(TContent).FullName}. Response is '{body}'");
                    }
                }
                else {
                    throw new InvalidOperationException($"Body response is null. Status code {response.StatusCode}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Response is null. Status code {response.StatusCode}");
            }
        }
    }
}

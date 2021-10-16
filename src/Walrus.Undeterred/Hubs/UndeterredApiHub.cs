using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Walrus.Undeterred.Hubs
{
    public class UndeterredApiHub : Hub
    {
    }

    public struct UndeterredApiHubAction
    {
        public static readonly string ROUGE_API_ISSUE = "rouge_api_issue";
    }
}

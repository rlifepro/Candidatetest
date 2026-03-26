using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CandidateTest.Api.Hubs
{
    [Authorize]
    public class LiveHub : Hub
    {
        public async Task SendCandidateUpdate(int testId, int submissionId, int score, string user)
        {
            await Clients.Group($"test-{testId}").SendAsync("CandidateScoreUpdated", new { submissionId, score, user });
        }

        public Task JoinTestGroup(int testId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"test-{testId}");
        }

        public Task LeaveTestGroup(int testId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"test-{testId}");
        }
    }
}

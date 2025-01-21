using System;
using System.Threading.Tasks;

namespace LendingView.Servcies
{
    public interface IUserService
    {
        Task<bool> IsAdmin();
        Task GetAccessToken(bool triggerLogin);

        public event Action AuthenticationStateChanged;

        Task TriggerLoginNotification();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Infrastructure.Session;

namespace WorkoutApp.Tests
{
    public class SessionManagerTests
    {
        [Fact]
        public void IsUserLoggedIn_ShouldBeFalse_OnInitialization()
        {
            SessionManager session = new();

            Assert.False(session.IsUserLoggedIn);
        }

        [Fact]
        public void IsUserLoggedIn_ShouldBeTrue_WhenCurrentUserIdIsSet()
        {
            SessionManager session = new() { CurrentUserId = 123 };

            Assert.True(session.IsUserLoggedIn);
        }

        [Fact]
        public void ClearSession_ShouldResetCurrentUserIdAndLoginState()
        {
            SessionManager session = new() { CurrentUserId = 123 };

            session.ClearSession();

            Assert.Null(session.CurrentUserId);
            Assert.False(session.IsUserLoggedIn);
        }
    }
}

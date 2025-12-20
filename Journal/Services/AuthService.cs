using System;

namespace Journal.Services
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; } = false;

        public event Action? OnAuthStateChanged;

        public bool Login(string username, string password)
        {
            if (username == "admin" && password == "test")
            {
                IsAuthenticated = true;
                NotifyAuthStateChanged();
                return true;
            }
            return false;
        }

        public void Logout()
        {
            IsAuthenticated = false;
            NotifyAuthStateChanged();
        }

        private void NotifyAuthStateChanged() => OnAuthStateChanged?.Invoke();
    }
}

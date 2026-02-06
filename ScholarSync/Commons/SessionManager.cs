using System;
using ScholarSync.Models;

namespace ScholarSync.Commons
{
    
    public static class SessionManager
    {
        
        private static UserModel currentUser;

        
        public static event EventHandler<UserModel> OnUserLoggedIn;
        public static event EventHandler OnUserLoggedOut;

        public static UserModel CurrentUser
        {
            get { return currentUser; }
        }

        public static bool IsLoggedIn
        {
            get { return currentUser != null; }
        }

       
        public static void Login(UserModel user)
        {
            currentUser = user;
            OnUserLoggedIn?.Invoke(null, user);
        }

       
        public static void Logout()
        {
            currentUser = null;
            OnUserLoggedOut?.Invoke(null, EventArgs.Empty);
        }

       
        public static string GetUserId()
        {
            return currentUser?.Id ?? string.Empty;
        }

        
        public static string GetUserName()
        {
            return currentUser?.UserName ?? "Guest";
        }

       
        public static string GetUserRole()
        {
            return currentUser?.Role ?? "Unknown";
        }

        
        public static string GetUserCNIC()
        {
            return currentUser?.CNIC ?? string.Empty;
        }

       
        public static string GetUserEmail()
        {
            return currentUser?.Email ?? string.Empty;
        }

       
        public static bool HasRole(string role)
        {
            return currentUser != null && currentUser.Role.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

       
        public static bool IsAdmin()
        {
            return HasRole("Admin") || HasRole("Administrator");
        }

        
        public static bool IsTeacher()
        {
            return HasRole("Teacher");
        }
    }
}

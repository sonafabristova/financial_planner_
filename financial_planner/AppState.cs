using System;
using financial_planner.Models;

namespace financial_planner
{
    public static class AppState
    {
        public static User CurrentUser { get; set; }

        public static event Action DataChanged;

        public static void NotifyDataChanged()
        {
            DataChanged?.Invoke();
        }
    }
}
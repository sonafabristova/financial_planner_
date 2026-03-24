using System.Collections.Generic;

namespace financial_planner.Models
{
    public class GoalStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static GoalStatus Active => new GoalStatus { Id = 1, Name = "Активна" };
        public static GoalStatus Completed => new GoalStatus { Id = 2, Name = "Выполнена" };
        public static GoalStatus Archived => new GoalStatus { Id = 3, Name = "Архивирована" };

        public static List<GoalStatus> GetAll()
        {
            return new List<GoalStatus> { Active, Completed, Archived };
        }

        public bool CanAllocate => Id == Active.Id;
        public override string ToString() => Name;
    }
}
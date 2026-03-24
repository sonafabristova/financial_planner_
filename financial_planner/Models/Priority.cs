using System.Collections.Generic;

namespace financial_planner.Models
{
    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Priority Primary => new Priority { Id = 1, Name = "Первичный" };
        public static Priority Secondary => new Priority { Id = 2, Name = "Вторичный" };
        public static Priority Residual => new Priority { Id = 3, Name = "Остаточный" };

        public static List<Priority> GetAll()
        {
            return new List<Priority> { Primary, Secondary, Residual };
        }

        public override string ToString() => Name;
    }
}
// --- Header: TodoContext.cs ---
using Microsoft.EntityFrameworkCore;

namespace SeltzrGetStarted {
	public class TodoContext : DbContext {
		public DbSet<Todo> Todos { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite("Data Source=todo.db");
	}
}
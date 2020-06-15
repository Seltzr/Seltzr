namespace TestProject.TestComponents {
	using Microsoft.EntityFrameworkCore;

	public class TestDbContext : DbContext {
		public TestDbContext(DbContextOptions options)
			: base(options) { }

		public DbSet<TestModel> Models { get; set; }
	}
}

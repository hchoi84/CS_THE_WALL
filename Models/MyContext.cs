using Microsoft.EntityFrameworkCore;

namespace _01_THE_WALL.Models
{
  public class MyContext : DbContext
  {
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<UserModel> Users {get;set;}
    public DbSet<MessageModel> Messages {get;set;}
    public DbSet<CommentModel> Comments {get;set;}
  }
}
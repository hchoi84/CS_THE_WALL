using System;
using System.ComponentModel.DataAnnotations;

namespace _01_THE_WALL.Models
{
  public class CommentModel
  {
    [Key]
    public int CommentId {get;set;}

    public int MessageId {get;set;}
    public int UserId {get;set;}

    [Required]
    [Display(Name="Comment")]
    [MinLength(2,ErrorMessage="Minimum 2 characters")]
    public string Comment {get;set;}
    
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public MessageModel Message {get;set;}
    public UserModel User {get;set;}
  }
}
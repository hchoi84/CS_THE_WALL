using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _01_THE_WALL.Models
{
  public class MessageModel
  {
    [Key]
    public int MessageId {get;set;}

    public int UserId {get;set;}

    [Required(ErrorMessage="Required")]
    [Display(Name="Message")]
    [MinLength(2, ErrorMessage="Minimum 2 characters")]
    public string Message {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    
    public List<CommentModel> Comments {get;set;}
    public UserModel User {get;set;}

    [NotMapped]
    public bool isDeleteable {get;set;}
  }
}
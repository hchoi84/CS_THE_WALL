using System.Collections.Generic;

namespace _01_THE_WALL.Models
{
  public class MsgMsgsModel
  {
    public MessageModel Msg {get;set;}
    public List<MessageModel> Msgs {get;set;}
    public CommentModel Cmt {get;set;}
    public List<CommentModel> Cmts {get;set;}
  }
}
using System.ComponentModel.DataAnnotations;
namespace belt.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId {get;set;}
        public int UserId {get;set;}
        public int HangoutId {get;set;}
        public User User {get;set;}
        public Hangout Hangout {get;set;}
    }
}
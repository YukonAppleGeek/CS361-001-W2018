using System.ComponentModel;

namespace StudyUp.Models
{
    public class CanvasUser
    {
        [DisplayName("Access Token")]
        public string Token { get; set; }

        public CanvasUser() {}
    }
}
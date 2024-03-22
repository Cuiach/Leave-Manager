using System.ComponentModel.DataAnnotations;

namespace Inewi_Console.Entities
{
    public class HistoryOfLeaves
    {
        [Key]
        public required int UserId { get; set; }
        public List<Leave>? TakenLeaves;
    }
}

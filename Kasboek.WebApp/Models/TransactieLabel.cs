namespace Kasboek.WebApp.Models
{
    public class TransactieLabel
    {

        public int TransactieId { get; set; }
        public Transactie Transactie { get; set; }

        public int LabelId { get; set; }
        public Label Label { get; set; }

    }
}

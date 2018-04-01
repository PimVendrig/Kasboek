namespace Kasboek.WebApp.Models
{
    public class RekeningLabel
    {

        public int RekeningId { get; set; }
        public Rekening Rekening { get; set; }

        public int LabelId { get; set; }
        public Label Label { get; set; }

    }
}

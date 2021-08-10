namespace HR_Prosecutors.Models
{
    public class PersonSL : IPerson
    {
        public string Isn { get; set; }
        public string FIO { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Organization { get; set; }
    }
}

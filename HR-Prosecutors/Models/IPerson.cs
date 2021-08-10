namespace HR_Prosecutors.Models
{
    interface IPerson
    {
        public string FIO { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
    }
}

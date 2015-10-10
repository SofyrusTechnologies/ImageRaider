namespace ImageRaider
{
    internal class AuthenticateResult
    {
        public int Status { get; set; }

        public string Project_Id { get; set; }

        public string Error { get; set; }

        public bool IsSuccess { get { return this.Status == 201 ? true : false; } }

    }


    internal class IntermediateResult
    {
        public int status { get; set; }
        public string statustext { get; set; }
        public Progress progress { get; set; }
    }

    internal class Progress
    {
        public int total { get; set; }
        public int done { get; set; }
    }

}

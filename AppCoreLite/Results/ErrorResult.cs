using AppCoreLite.Results.Bases;

namespace AppCoreLite.Results
{
    public class ErrorResult : Result
    {
        public ErrorResult(string message) : base(false, message)
        {

        }

        public ErrorResult() : base(false, "")
        {

        }
    }
}

using AppCoreLite.Results.Bases;

namespace AppCoreLite.Results
{
    public class SuccessResult : Result
    {
        public SuccessResult(string message) : base(true, message)
        {

        }

        public SuccessResult() : base(true, "")
        {

        }
    }
}

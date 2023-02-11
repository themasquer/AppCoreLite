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

    public class SuccessResult<TResultType> : Result<TResultType>
    {
        public SuccessResult(string message, TResultType data) : base(true, message, data)
        {

        }

        public SuccessResult(string message) : base(true, message, default)
        {

        }

        public SuccessResult(TResultType data) : base(true, "", data)
        {

        }

        public SuccessResult() : base(true, "", default)
        {

        }
	}
}

namespace IS.Core.Communication
{
    public sealed class BaseResult<T>
    {
        public T? Response { get; set; }
        public ICollection<string> Errors { get; init; } = [];
        public bool IsValid { get => Errors.Count == 0; }

        public void AddError(string error) => Errors.Add(error);

        public static BaseResult<T> Success(T response)
        {
            return new BaseResult<T>
            {
                Response = response
            };
        }

        public static BaseResult<T> Failure(params string[] errors)
        {
            return new BaseResult<T>
            {
                Errors = errors
            };
        }
    }
}

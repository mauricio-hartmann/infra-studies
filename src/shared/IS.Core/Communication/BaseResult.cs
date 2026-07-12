namespace IS.Core.Communication
{
    public sealed class BaseResult<T>
    {
        private const string _generalErrorKey = "General";

        public T Response { get; set; }
        public IDictionary<string, ICollection<string>> Errors { get; init; } = new Dictionary<string, ICollection<string>>();
        public bool IsValid { get => Errors.Count == 0; }

        public void AddError(string key, string error)
        {
            if (!Errors.TryGetValue(key, out ICollection<string> errorsList))
            {
                errorsList = new List<string>();
                Errors.Add(key, errorsList);
            }

            errorsList.Add(error);
        }

        public void AddGeneralError(string error)
        {
            AddError(_generalErrorKey, error);
        }

        public static BaseResult<T> Success(T response)
        {
            return new BaseResult<T>
            {
                Response = response
            };
        }

        public static BaseResult<T> Failure(IDictionary<string, string[]> errors)
        {
            return new BaseResult<T>
            {
                Errors = errors.ToDictionary(x => x.Key, x => (ICollection<string>)x.Value)
            };
        }

        public static BaseResult<T> Failure(string error)
        {
            var result = new BaseResult<T>();
            result.AddGeneralError(error);

            return result;
        }
    }
}

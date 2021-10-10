namespace API.Errors
{
    public class ApiValidationErroResponse : ApiResponse
    {
        public ApiValidationErroResponse() : base(400)
        {

        }
        public IEnumerable<string> Errors { get; set; }
    }
}
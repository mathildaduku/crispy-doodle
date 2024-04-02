namespace SubscriptionService.Helpers
{
    public class ApiResponse<T>
    {
        public ResponseStatus Status { get; set; } = ResponseStatus.Success;
        public string Message { get; set; } = string.Empty;
        public T? Result { get; set; }
    }
}
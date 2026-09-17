namespace FlashCart.Application.Payments;

public static class RetryHelper
{

    private static bool IsRetryable(Exception ex)
{
    return ex is TimeoutException
        || ex is HttpRequestException;
}
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        int maxAttempts = 3)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                  if (!IsRetryable(ex))
    {
        throw;
    }
                if (attempt == maxAttempts)
                {
                    throw;
                }

                var delaySeconds =
                    Math.Pow(2, attempt - 1);

                await Task.Delay(
                    TimeSpan.FromSeconds(delaySeconds));
            }
        }

        throw new InvalidOperationException(
            "Retry operation failed.");
    }
}
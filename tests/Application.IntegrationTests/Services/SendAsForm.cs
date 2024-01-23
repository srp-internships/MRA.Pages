namespace Application.IntegrationTests.Services;

public static class SendAsForm
{
    public static async Task<HttpResponseMessage> PostAsFormAsync<T>(this HttpClient httpClient, string requestUri, T data)
    {
        var formData = new MultipartFormDataContent();

        foreach (var property in typeof(T).GetProperties())
        {
            var value = property.GetValue(data)?.ToString();
            if (value != null)
            {
                formData.Add(new StringContent(value), property.Name);
            }
        }

        return await httpClient.PostAsync(requestUri, formData);
    }
}
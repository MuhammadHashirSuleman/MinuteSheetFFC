using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MinuteSheetFFC.Application.DTOs.Common;
using MinuteSheetFFC.Web.State;

namespace MinuteSheetFFC.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly AppState _state;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ApiClient(HttpClient http, AppState state)
    {
        _http = http;
        _state = state;
    }

    private void SetAuth()
    {
        if (!string.IsNullOrEmpty(_state.AccessToken))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _state.AccessToken);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        SetAuth();
        try
        {
            var response = await _http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions) ?? ApiResponse<T>.Fail("Deserialization failed.");
        }
        catch (Exception ex)
        {
            return ApiResponse<T>.Fail(ex.Message);
        }
    }

    public async Task<PagedResponse<T>?> GetPagedAsync<T>(string url)
    {
        SetAuth();
        try
        {
            var response = await _http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PagedResponse<T>>(json, JsonOptions);
        }
        catch { return null; }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string url, object? body = null)
    {
        SetAuth();
        try
        {
            var response = await _http.PostAsJsonAsync(url, body, JsonOptions);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions) ?? ApiResponse<T>.Fail("Failed.");
        }
        catch (Exception ex) { return ApiResponse<T>.Fail(ex.Message); }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string url, object body)
    {
        SetAuth();
        try
        {
            var response = await _http.PutAsJsonAsync(url, body, JsonOptions);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions) ?? ApiResponse<T>.Fail("Failed.");
        }
        catch (Exception ex) { return ApiResponse<T>.Fail(ex.Message); }
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string url)
    {
        SetAuth();
        try
        {
            var response = await _http.DeleteAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions) ?? ApiResponse<T>.Fail("Failed.");
        }
        catch (Exception ex) { return ApiResponse<T>.Fail(ex.Message); }
    }
}

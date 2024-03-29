﻿using client.Model;
using Serilog;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static client.Model.ResponseEnum;

namespace client.Utils;

public class NetUtils
{
    private class TokenModel
    {
        public string token { get; set; }
    }

    private readonly HttpClientHandler _handler;
    private HttpClient _client;

    public NetUtils()
    {
        _handler = new HttpClientHandler();
    }

    public async Task<NetUtilsResponseCodes> AuthAsync(string url, string username, string password)
    {
        if (!await CheckServerConnection())
        {
            return NetUtilsResponseCodes.CANTCONNECTTOTHESERVER;
        }

        var newUser = new User
        {
            Login = username,
            Password = password
        };

        try
        {
            var response = await _client.PostAsJsonAsync(url, newUser);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseContent))
                {
                    var tokenResponse = JsonSerializer.Deserialize<TokenModel>(responseContent);

                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.token))
                    {
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.token);

                        Log.Information($"Successfull authorization on {response.RequestMessage.RequestUri.AbsoluteUri}");
                        return NetUtilsResponseCodes.OK;
                    }
                }
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                Log.Warning($"Registration failed. User is already registered on {response.RequestMessage.RequestUri.AbsoluteUri}");
                return NetUtilsResponseCodes.USERISALREDYEXISTS;
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Warning($"Registration failed. Invalid login or password on {response.RequestMessage.RequestUri.AbsoluteUri}");
                return NetUtilsResponseCodes.BADREQUEST;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Log.Warning($"Login failed. Incorrect login or password on {response.RequestMessage.RequestUri.AbsoluteUri}");
                return NetUtilsResponseCodes.UNATHROIZED;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }

        Log.Warning($"Recieved error code from {_client.BaseAddress}");
        return NetUtilsResponseCodes.ERROR;
    }

    public Task<NetUtilsResponseCodes> LoginAsync(string username, string password)
    {
        return AuthAsync(ApiLinks.LoginLink, username, password);
    }

    public Task<NetUtilsResponseCodes> RegisterAsync(string username, string password)
    {
        return AuthAsync(ApiLinks.RegisterLink, username, password);
    }

    public async Task<bool> CheckServerConnection()
    {
        try
        {
            var response = await _client.GetAsync(ApiLinks.HealthLink);
            response.EnsureSuccessStatusCode();

            Log.Information($"Successfully connected to {response.RequestMessage.RequestUri.AbsoluteUri}");

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        return false;
    }

    public async Task<ObservableCollection<LogRecord>> GetDataAsync()
    {
        try
        {
            var response = await _client.GetAsync(ApiLinks.DataLink);

            response.EnsureSuccessStatusCode();

            var logsData = await response.Content.ReadFromJsonAsync<ObservableCollection<LogRecord>>();

            if (logsData is null)
            {
                throw new ArgumentNullException(nameof(logsData));
            }

            Log.Information($"Successfully received data ({response.Content.Headers.ContentLength} bytes) from {response.RequestMessage.RequestUri.AbsoluteUri}");
            return logsData;
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
        }
        return null;
    }

    public bool SetIpAndPort(string ip, string port)
    {
        try
        {
            // Задавать параметры можно только до отправки первого запроса
            _client = new HttpClient(_handler) { BaseAddress = new Uri($"https://{ip}:{port}") };
            Log.Information($"Successfully changed base address to {_client.BaseAddress}");

            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"{ex.Message} ip = {ip}, port = {port}");
        }
        return false;
    }
}
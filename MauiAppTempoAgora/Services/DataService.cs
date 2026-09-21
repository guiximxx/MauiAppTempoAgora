using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            // Parte 2: Alerta/verificação de conexão com a internet
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Sem Conexão", "Você está sem acesso à internet. Verifique sua rede e tente novamente.", "OK");
                return null;
            }

            Tempo? t = null;
            string chave = "c4688825bdf0bf88b416c51b91e4571b";

            // Adicionado &lang=pt_br para trazer a descrição em português
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&lang=pt_br&appid={chave}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    // Parte 2: Tratamento do código HTTP 404 (Cidade não encontrada)
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        await Shell.Current.DisplayAlert("Cidade Não Encontrada", $"A cidade '{cidade}' não foi localizada. Verifique o nome digitado.", "OK");
                        return null;
                    }

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();
                        var rascunho = JObject.Parse(json);

                        DateTime time = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        // Parte 1: Garantindo o mapeamento de description, speed e visibility
                        t = new()
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"],
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString("HH:mm"),
                            sunset = sunset.ToString("HH:mm"),
                        };
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Erro", $"Ocorreu um erro ao consultar o tempo ({resp.StatusCode}).", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Erro de Comunicação", $"Falha ao conectar: {ex.Message}", "OK");
                }
            }

            return t;
        }
    }
}
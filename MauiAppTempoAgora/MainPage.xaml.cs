using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        // Parte 1: Adicionados os campos description, speed e visibility na string de exibição
                        // A visibilidade vem em metros (ex: 10000 m = 10 km)
                        double visibilidadeKm = (t.visibility ?? 0) / 1000.0;

                        string dados_previsao = $"Descrição: {t.description} \n" +
                                                 $"Vento: {t.speed} m/s \n" +
                                                 $"Visibilidade: {visibilidadeKm:F1} km \n" +
                                                 $"Latitude: {t.lat} \n" +
                                                 $"Longitude: {t.lon} \n" +
                                                 $"Nascer do Sol: {t.sunrise} \n" +
                                                 $"Por do Sol: {t.sunset} \n" +
                                                 $"Temp Máx: {t.temp_max}°C \n" +
                                                 $"Temp Min: {t.temp_min}°C \n";

                        lbl_res.Text = dados_previsao;
                    }
                    else
                    {
                        lbl_res.Text = "Sem dados de Previsão";
                    }
                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }
}
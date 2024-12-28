using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

[Route("api/hairstyle")]
[ApiController]
public class HairStyleApiController : ControllerBase
{
    [HttpPost("suggest")]
    public async Task<IActionResult> SuggestHairStyle(
        [FromForm] IFormFile image, 
        [FromForm] string? param1 = null,     // 1. tercihin stili
        [FromForm] string? param2 = null,     // 1. tercihin rengi
        [FromForm] string? param3 = null,     // 2. tercihin stili
        [FromForm] string? param4 = null,     // 2. tercihin rengi
        [FromForm] string? param5 = null,     // 3. tercihin stili
        [FromForm] string? param6 = null)     // 3. tercihin rengi
    {
        try
        {
            var suggestions = new List<string>();
            var generatedImages = new List<string>();

            // İlk tercih
            if (!string.IsNullOrEmpty(param1))
            {
                var (image1, suggestion1) = await ProcessStyleRequest(image, param1, param2 ?? "default");
                if (image1 != null)
                {
                    generatedImages.Add(image1);
                    suggestions.Add(suggestion1);
                }
            }

            // İkinci tercih
            if (!string.IsNullOrEmpty(param3))
            {
                var (image2, suggestion2) = await ProcessStyleRequest(image, param3, param4 ?? "default");
                if (image2 != null)
                {
                    generatedImages.Add(image2);
                    suggestions.Add(suggestion2);
                }
            }

            // Üçüncü tercih
            if (!string.IsNullOrEmpty(param5))
            {
                var (image3, suggestion3) = await ProcessStyleRequest(image, param5, param6 ?? "default");
                if (image3 != null)
                {
                    generatedImages.Add(image3);
                    suggestions.Add(suggestion3);
                }
            }

            // Eğer hiç tercih yoksa
            if (!generatedImages.Any())
            {
                return BadRequest(new { error = "En az bir saç modeli seçmelisiniz." });
            }

            var response = new
            {
                images = generatedImages.ToArray(),
                suggestion = string.Join("\n", suggestions)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private async Task<(string imageUrl, string suggestion)> ProcessStyleRequest(IFormFile originalImage, string styleModel, string styleColor)
    {
        try
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://hairstyle-changer-pro.p.rapidapi.com/facebody/editing/hairstyle-pro"),
                Headers =
                {
                    { "x-rapidapi-key", "181d7aee43mshdd862dcbf9cd2a7p1a1e3ajsnae94312e64eb" },
                    { "x-rapidapi-host", "hairstyle-changer-pro.p.rapidapi.com" },
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "task_type", "async" },
                    { "hair_style", styleModel },
                    { "color", styleColor }
                })
            };

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            // JSON yanıtı deserialize et
            var responseData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
            
            // task_id'yi al
            if (responseData != null && responseData.TryGetValue("task_id", out string? taskId))
            {
                return (
                    imageUrl: taskId, // task_id'yi imageUrl olarak döndür
                    suggestion: $"{styleModel} modeli {styleColor} rengi ile işleniyor. Task ID: {taskId}"
                );
            }
            else
            {
                throw new Exception("API yanıtında task_id bulunamadı");
            }
        }
        catch (Exception ex)
        {
            return (
                imageUrl: "https://example.com/default.jpg",
                suggestion: $"Üzgünüz, bir hata oluştu: {ex.Message}"
            );
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

[Route("api/hairstyle")]
[ApiController]
public class HairStyleApiController : ControllerBase
{
    [HttpPost("suggest")]
    public async Task<IActionResult> SuggestHairStyle([FromForm] IFormFile image, [FromForm] string param1, [FromForm] string param2, [FromForm] string param3, [FromForm] string param4, [FromForm] string param5, [FromForm] string param6)
    {
        try
        {
            var suggestions = new List<string>();
            var generatedImages = new List<string>();

            // İlk tercih
            if (!string.IsNullOrEmpty(param1))
            {
                var (image1, suggestion1) = await ProcessStyleRequest(image, param1, param2);
                if (image1 != null)
                {
                    generatedImages.Add(image1);
                    suggestions.Add(suggestion1);
                }
            }

            // İkinci tercih
            if (!string.IsNullOrEmpty(param3))
            {
                var (image2, suggestion2) = await ProcessStyleRequest(image, param3, param4);
                if (image2 != null)
                {
                    generatedImages.Add(image2);
                    suggestions.Add(suggestion2);
                }
            }

            // Üçüncü tercih
            if (!string.IsNullOrEmpty(param5))
            {
                var (image3, suggestion3) = await ProcessStyleRequest(image, param5, param6);
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
            // TODO: Burada gerçek API çağrısı yapılacak
            // Şimdilik test verisi döndürüyoruz
            return (
                $"https://example.com/generated_{styleModel}_{styleColor}.jpg",
                $"{styleModel} modeli {styleColor} rengi ile harika görünecek!"
            );
        }
        catch (Exception)
        {
            return (null, null);
        }
    }
} 
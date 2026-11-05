using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using HeritageAI.Application.Interfaces;

namespace HeritageAI.API.Services
{
    public class AiDesignRequestDto
    {
        public string Prompt { get; set; } = string.Empty;
    }

    public class AiDesignIdeaDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EmbroideryStyle { get; set; } = string.Empty;
        public List<string> Palette { get; set; } = new();   // الأول = لون الخلفية، والباقي ألوان الغرز
        public string Garment { get; set; } = "cushion";      // dress | shawl | cushion
        public List<string> Motifs { get; set; } = new();     // star, cypress, olive, diamond, flower
        public int Seed { get; set; }
    }

    public class CraftsmanSuggestionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductsCount { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class AiDesignResponseDto
    {
        public string Summary { get; set; } = string.Empty;
        public bool IsDemo { get; set; }
        public List<AiDesignIdeaDto> Designs { get; set; } = new();
        public List<CraftsmanSuggestionDto> Craftsmen { get; set; } = new();
    }

    internal class AiConcept
    {
        public string Summary { get; set; } = string.Empty;
        public List<string> Keywords { get; set; } = new();
        public List<AiDesignIdeaDto> Designs { get; set; } = new();
    }

    public interface IAiDesignService
    {
        Task<AiDesignResponseDto> GenerateAsync(AiDesignRequestDto request);
    }

    public class AiDesignService : IAiDesignService
    {
        private static readonly Regex HexColor = new("^#[0-9a-fA-F]{6}$");
        private static readonly string[] AllowedMotifs = { "star", "cypress", "olive", "diamond", "flower" };
        private static readonly string[] AllowedGarments = { "dress", "shawl", "cushion" };
        private static readonly string[] FallbackPalette = { "#F5EFE6", "#7A1F2B", "#C9A24A", "#1F3A5F" };

        private static readonly Dictionary<string, string> MotifNames = new()
        {
            ["star"] = "نجمة القدس",
            ["cypress"] = "السرو",
            ["olive"] = "غصن الزيتون",
            ["diamond"] = "المعين",
            ["flower"] = "الزهرة"
        };

        private static readonly (string[] Words, string Name, string Hex)[] ColorMap =
        {
            (new[] { "أحمر", "احمر", "حمراء", "حمرا" }, "الأحمر", "#C0272D"),
            (new[] { "أخضر", "اخضر", "خضراء", "خضرا" }, "الأخضر", "#2E7D4F"),
            (new[] { "أزرق", "ازرق", "زرقاء", "كحلي" }, "الأزرق", "#1F3A5F"),
            (new[] { "ذهبي", "دهبي", "ذهب" }, "الذهبي", "#C9A24A"),
            (new[] { "أسود", "اسود", "سوداء" }, "الأسود", "#1C1A17"),
            (new[] { "أبيض", "ابيض", "بيضاء" }, "الأبيض", "#FFFFFF"),
            (new[] { "برتقالي" }, "البرتقالي", "#D9792B"),
            (new[] { "بنفسجي", "موف" }, "البنفسجي", "#6B3FA0"),
            (new[] { "وردي" }, "الوردي", "#D86A8F"),
            (new[] { "بني" }, "البني", "#6B4423"),
            (new[] { "أصفر", "اصفر" }, "الأصفر", "#E2B93B"),
            (new[] { "عنابي" }, "العنابي", "#7A1F2B"),
            (new[] { "تركوازي", "فيروزي" }, "التركوازي", "#2A9D8F"),
        };

        private const string SystemPrompt = """
            أنت مصمم تطريز فلسطيني تراثي خبير في موقع غُرزّة.
            المشتري بيوصفلك قطعة بدو ياها (ممكن يذكر القياسات والألوان والمناسبة).
            ارجع JSON فقط، بدون أي نص قبله أو بعده، بالشكل التالي بالضبط:
            {
              "summary": "جملتين بالعربية بتلخص فهمك للطلب وبتراعي القياسات إذا انذكرت",
              "keywords": ["كلمات مفتاحية قصيرة بالعربية للقطعة ونمط التطريز"],
              "designs": [
                {
                  "title": "اسم قصير للتصميم",
                  "description": "وصف التصميم: القصّة، توزيع التطريز، الرموز المستخدمة",
                  "embroideryStyle": "نمط التطريز",
                  "garment": "dress أو shawl أو cushion",
                  "motifs": ["3 رموز مختارة فقط من: star, cypress, olive, diamond, flower"],
                  "palette": ["#RRGGBB لون الخلفية أولاً", "#RRGGBB", "#RRGGBB", "#RRGGBB"]
                }
              ]
            }
            garment: dress للثوب، shawl للشال، cushion للوسادة وأي قطعة تانية.
            اعطِ 3 تصاميم مختلفة وحصرية. احترم الألوان اللي طلبها المشتري. الألوان بصيغة hex فقط.
            """;

        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly IProductRepository _products;
        private readonly ILogger<AiDesignService> _logger;

        public AiDesignService(
            HttpClient http,
            IConfiguration config,
            IProductRepository products,
            ILogger<AiDesignService> logger)
        {
            _http = http;
            _config = config;
            _products = products;
            _logger = logger;
        }

        public async Task<AiDesignResponseDto> GenerateAsync(AiDesignRequestDto request)
        {
            var prompt = request.Prompt?.Trim() ?? "";
            if (prompt.Length < 10)
                throw new InvalidOperationException("Please describe your idea (at least 10 characters).");
            if (prompt.Length > 600)
                throw new InvalidOperationException("Description is too long.");

            var isClaude = string.Equals(_config["Ai:Mode"], "Claude", StringComparison.OrdinalIgnoreCase);
            var concept = isClaude ? await CallClaudeAsync(prompt) : BuildMock(prompt);

            return new AiDesignResponseDto
            {
                Summary = concept.Summary,
                IsDemo = !isClaude,
                Designs = concept.Designs,
                Craftsmen = await RecommendCraftsmenAsync(concept)
            };
        }

        // =====================================================
        // المولّد الخوارزمي (بدون ذكاء اصطناعي)
        // =====================================================
        private static AiConcept BuildMock(string prompt)
        {
            var tokens = Regex.Split(prompt, @"[^\p{L}\p{N}]+").Where(t => t.Length > 0).ToList();

            var garment = DetectGarment(prompt);
            var label = garment switch { "dress" => "ثوب", "shawl" => "شال", _ => "وسادة" };
            var flag = prompt.Contains("علم") || prompt.Contains("فلسطين");
            var found = DetectColors(tokens);

            List<List<string>> palettes;
            List<string> colorNames;

            if (flag)
            {
                palettes = new()
                {
                    new() { "#FFFFFF", "#111111", "#007A3D", "#CE1126" },
                    new() { "#111111", "#FFFFFF", "#007A3D", "#CE1126" },
                    new() { "#007A3D", "#FFFFFF", "#CE1126", "#111111" },
                };
                colorNames = new() { "الأسود", "الأبيض", "الأخضر", "الأحمر" };
            }
            else
            {
                var c = found.Select(f => f.Hex).ToList();
                foreach (var d in new[] { "#7A1F2B", "#C9A24A", "#1F3A5F" })
                    if (c.Count < 3 && !c.Contains(d)) c.Add(d);

                var v1Bg = c.Any(IsLight) ? "#1F2A38" : "#F5EFE6";
                var v2Bg = c.FirstOrDefault(h => !IsLight(h)) ?? "#1F2A38";

                palettes = new()
                {
                    BuildPalette(v1Bg, c),
                    BuildPalette(v2Bg, new[] { "#F5EFE6" }.Concat(c)),
                    BuildPalette("#1F2A38", new[] { "#F5EFE6" }.Concat(c)),
                };
                colorNames = found.Count > 0
                    ? found.Select(f => f.Name).ToList()
                    : new() { "العنابي", "الذهبي", "الكحلي" };
            }

            var sets = new[]
            {
                new[] { "star", "cypress", "diamond" },
                new[] { "flower", "olive", "diamond" },
                new[] { "cypress", "star", "flower" },
            };
            if (prompt.Contains("زيتون")) sets[0] = new[] { "olive", "star", "diamond" };

            var titles = new[] { $"{label} نجمة القدس", $"{label} الزهر والزيتون", $"{label} درب السرو" };
            var styles = new[] { "تطريز قدسي", "تطريز فلاحي", "تطريز مختلط" };

            var layout = garment switch
            {
                "dress" => "الوحدة الرئيسية بالصدر، وزخارف عمودية على الأكمام، وحاشية مزخرفة بالذيل",
                "shawl" => "إطار بحواف الشال، ووحدة مركزية تحيط فيها زخارف جانبية متكررة",
                _ => "وحدة مركزية كبيرة مع زخارف بالزوايا وإطار حولها"
            };

            var designs = new List<AiDesignIdeaDto>();
            for (var i = 0; i < 3; i++)
            {
                designs.Add(new AiDesignIdeaDto
                {
                    Title = titles[i],
                    EmbroideryStyle = styles[i],
                    Description = $"{layout}. الرموز: {string.Join(" و", sets[i].Select(m => MotifNames[m]))}.",
                    Palette = palettes[i],
                    Garment = garment,
                    Motifs = sets[i].ToList(),
                    Seed = Random.Shared.Next(1, 100000)
                });
            }

            var colorsText = flag ? "ألوان العلم الفلسطيني" : string.Join(" و", colorNames);

            var sizeText = "";
            var w = Regex.Match(prompt, @"وزن\w*\s*(\d{2,3})");
            if (garment == "dress" && w.Success && int.TryParse(w.Groups[1].Value, out var kg))
            {
                var size = kg <= 55 ? "S" : kg <= 68 ? "M" : kg <= 82 ? "L" : "XL";
                sizeText = $" المقاس المقترح تقريباً: {size} (بيتأكد مع الحرفي).";
            }

            var keywords = tokens.Where(t => t.Length >= 3).Distinct().Take(8)
                .Concat(new[] { label })
                .Concat(colorNames)
                .Concat(sets.SelectMany(s => s).Distinct().Select(m => MotifNames[m]))
                .Concat(styles)
                .ToList();

            return new AiConcept
            {
                Summary = $"فهمت طلبك: {label} بألوان {colorsText}.{sizeText} " +
                          "هاي 3 تصاميم حصرية مولّدة من وحدات التطريز الفلسطيني، وبتتغيّر كل مرة بتضغط فيها (صمّملي).",
                Keywords = keywords,
                Designs = designs
            };
        }

        private static string DetectGarment(string prompt)
        {
            if (prompt.Contains("شال") || prompt.Contains("وشاح")) return "shawl";
            if (prompt.Contains("وسادة") || prompt.Contains("مخدة") || prompt.Contains("كوشن")) return "cushion";
            if (prompt.Contains("ثوب") || prompt.Contains("فستان") || prompt.Contains("عباية")) return "dress";
            return "cushion";
        }

        private static List<(string Name, string Hex)> DetectColors(List<string> tokens)
        {
            var result = new List<(string Name, string Hex)>();
            foreach (var t in tokens)
            {
                var variants = Variants(t);
                foreach (var entry in ColorMap)
                {
                    if (variants.Any(v => entry.Words.Contains(v)) && !result.Any(r => r.Hex == entry.Hex))
                        result.Add((entry.Name, entry.Hex));
                }
            }
            return result;
        }

        // الكلمة نفسها + الكلمة بعد إزالة البادئات (بال، وال، ال، و، ب)
        private static List<string> Variants(string token)
        {
            var list = new List<string> { token };
            foreach (var p in new[] { "وبال", "بال", "وال", "ال", "و", "ب" })
                if (token.StartsWith(p) && token.Length - p.Length >= 2)
                    list.Add(token[p.Length..]);
            return list;
        }

        private static bool IsLight(string hex)
        {
            var h = hex.TrimStart('#');
            if (h.Length != 6) return false;
            var r = Convert.ToInt32(h[..2], 16);
            var g = Convert.ToInt32(h.Substring(2, 2), 16);
            var b = Convert.ToInt32(h.Substring(4, 2), 16);
            return (0.299 * r + 0.587 * g + 0.114 * b) / 255 > 0.75;
        }

        private static List<string> BuildPalette(string bg, IEnumerable<string> pool)
        {
            var list = new List<string> { bg };
            foreach (var h in pool.Where(h => !h.Equals(bg, StringComparison.OrdinalIgnoreCase)).Distinct())
                if (list.Count < 4) list.Add(h);
            foreach (var h in new[] { "#F5EFE6", "#C9A24A", "#1F2A38" })
                if (list.Count < 4 && !list.Contains(h)) list.Add(h);
            return list;
        }

        // =====================================================
        // Claude الحقيقي
        // =====================================================
        private async Task<AiConcept> CallClaudeAsync(string prompt)
        {
            var apiKey = _config["Ai:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("AI service is not configured.");

            var body = new
            {
                model = _config["Ai:Model"] ?? "claude-sonnet-5-5",
                max_tokens = 4000,
                system = SystemPrompt,
                messages = new[] { new { role = "user", content = prompt } }
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
            req.Headers.Add("x-api-key", apiKey);
            req.Headers.Add("anthropic-version", "2023-06-01");
            req.Content = JsonContent.Create(body);

            using var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadAsStringAsync();
                _logger.LogError("Anthropic API error {Status}: {Body}", (int)res.StatusCode, err);
                throw new InvalidOperationException("AI service is unavailable.");
            }

            try
            {
                using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
                var text = doc.RootElement.GetProperty("content")[0].GetProperty("text").GetString() ?? "";

                var start = text.IndexOf('{');
                var end = text.LastIndexOf('}');
                if (start < 0 || end <= start)
                    throw new InvalidOperationException("AI service is unavailable.");

                var concept = JsonSerializer.Deserialize<AiConcept>(
                    text[start..(end + 1)],
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (concept is null || concept.Designs.Count == 0)
                    throw new InvalidOperationException("AI service is unavailable.");

                concept.Designs = concept.Designs.Take(4).ToList();
                foreach (var d in concept.Designs)
                {
                    d.Palette = d.Palette.Where(c => HexColor.IsMatch(c)).Take(6).ToList();
                    foreach (var f in FallbackPalette)
                        if (d.Palette.Count < 4 && !d.Palette.Contains(f)) d.Palette.Add(f);

                    if (!AllowedGarments.Contains(d.Garment)) d.Garment = "cushion";

                    d.Motifs = d.Motifs.Where(m => AllowedMotifs.Contains(m)).Distinct().ToList();
                    foreach (var m in AllowedMotifs)
                        if (d.Motifs.Count < 3 && !d.Motifs.Contains(m)) d.Motifs.Add(m);

                    d.Seed = Random.Shared.Next(1, 100000);
                }
                return concept;
            }
            catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
            {
                _logger.LogError(ex, "Failed to parse AI response");
                throw new InvalidOperationException("AI service is unavailable.");
            }
        }

        // =====================================================
        // ترشيح الحرفيين من قاعدة البيانات
        // =====================================================
        private async Task<List<CraftsmanSuggestionDto>> RecommendCraftsmenAsync(AiConcept concept)
        {
            var products = (await _products.GetPublishedAsync())
                .Where(p => p.Craftsman.User.IsActive)
                .ToList();

            var keywords = concept.Keywords
                .Concat(concept.Designs.Select(d => d.EmbroideryStyle))
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Select(k => k.Trim().ToLowerInvariant())
                .Distinct().ToList();

            return products
                .GroupBy(p => p.CraftsmanId)
                .Select(g =>
                {
                    var text = string.Join(' ', g.Select(p =>
                        $"{p.Name} {p.Description} {p.Category.Name} {p.EmbroideryStyle}")).ToLowerInvariant();
                    var images = g.SelectMany(p => p.Images).ToList();

                    return new
                    {
                        Score = keywords.Count(k => text.Contains(k)),
                        Dto = new CraftsmanSuggestionDto
                        {
                            Id = g.Key,
                            Name = g.First().Craftsman.User.FullName,
                            ProductsCount = g.Count(),
                            ImageUrl = (images.FirstOrDefault(i => i.IsMain) ?? images.FirstOrDefault())?.Url
                        }
                    };
                })
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Dto.ProductsCount)
                .Take(3)
                .Select(x => x.Dto)
                .ToList();
        }
    }
}
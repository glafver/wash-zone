namespace WashZone.Models.UI
{
    /// <summary>Reusable text/number/email input field.</summary>
    public class FieldModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Label { get; set; }
        public string? Value { get; set; }
        public string Type { get; set; } = "text";
        public string? Placeholder { get; set; }
        public bool Required { get; set; }
        public string? Autocomplete { get; set; }
        public string? OnChange { get; set; }
        public string? Class { get; set; }
    }

    /// <summary>Reusable dropdown (select) field.</summary>
    public class SelectModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Label { get; set; }
        public List<SelectOptionModel> Options { get; set; } = new();
        public bool Required { get; set; }
        public string? OnChange { get; set; }
        public string? Class { get; set; }
    }

    public class SelectOptionModel
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }

    /// <summary>Reusable link button.</summary>
    public class ButtonModel
    {
        public string Text { get; set; } = string.Empty;
        public string? Page { get; set; }
        public string? Href { get; set; }
        public int? RouteId { get; set; }
        public string Variant { get; set; } = "dark";
        public string Size { get; set; } = string.Empty;
        public string? Class { get; set; }
    }
}

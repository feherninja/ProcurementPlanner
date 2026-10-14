using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace DeliveryManager.Components;

// Decimal price input that accepts both "12,5" and "12.5", whatever the Windows culture is
public partial class PriceFieldBase : ComponentBase
{
	[Parameter]
	public decimal Value { get; set; }

	[Parameter]
	public EventCallback<decimal> ValueChanged { get; set; }

	[Parameter]
	public string Label { get; set; } = "Ár";

	protected string text = string.Empty;

	protected override void OnParametersSet()
	{
		// Only reformat when the value changed from outside, so typing "12," is not overwritten
		if (!TryParse(text, out var current) || current != Value)
			text = Value.ToString("0.##########", CultureInfo.CurrentCulture);
	}

	protected async Task OnTextChanged(string? value)
	{
		text = value ?? string.Empty;

		if (TryParse(text, out var parsed))
			await ValueChanged.InvokeAsync(parsed);
	}

	protected string? Validate(string? value) =>
		TryParse(value, out _) ? null : "Érvénytelen ár, pl. 12,50 vagy 12.50";

	private static bool TryParse(string? value, out decimal result)
	{
		var normalized = (value ?? string.Empty)
		                 .Replace(" ", "")
		                 .Replace(" ", "")
		                 .Replace(',', '.');

		if (normalized.Length == 0)
		{
			result = 0;
			return true;
		}

		return decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result);
	}
}

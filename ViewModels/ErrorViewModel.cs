namespace ClickAndGoApp.ViewModels;

public record ErrorViewModel(string? RequestId)
{
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

using AdvancedSharpAdbClient;
using FacebookCreator.MuiltiTask;

namespace FacebookCreator.Models
{
    public class DeviceInfo : BaseViewModel
    {
        public ViewInfo? View { get; set; } 
        public string? Id { get; set; }
        public string? Status { get; set; }
        public DeviceData Data { get; set; }
        public AdbClient AdbClient { get; set; }
        public string? IndexLDPlayer { get; set; }
    }
    public class ViewInfo
    {
        public nint LdplayerHandle { get; set; }
        public nint originalParentHandle { get; set; }
        public bool IsPinned { get; set; }
        public Label StatusLabel { get; set; }
        public Panel Panel { get; set; }
        public Panel Embeddedpanel { get; set; }
        public Button BtnClose { get; set; }
        public Panel PanelButton { get; set; }
    }
}

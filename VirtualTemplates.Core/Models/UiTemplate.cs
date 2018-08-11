namespace VirtualTemplates.Core.Models
{
    /// <summary>
    /// Used to display template listings in the UI
    /// </summary>
    public class UiTemplate
    {
        public bool IsVirtual { get; set; }
        public string FilePath { get; set; }
        public string ChangedBy { get; set; }

    }
}

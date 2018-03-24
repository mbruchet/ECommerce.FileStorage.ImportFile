namespace Import.Abstractions
{
    public class TransformationSettings
    {
        public string TargetFolder { get; set; }
        public bool HeaderOnFirstRow { get; set; }
        public string Separator { get; set; }
    }
}

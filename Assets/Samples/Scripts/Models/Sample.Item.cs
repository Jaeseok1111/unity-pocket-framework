using ThridParty.SQLite;

namespace Sample
{
    [GameChart(Name = "item")]
    [Table(Name = "items")]
    public partial class ItemData
    {
        [GameChartColumn(Name = "templateId")]
        [Field(Name = "templateId", NotNull = true, PrimaryKey = true)]
        public int TemplateId;

        [GameChartColumn(Name = "name")]
        [Field(Name = "name", NotNull = true)]
        public string Name;
    }
}

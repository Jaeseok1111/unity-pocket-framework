using System;
using ThePocket;

namespace Sample
{
    [Model(Name = "items", Usage = ModelUsageTargets.GameChart)]
    public partial class ItemData
    {
        [Field(Name = "templateId", NotNull = true, PrimaryKey = true)]
        public int TemplateId;

        [Field(Name = "name", NotNull = true)]
        public string Name;

        [Field(Name = "created", NotNull = true)]
        public DateTime Created = DateTime.Now;
    }
}
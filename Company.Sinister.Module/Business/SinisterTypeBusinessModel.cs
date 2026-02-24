using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Business
{
    public class SinisterTypeBusinessModel
    {
        public long Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public static SinisterTypeBusinessModel From(SinisterTypeDataModel dataModel)
        {
            return new SinisterTypeBusinessModel
            {
                Id = dataModel.Id,
                TypeName = dataModel.TypeName
            };
        }

        public SinisterTypeDataModel ToDataModel()
        {
            return new SinisterTypeDataModel
            {
                Id = Id,
                TypeName = TypeName
            };
        }
    }
}

using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class CategoryEntity : IUserEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public byte ColorA { get; set; }
        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }

        public bool IsDeleted { get; set; }

        public IList<OutcomeCategoryEntity> Outcomes { get; set; }

        public CategoryEntity()
        { }

        public CategoryEntity(CategoryModel model)
        {
            Id = model.Key.AsGuidKey().Guid;
            Name = model.Name;
            Description = model.Description;
            SetColor(model.Color);
            Icon = model.Icon;
        }

        public void SetColor(Color color)
        {
            ColorA = color.A;
            ColorR = color.R;
            ColorG = color.G;
            ColorB = color.B;
        }

        public CategoryModel ToModel()
        {
            return new CategoryModel(
                GuidKey.Create(Id, KeyFactory.Empty(typeof(Category)).Type),
                Name,
                Description,
                ToColor(),
                Icon
            );
        }

        public Color ToColor()
        {
            return Color.FromArgb(ColorA, ColorR, ColorG, ColorB);
        }
    }
}

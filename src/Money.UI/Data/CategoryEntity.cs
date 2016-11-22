using Money.Services.Models;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money.Data
{
    internal class CategoryEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte ColorA { get; set; }
        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }

        public IList<OutcomeCategoryEntity> Outcomes { get; set; }

        public CategoryEntity()
        { }

        public CategoryEntity(CategoryModel model)
        {
            Id = model.Key.AsGuidKey().Guid;
            Name = model.Name;
            ColorA = model.Color.A;
            ColorR = model.Color.R;
            ColorG = model.Color.G;
            ColorB = model.Color.B;
        }

        public CategoryModel ToModel()
        {
            return new CategoryModel(
                GuidKey.Create(Id, KeyFactory.Empty(typeof(Category)).Type),
                Name,
                Color.FromArgb(ColorA, ColorR, ColorG, ColorB)
            );
        }
    }
}

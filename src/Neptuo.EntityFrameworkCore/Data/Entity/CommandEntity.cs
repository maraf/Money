using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data.Entity
{
    public class CommandEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public Guid CommandID { get; set; }
        public string CommandType { get; set; }

        public string Payload { get; set; }
        public DateTime RaisedAt { get; set; }

        public CommandModel ToModel()
        {
            return new CommandModel(GuidKey.Create(CommandID, CommandType), Payload)
            {
                RaisedAt = RaisedAt
            };
        }

        public static CommandEntity FromModel(CommandModel model)
        {
            Ensure.NotNull(model, "model");

            GuidKey commandKey = model.CommandKey as GuidKey;
            if (commandKey == null)
                throw Ensure.Exception.NotGuidKey(model.CommandKey.GetType(), "commandKey");

            return new CommandEntity()
            {
                CommandID = commandKey.Guid,
                CommandType = commandKey.Type,

                Payload = model.Payload,
                RaisedAt = model.RaisedAt
            };
        }
    }
}

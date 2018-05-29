using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Confirmation
{
    /// <summary>
    /// Confirmation dialog state.
    /// </summary>
    /// <typeparam name="T">A type of the model to delete.</typeparam>
    public class DeleteContext<T> : ConfirmContext
    {
        private T model;

        /// <summary>
        /// A formatter delegate for creating <see cref="Message"/> from <see cref="Model"/>.
        /// </summary>
        public Func<T, string> MessageFormatter { get; set; }

        /// <summary>
        /// Gets or sets a model.
        /// </summary>
        public T Model
        {
            get => model;
            set
            {
                model = value;

                if (model != null)
                {
                    Message = MessageFormatter?.Invoke(model);
                    IsVisible = true;
                }
                else
                {
                    Message = null;
                    IsVisible = false;
                }
            }
        }

        /// <summary>
        /// Actions raised when action was confirmed.
        /// </summary>
        public event Action<T> Confirmed;

        /// <summary>
        /// Runs <see cref="Confirmed"/>.
        /// </summary>
        public override void OnConfirmed()
        {
            Confirmed?.Invoke(Model);
            Model = default;
        }
    }
}

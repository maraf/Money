using Microsoft.AspNetCore.Components;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class TemplateService
    {
        private readonly Dictionary<string, TemplatePlaceholder> declarations = new Dictionary<string, TemplatePlaceholder>();

        public void DeclarePlaceholder(string name, TemplatePlaceholder component)
        {
            Ensure.NotNull(name, "name");
            Ensure.NotNull(component, "component");
            declarations[name] = component;
        }

        public void DisposePlaceholder(string name, TemplatePlaceholder component)
        {
            Ensure.NotNull(name, "name");
            Ensure.NotNull(component, "component");
            if (declarations.TryGetValue(name, out var current) && current == component)
                declarations.Remove(name);
        }

        public void UseContent(string name, RenderFragment content)
        {
            Ensure.NotNull(name, "name");
            if (declarations.TryGetValue(name, out var placeholder))
                placeholder.UseContent(content);
            else
                throw Ensure.Exception.InvalidOperation($"Missing placeholder named '{name}'.");
        }
    }
}

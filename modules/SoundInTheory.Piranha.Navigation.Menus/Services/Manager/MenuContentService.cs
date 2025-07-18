using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Manager.Models.Content;
using Piranha.Models;
using Piranha.Runtime;
using SoundInTheory.Piranha.Navigation.Models;
using SoundInTheory.Piranha.Navigation.Models.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundInTheory.Piranha.Navigation.Services.Manager
{
    public class MenuContentService
    {
        private readonly IServiceProvider _services;

        public MenuContentService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<MenuItemTypeModel[]> GetAllItemTypes()
        {
            var output = new List<MenuItemTypeModel>();

            foreach (var item in MenuModule.Instance.MenuItems.GetAll())
            {
                output.Add(await GetItemModel(item));
            }

            return output.ToArray();
        }

        public async Task<MenuItemTypeModel> GetItemModel(MenuItemDefinition definition)
        {
            if (definition == null)
            {
                return null;
            }

            var model =  new MenuItemTypeModel
            {
                Id = definition.Id,
                Title = definition.Title,
                Type = definition.TypeName,
                ListTitle = definition.ListTitle,
                ListType = definition.ListType,
                Description = definition.Description,
                AllowedChildren = definition.AllowedChildren?.Select(t => MenuModule.Instance.MenuItems.GetByType(t)?.Id)?.Where(x => x != null)?.ToArray() ?? Array.Empty<string>(),
                AllowedParents = definition.AllowedParents?.Select(t => t == typeof(Menu) ? "root" : MenuModule.Instance.MenuItems.GetByType(t)?.Id)?.Where(x => x != null)?.ToArray() ?? Array.Empty<string>(),
                MaxLevel = definition.MaxLevel,
                MaxDepth = definition.MaxDepth,
                Fields = new List<FieldModel>()
            };

            using var scope = _services.CreateScope();

            foreach (var fieldType in definition.Fields)
            {
                var appFieldType = App.Fields.GetByType(fieldType.Type);
                var field = new FieldModel
                {
                    Model = (IField)Activator.CreateInstance(appFieldType.Type),
                    Meta = new FieldMeta
                    {
                        Id = fieldType.Id,
                        Name = fieldType.Title,
                        Component = fieldType.Settings.TryGetValue("Component", out var component) ? (string)component : appFieldType?.Component ?? "string-field",
                        Placeholder = fieldType.Placeholder,
                        IsHalfWidth = fieldType.Options.HasFlag(FieldOption.HalfWidth),
                        IsTranslatable = typeof(ITranslatable).IsAssignableFrom(appFieldType.Type),
                        Description = fieldType.Description,
                        Settings = fieldType.Settings
                    }
                };

                await appFieldType.Init.InvokeAsync(field.Model, scope, true);

                if (field.Model is SelectFieldBase selectField)
                {
                    foreach (var item in selectField.Items)
                    {
                        field.Meta.Options.Add(Convert.ToInt32(item.Value), item.Title);
                    }
                }

                model.Fields.Add(field);
            }

            return model;
        }
    }
}

using SysCommand.ConsoleApp;
using SysCommand.ConsoleApp.View.TemplatesGenerator.Razor;
using SysCommand.Mapping;

namespace SysCommand.Tests.UnitTests.Common.Commands.ViewModels
{
    public class RazorCommand : Command
    {
        private string content = @"
@using SysCommand.Tests.UnitTests.Common.Commands.ViewModels
@model RazorCommand.MyModel
@if(Model == null)
{
    <text>WithoutModel</text>
}
else
{
    @:With: @Model.Name
}";

        #region Razor by string

        public string WithContentWithoutModel()
        {
            return ViewContent(content.Replace("@model RazorCommand.MyModel", ""));
        }

        public string WithContentWithModel()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return ViewContent(content, model);
        }

        public string WithContentWithoutModelAsync()
        {
            return ViewContentAsync(content.Replace("@model RazorCommand.MyModel", "")).Result;
        }

        public string WithContentWithModelAsync()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return ViewContentAsync(content, model).Result;
        }

        #endregion

        #region Razor with file system and no view specify

        public string WithFileSystemWithoutViewNameWithoutModel()
        {
            return View();
        }

        public string WithFileSystemWithoutViewNameWithDynamicModel()
        {
            return View(new { Name = "MyName" });
        }

        public string WithFileSystemWithoutViewNameWithoutModelAsync()
        {
            return ViewAsync().Result;
        }

        public string WithFileSystemWithoutViewNameWithDynamicModelAsync()
        {
            return ViewAsync(new { Name = "MyName" }).Result; ;
        }

        #endregion

        #region Razor with file system and view name specify

        public string WithFileSystemViewNameWithModel()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return View(model, "Views/FileSystemView.cshtml");
        }

        #endregion

        #region Embedded view

        public string WithEmbeddedWithoutViewNameWithoutModel()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return View(model, searchOnlyInResources: true);
        }

        public string WithEmbeddedWithViewNameWithoutModel()
        {
            var model = new MyModel
            {
                Name = "MyName"
            };

            return View(model, "Views/EmbeddedView.cshtml", searchOnlyInResources: true);
        }

        #endregion

        public class MyModel
        {
            public string Name { get; set; }
        }

        public abstract class CustomRazorPage<TModel> : RazorView<TModel>
        {
            public string CustomText { get; } = "CustomText";
            public string GetMethodValue()
            {
                return CustomText;
            }
        }
    }
}

using Abp.Authorization;
using Abp.Localization;

namespace ATI.Revenue.Application
{
    public class RevenueAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages");
            
            // Cases permissions
            var cases = pages.CreateChildPermission("Pages.Cases", L("Cases"));
            cases.CreateChildPermission("Pages.Cases.Create", L("CreateCase"));
            cases.CreateChildPermission("Pages.Cases.Edit", L("EditCase"));
            cases.CreateChildPermission("Pages.Cases.Delete", L("DeleteCase"));

            // Products permissions
            var products = pages.CreateChildPermission("Pages.Products", L("Products"));
            products.CreateChildPermission("Pages.Products.Create", L("CreateProduct"));
            products.CreateChildPermission("Pages.Products.Edit", L("EditProduct"));
            products.CreateChildPermission("Pages.Products.Delete", L("DeleteProduct"));

            // Product Categories permissions
            var categories = pages.CreateChildPermission("Pages.ProductCategories", L("ProductCategories"));
            categories.CreateChildPermission("Pages.ProductCategories.Create", L("CreateProductCategory"));
            categories.CreateChildPermission("Pages.ProductCategories.Edit", L("EditProductCategory"));
            categories.CreateChildPermission("Pages.ProductCategories.Delete", L("DeleteProductCategory"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, "ATI");
        }
    }
}
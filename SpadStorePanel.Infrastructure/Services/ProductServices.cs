using SpadStorePanel.Core.Models;
using SpadStorePanel.Infrastructure.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using SpadStorePanel.Infrastructure.Repositories;

namespace SpadStorePanel.Infrastructure.Services
{
    public class ProductService
    {
        private readonly InvoicesRepository _InvoiceRepo;
        private readonly ProductsRepository _productRepo;
        private readonly ProductGroupsRepository _productGroupRepo;
        private readonly ProductMainFeaturesRepository _productMainFeatureRepo;
        private readonly DiscountsRepository _discountRepo;
        private readonly MyDbContext _context;
        public ProductService(ProductsRepository productRepo,
            InvoicesRepository invoiceRepo,
            ProductMainFeaturesRepository productMainFeatureRepo,
            DiscountsRepository discountRepo, MyDbContext context,
            ProductGroupsRepository productGroupRepo)
        {
            _productRepo = productRepo;
            _InvoiceRepo = invoiceRepo;
            _productMainFeatureRepo = productMainFeatureRepo;
            _discountRepo = discountRepo;
            _context = context;
            _productGroupRepo = productGroupRepo;
        }
        public List<Product> GetTopSoldProducts(int take)
        {
            var products = new List<Product>();
            var TopSoldProducts = _InvoiceRepo.GertTopSoldProducts(take);
            if (TopSoldProducts.Any() == false)
                products = _productRepo.GetNewestProducts(take);
            else
            {
                products = TopSoldProducts;
            }

            return products;
        }

        public int GetProductSoldCount(Product product)
        {
            var amount = 0;
            var invoices = _context.InvoiceItems.Where(i => i.ProductId == product.Id && i.IsDeleted == false).ToList();
            if (invoices.Any())
                amount += invoices.Sum(i => i.Quantity);
            return amount;
        }

        public int GetProductStockCount(int productId)
        {
            var inStock = 0;
            var mainFeature = _productMainFeatureRepo.GetByProductId(productId);
            if (mainFeature != null)
                inStock += mainFeature.Quantity;
            return inStock;
        }
        public int GetProductStockCount(int productId, int mainFeatureId)
        {
            var inStock = 0;
            var mainFeature = _productMainFeatureRepo.Get(mainFeatureId);
            if (mainFeature != null)
                inStock += mainFeature.Quantity;
            return inStock;
        }
        public long GetProductPrice(Product product)
        {
            long price = 0;
            var mainFeature = _productMainFeatureRepo.GetByProductId(product.Id);
            if (mainFeature != null && mainFeature.Quantity > 0)
            {
                price = mainFeature.Price;
            }

            return price;
        }
        public long GetProductPrice(Product product, int mainFeatureId)
        {
            long price = 0;
            var mainFeature = _productMainFeatureRepo.GetByProductId(product.Id, mainFeatureId);
            if (mainFeature != null && mainFeature.Quantity > 0)
            {
                price = mainFeature.Price;
            }

            return price;
        }
        public long GetProductPriceAfterDiscount(Product product)
        {
            var productPrice = GetProductPrice(product);
            var priceAfterDiscount = productPrice;

            // Checking For Product Discount
            var discount = _discountRepo.GetProductDiscount(product.Id);

            // Checking For ProductGroupDiscount
            if (discount == null)
                discount = _discountRepo.GetProductGroupDiscount(product.ProductGroupId ?? 0);

            // Checking For Brand Discount
            if (discount == null)
                discount = _discountRepo.GetBrandDiscount(product.BrandId ?? 0);

            if (discount != null)
            {
                if (discount.DiscountType == 2)
                {
                    priceAfterDiscount -= discount.Amount;
                }
                else if (discount.DiscountType == 1)
                {
                    var discountAmount = (discount.Amount * productPrice / 100);
                    priceAfterDiscount -= discountAmount;
                }
            }

            return priceAfterDiscount;
        }
        public long GetProductPriceAfterDiscount(Product product, int mainFeatureId)
        {
            var productPrice = GetProductPrice(product, mainFeatureId);
            var priceAfterDiscount = productPrice;

            // Checking For Product Discount
            var discount = _discountRepo.GetProductDiscount(product.Id);

            // Checking For ProductGroupDiscount
            if (discount == null)
                discount = _discountRepo.GetProductGroupDiscount(product.ProductGroupId ?? 0);

            // Checking For Brand Discount
            if (discount == null)
                discount = _discountRepo.GetBrandDiscount(product.BrandId ?? 0);

            if (discount != null)
            {
                if (discount.DiscountType == 2)
                {
                    priceAfterDiscount -= discount.Amount;
                }
                else if (discount.DiscountType == 1)
                {
                    var discountAmount = (discount.Amount * productPrice / 100);
                    priceAfterDiscount -= discountAmount;
                }
            }

            return priceAfterDiscount;
        }
        public List<ProductWithPriceDto> GetTopSoldProductsWithPrice(int take)
        {
            var productsDto = new List<ProductWithPriceDto>();
            var products = GetTopSoldProducts(take);

            #region Getting Product Price And Discount

            foreach (var product in products)
            {
                var productDto = CreateProductWithPriceDto(product);
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }
        public List<ProductWithPriceDto> GetRelatedProducts(int productId, int take)
        {
            var productt = _productRepo.Get(productId);
            var relatedProducts = _context.Products
                .Where(p => p.ProductGroupId == productt.ProductGroupId && p.IsDeleted == false && p.Id != productId)
                .Take(take).ToList();
            var productsDto = new List<ProductWithPriceDto>();

            #region Getting Product Price And Discount

            foreach (var product in relatedProducts)
            {
                var productDto = CreateProductWithPriceDto(product);
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }
        public List<ProductWithPriceDto> GetLatestProductsWithPrice(int take, int? skip = null)
        {
            var productsDto = new List<ProductWithPriceDto>();
            var products = _productRepo.GetNewestProducts(take, skip);

            #region Getting Product Price And Discount

            foreach (var product in products)
            {
                var productDto = CreateProductWithPriceDto(product);
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }

        public List<ProductWithPriceDto> GetPopularProductsWithPrice(int take, int? skip = null)
        {
            var productsDto = new List<ProductWithPriceDto>();
            var products = _productRepo.GetAll().Where(x => x.Rate == 5).Take(take);

            #region Getting Product Price And Discount

            foreach (var product in products)
            {
                var productDto = CreateProductWithPriceDto(product);
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }

        public List<ProductWithPriceDto> GetProductsWithPrice()
        {
            var productsDto = new List<ProductWithPriceDto>();
            var products = _productRepo.GetAll();

            #region Getting Product Price And Discount

            foreach (var product in products)
            {
                var productDto = CreateProductWithPriceDto(product);
                productsDto.Add(productDto);
            }
            #endregion
            return productsDto;
        }

        public ProductWithPriceDto GetProductWithPrice(int id)
        {
            var product = _productRepo.Get(id);
            var productDto = CreateProductWithPriceDto(product);

            return productDto;
        }

        public ProductWithPriceDto CreateProductWithPriceDto(int productId)
        {
            var product = _productRepo.Get(productId);
            var productGroup = _productGroupRepo.Get(product.ProductGroupId.Value);
            var price = GetProductPrice(product);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product);
            int discountPercentage = 0;
            if (price != 0 && priceAfterDiscount != 0)
                discountPercentage = (int)(100 - (priceAfterDiscount * 100 / price));

            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.Title,
                ProductGroupId = productGroup.Id,
                ProductGroupName = productGroup.Title,
                Rate = product.Rate,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount,
                DiscountPercentage = discountPercentage
            };
            return productDto;
        }
        public ProductWithPriceDto CreateProductWithPriceDto(int productId, int mainFeatureId)
        {
            var product = _productRepo.Get(productId);
            var productGroup = _productGroupRepo.Get(product.ProductGroupId.Value);
            var price = GetProductPrice(product, mainFeatureId);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product, mainFeatureId);
            int discountPercentage = 0;
            if (price != 0 && priceAfterDiscount != 0)
                discountPercentage = (int)(100 - (priceAfterDiscount * 100 / price));

            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.Title,
                ProductGroupId = productGroup.Id,
                ProductGroupName = productGroup.Title,
                Rate = product.Rate,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount,
                DiscountPercentage = discountPercentage
            };
            return productDto;
        }
        public ProductWithPriceDto CreateProductWithPriceDto(Product product)
        {
            var productGroup = _productGroupRepo.Get(product.ProductGroupId.Value);
            var price = GetProductPrice(product);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product);
            int discountPercentage = 0;
            if (price != 0 && priceAfterDiscount != 0)
                discountPercentage = (int)(100 - (priceAfterDiscount * 100 / price));

            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.Title,
                ProductGroupId = productGroup.Id,
                ProductGroupName = productGroup.Title,
                Rate = product.Rate,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount,
                DiscountPercentage = discountPercentage
            };
            return productDto;
        }
        public ProductWithPriceDto CreateProductWithPriceDto(Product product, int mainFeatureId)
        {
            var productGroup = _productGroupRepo.Get(product.ProductGroupId.Value);
            var price = GetProductPrice(product, mainFeatureId);
            var priceAfterDiscount = GetProductPriceAfterDiscount(product, mainFeatureId);
            int discountPercentage = 0;
            if (price != 0 && priceAfterDiscount != 0)
                discountPercentage = (int)(100 - (priceAfterDiscount * 100 / price));

            var productDto = new ProductWithPriceDto()
            {
                Id = product.Id,
                ShortTitle = product.Title,
                ProductGroupId = productGroup.Id,
                ProductGroupName = productGroup.Title,
                Rate = product.Rate,
                Image = product.Image,
                Price = price,
                PriceAfterDiscount = priceAfterDiscount,
                DiscountPercentage = discountPercentage
            };
            return productDto;
        }
        public List<int> GetAllChildrenProductGroupIds(int id)
        {
            var ids = new List<int>();
            ids.AddRange(_context.ProductGroups.Where(p => p.IsDeleted == false && p.ParentId == id).Select(p => p.Id).ToList());
            foreach (var item in ids.ToList())
            {
                var childIds = GetAllChildrenProductGroupIds(item);
                if (childIds.Any())
                {
                    ids.AddRange(childIds);
                }
            }
            return ids;
        }

        public List<ProductGroupWithProductCountDto> GetPopularProductGroups(int take)
        {
            var dto = new List<ProductGroupWithProductCountDto>();
            var productGroups = _context.ProductGroups.Where(p => p.ParentId != null && p.IsDeleted == false).Take(take)
                .ToList();
            foreach (var item in productGroups)
            {
                var count = _context.Products.Count(p => p.IsDeleted == false && p.ProductGroupId == item.Id);
                var childIds = GetAllChildrenProductGroupIds(item.Id);
                count += childIds.Sum(id => _context.Products.Count(p => p.IsDeleted == false && p.ProductGroupId == id));
                dto.Add(new ProductGroupWithProductCountDto()
                {
                    ProductGroup = item,
                    ProductCount = count
                });
            }

            return dto;
        }
        #region Get Products Grid

        public List<Product> GetProductsGrid(int? productGroupId, List<int> brandIds = null, List<int> subFeatureIds = null, long? fromPrice = null, long? toPrice = null, string searchString = null)
        {
            var products = new List<Product>();
            var count = 0;
            if (productGroupId == null || productGroupId == 0)
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    products = _context.Products.Include(p => p.ProductMainFeatures).Include(p => p.ProductFeatureValues).Where(p => p.IsDeleted == false).OrderByDescending(p => p.InsertDate).ToList();
                }
                else
                {
                    products = _context.Products.Include(p => p.ProductMainFeatures)
                        .Include(p => p.ProductFeatureValues)
                        .Where(p => p.IsDeleted == false && (p.Title.Trim().ToLower().Contains(searchString.Trim().ToLower()) || p.Title.Trim().ToLower().Contains(searchString.Trim().ToLower())))
                        .OrderByDescending(p => p.InsertDate).ToList();
                }
            }
            else
            {
                products = _context.Products.Include(p => p.ProductMainFeatures).Include(p => p.ProductFeatureValues).Where(p => p.IsDeleted == false && p.ProductGroupId == productGroupId).OrderByDescending(p => p.InsertDate).ToList();

                var allChildrenGroups = GetAllChildrenProductGroupIds(productGroupId.Value);
                foreach (var groupId in allChildrenGroups)
                    products.AddRange(_context.Products.Where(p => p.IsDeleted == false && p.ProductGroupId == groupId).OrderByDescending(p => p.InsertDate).ToList());
                if (string.IsNullOrEmpty(searchString) == false)
                {
                    products = products
                        .Where(p => p.IsDeleted == false && (p.Title.Trim().ToLower().Contains(searchString.Trim().ToLower()) || p.Title.Trim().ToLower().Contains(searchString.Trim().ToLower())))
                        .OrderByDescending(p => p.InsertDate).ToList();
                }
            }

            if (brandIds != null && brandIds.Any())
            {
                var productsFilteredByBrand = new List<Product>();
                foreach (var brand in brandIds)
                    productsFilteredByBrand.AddRange(products.Where(p => p.IsDeleted == false && p.BrandId == brand).OrderByDescending(p => p.InsertDate).ToList());
                products = productsFilteredByBrand;
            }
            if (subFeatureIds != null && subFeatureIds.Any(f => f != 0))
            {
                var productsFilteredByFeature = new List<Product>();
                foreach (var subFeature in subFeatureIds.Where(f => f != 0))
                    productsFilteredByFeature.AddRange(products.Where(p => p.ProductFeatureValues.Any(pf => pf.SubFeatureId == subFeature) || p.ProductMainFeatures.Any(pf => pf.SubFeatureId == subFeature)).OrderByDescending(p => p.InsertDate).ToList());
                products = productsFilteredByFeature;
            }

            if (fromPrice != null)
                products = products.Where(p => GetProductPriceAfterDiscount(p) >= fromPrice).ToList();

            if (toPrice != null)
                products = products.Where(p => GetProductPriceAfterDiscount(p) <= toPrice).ToList();

            return products;
        }
        #endregion
    }
}

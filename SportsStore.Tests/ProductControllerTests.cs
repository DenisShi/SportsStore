using System.Collections.Generic;
using System.Linq;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using Xunit;
using SportsStore.Models.ViewModels;

namespace SportsStore.Tests
{
    public class ProductControllerTests
    {
        private Product[] GetTestProducts()
        {
            return new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
            };
        }
       

        [Fact]
        public void Can_Paginate()
        {
            #region Организация
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(GetTestProducts().AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            #endregion

            #region Действие
            ProductsListViewModel result =
                controller.List(null, 2).ViewData.Model as ProductsListViewModel;


            #endregion
                
            #region Утверждение
            Product[] prodArray = result.Products.ToArray();
            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);

            #endregion
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            #region Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(GetTestProducts().AsQueryable());

            ProductController controller = new ProductController(mock.Object) { PageSize = 3 };
            #endregion
            #region Act
            ProductsListViewModel result =
                controller.List(null, 2).ViewData.Model as ProductsListViewModel;
            #endregion
            #region Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
            #endregion
        }
        [Fact]
        public void Can_Filter_Products()
        {
            #region Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(GetTestProducts().AsQueryable()); 
            
            ProductController controller = new ProductController(mock.Object) { PageSize = 3 };
            #endregion

            #region Act
            Product[] result =
                    (controller.List("Cat2", 1).ViewData.Model as ProductsListViewModel)
                    .Products.ToArray();
            #endregion
            #region Assert
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
            #endregion
        }
    }
}

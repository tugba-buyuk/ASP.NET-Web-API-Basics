﻿using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductLinks : IProductLinks
    {
        private readonly IDataShaper<ProductDTO> _shaper;
        private readonly LinkGenerator _linkGenerator;

        public ProductLinks(LinkGenerator linkGenerator, IDataShaper<ProductDTO> shaper)
        {
            _linkGenerator = linkGenerator;
            _shaper = shaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<ProductDTO> productDto, string fields, HttpContext httpContext)
        {
            var shapedProduct = ShapeProduct(productDto, fields);
            if (ShouldGenerateLink(httpContext))
            {
                return ReturnLinkedProducts(productDto, fields, httpContext, shapedProduct);
            }
            return ReturnShapedProducts(shapedProduct);
        }

        private LinkResponse ReturnLinkedProducts(IEnumerable<ProductDTO> productDto, string fields, HttpContext httpContext, List<Entity> shapedProduct)
        {
            var productDtoList=productDto.ToList();
            for (int index = 0; index < productDtoList.Count(); index++)
            {
                var productLinks= CreateForProduct(productDtoList[index],fields,httpContext);
                shapedProduct[index].Add("Links", productLinks);
            }
            var productCollection=new LinkCollectionWrapper<Entity>(shapedProduct);
            CreateForProducts(httpContext, productCollection);
            return new LinkResponse { HasLink = true, LinkedEntites=productCollection };
        }

        private LinkCollectionWrapper<Entity> CreateForProducts(HttpContext httpContext, LinkCollectionWrapper<Entity> productCollectionWrapper)
        {
            productCollectionWrapper.Links.Add(new Link()
            {
                Href = $"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
                Rel = "self",
                Method = "GET"
            });
            return productCollectionWrapper;
        }

        private List<Link> CreateForProduct(ProductDTO productDTO, string fields, HttpContext httpContext)
        {
            var links = new List<Link>()
            {
                new Link()
                {
                    Href=$"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}/{productDTO.Id}",
                    Rel="self",
                    Method="GET"
                },
                new Link()
                {
                    Href=$"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
                    Rel="create",
                    Method="POST"
                }
            };
            return links;
        }

        private LinkResponse ReturnShapedProducts(List<Entity> shapedProduct)
        {
            return new LinkResponse() { ShapedEntities = shapedProduct };
        }

        private bool ShouldGenerateLink(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMedaType"];
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        private List<Entity>ShapeProduct(IEnumerable<ProductDTO> productDto, string fields)
        {
            return _shaper.ShapeData(productDto,fields).Select(p=>p.Entity).ToList();
        }
    }
}

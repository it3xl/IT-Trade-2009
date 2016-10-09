﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::System.Data.Objects.DataClasses.EdmSchemaAttribute()]
[assembly: global::System.Data.Objects.DataClasses.EdmRelationshipAttribute("ITTradeModel", "FK_Products_ProductGroups", "ProductGroup", global::System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(ITTrade.Models.ProductGroup), "Product", global::System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(ITTrade.Models.Product))]

// Original file name:
// Generation date: 09/10/2016 13:52:51
namespace ITTrade.Models
{
    
    /// <summary>
    /// There are no comments for ProductGroupEntities in the schema.
    /// </summary>
    public partial class ProductGroupEntities : global::System.Data.Objects.ObjectContext
    {
        /// <summary>
        /// Initializes a new ProductGroupEntities object using the connection string found in the 'ProductGroupEntities' section of the application configuration file.
        /// </summary>
        public ProductGroupEntities() : 
                base("name=ProductGroupEntities", "ProductGroupEntities")
        {
            this.OnContextCreated();
        }
        /// <summary>
        /// Initialize a new ProductGroupEntities object.
        /// </summary>
        public ProductGroupEntities(string connectionString) : 
                base(connectionString, "ProductGroupEntities")
        {
            this.OnContextCreated();
        }
        /// <summary>
        /// Initialize a new ProductGroupEntities object.
        /// </summary>
        public ProductGroupEntities(global::System.Data.EntityClient.EntityConnection connection) : 
                base(connection, "ProductGroupEntities")
        {
            this.OnContextCreated();
        }
        partial void OnContextCreated();
        /// <summary>
        /// There are no comments for Product in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public global::System.Data.Objects.ObjectQuery<Product> Product
        {
            get
            {
                if ((this._Product == null))
                {
                    this._Product = base.CreateQuery<Product>("[Product]");
                }
                return this._Product;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private global::System.Data.Objects.ObjectQuery<Product> _Product;
        /// <summary>
        /// There are no comments for ProductGroup in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public global::System.Data.Objects.ObjectQuery<ProductGroup> ProductGroup
        {
            get
            {
                if ((this._ProductGroup == null))
                {
                    this._ProductGroup = base.CreateQuery<ProductGroup>("[ProductGroup]");
                }
                return this._ProductGroup;
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private global::System.Data.Objects.ObjectQuery<ProductGroup> _ProductGroup;
        /// <summary>
        /// There are no comments for Product in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public void AddToProduct(Product product)
        {
            base.AddObject("Product", product);
        }
        /// <summary>
        /// There are no comments for ProductGroup in the schema.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public void AddToProductGroup(ProductGroup productGroup)
        {
            base.AddObject("ProductGroup", productGroup);
        }
    }
    /// <summary>
    /// There are no comments for ITTradeModel.Product in the schema.
    /// </summary>
    /// <KeyProperties>
    /// ProductID
    /// </KeyProperties>
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName="ITTradeModel", Name="Product")]
    [global::System.Runtime.Serialization.DataContractAttribute(IsReference=true)]
    [global::System.Serializable()]
    public partial class Product : global::System.Data.Objects.DataClasses.EntityObject
    {
        /// <summary>
        /// Create a new Product object.
        /// </summary>
        /// <param name="productID">Initial value of ProductID.</param>
        /// <param name="currentWholesalePrice">Initial value of CurrentWholesalePrice.</param>
        /// <param name="currentRetailPrice">Initial value of CurrentRetailPrice.</param>
        /// <param name="discountForbidden">Initial value of DiscountForbidden.</param>
        /// <param name="canDelete">Initial value of CanDelete.</param>
        /// <param name="creationDate">Initial value of CreationDate.</param>
        /// <param name="modifyDate">Initial value of ModifyDate.</param>
        /// <param name="productBarcode">Initial value of ProductBarcode.</param>
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public static Product CreateProduct(long productID, decimal currentWholesalePrice, decimal currentRetailPrice, bool discountForbidden, bool canDelete, global::System.DateTime creationDate, global::System.DateTime modifyDate, string productBarcode)
        {
            Product product = new Product();
            product.ProductID = productID;
            product.CurrentWholesalePrice = currentWholesalePrice;
            product.CurrentRetailPrice = currentRetailPrice;
            product.DiscountForbidden = discountForbidden;
            product.CanDelete = canDelete;
            product.CreationDate = creationDate;
            product.ModifyDate = modifyDate;
            product.ProductBarcode = productBarcode;
            return product;
        }
        /// <summary>
        /// There are no comments for property ProductID in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public long ProductID
        {
            get
            {
                return this._ProductID;
            }
            set
            {
                this.OnProductIDChanging(value);
                this.ReportPropertyChanging("ProductID");
                this._ProductID = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("ProductID");
                this.OnProductIDChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private long _ProductID;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnProductIDChanging(long value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnProductIDChanged();
        /// <summary>
        /// There are no comments for property Article in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public string Article
        {
            get
            {
                return this._Article;
            }
            set
            {
                this.OnArticleChanging(value);
                this.ReportPropertyChanging("Article");
                this._Article = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, true);
                this.ReportPropertyChanged("Article");
                this.OnArticleChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private string _Article;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnArticleChanging(string value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnArticleChanged();
        /// <summary>
        /// There are no comments for property CurrentWholesalePrice in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public decimal CurrentWholesalePrice
        {
            get
            {
                return this._CurrentWholesalePrice;
            }
            set
            {
                this.OnCurrentWholesalePriceChanging(value);
                this.ReportPropertyChanging("CurrentWholesalePrice");
                this._CurrentWholesalePrice = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("CurrentWholesalePrice");
                this.OnCurrentWholesalePriceChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private decimal _CurrentWholesalePrice;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCurrentWholesalePriceChanging(decimal value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCurrentWholesalePriceChanged();
        /// <summary>
        /// There are no comments for property CurrentRetailPrice in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public decimal CurrentRetailPrice
        {
            get
            {
                return this._CurrentRetailPrice;
            }
            set
            {
                this.OnCurrentRetailPriceChanging(value);
                this.ReportPropertyChanging("CurrentRetailPrice");
                this._CurrentRetailPrice = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("CurrentRetailPrice");
                this.OnCurrentRetailPriceChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private decimal _CurrentRetailPrice;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCurrentRetailPriceChanging(decimal value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCurrentRetailPriceChanged();
        /// <summary>
        /// There are no comments for property DiscountForbidden in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public bool DiscountForbidden
        {
            get
            {
                return this._DiscountForbidden;
            }
            set
            {
                this.OnDiscountForbiddenChanging(value);
                this.ReportPropertyChanging("DiscountForbidden");
                this._DiscountForbidden = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("DiscountForbidden");
                this.OnDiscountForbiddenChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private bool _DiscountForbidden;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnDiscountForbiddenChanging(bool value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnDiscountForbiddenChanged();
        /// <summary>
        /// There are no comments for property CanDelete in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public bool CanDelete
        {
            get
            {
                return this._CanDelete;
            }
            set
            {
                this.OnCanDeleteChanging(value);
                this.ReportPropertyChanging("CanDelete");
                this._CanDelete = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("CanDelete");
                this.OnCanDeleteChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private bool _CanDelete;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCanDeleteChanging(bool value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCanDeleteChanged();
        /// <summary>
        /// There are no comments for property CreationDate in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public global::System.DateTime CreationDate
        {
            get
            {
                return this._CreationDate;
            }
            set
            {
                this.OnCreationDateChanging(value);
                this.ReportPropertyChanging("CreationDate");
                this._CreationDate = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("CreationDate");
                this.OnCreationDateChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private global::System.DateTime _CreationDate;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCreationDateChanging(global::System.DateTime value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnCreationDateChanged();
        /// <summary>
        /// There are no comments for property ModifyDate in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public global::System.DateTime ModifyDate
        {
            get
            {
                return this._ModifyDate;
            }
            set
            {
                this.OnModifyDateChanging(value);
                this.ReportPropertyChanging("ModifyDate");
                this._ModifyDate = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("ModifyDate");
                this.OnModifyDateChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private global::System.DateTime _ModifyDate;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnModifyDateChanging(global::System.DateTime value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnModifyDateChanged();
        /// <summary>
        /// There are no comments for property ProductBarcode in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public string ProductBarcode
        {
            get
            {
                return this._ProductBarcode;
            }
            set
            {
                this.OnProductBarcodeChanging(value);
                this.ReportPropertyChanging("ProductBarcode");
                this._ProductBarcode = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false);
                this.ReportPropertyChanged("ProductBarcode");
                this.OnProductBarcodeChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private string _ProductBarcode;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnProductBarcodeChanging(string value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnProductBarcodeChanged();
        /// <summary>
        /// There are no comments for ProductGroup in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute("ITTradeModel", "FK_Products_ProductGroups", "ProductGroup")]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        [global::System.Xml.Serialization.XmlIgnoreAttribute()]
        [global::System.Xml.Serialization.SoapIgnoreAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public ProductGroup ProductGroup
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<ProductGroup>("ITTradeModel.FK_Products_ProductGroups", "ProductGroup").Value;
            }
            set
            {
                ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<ProductGroup>("ITTradeModel.FK_Products_ProductGroups", "ProductGroup").Value = value;
            }
        }
        /// <summary>
        /// There are no comments for ProductGroup in the schema.
        /// </summary>
        [global::System.ComponentModel.BrowsableAttribute(false)]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public global::System.Data.Objects.DataClasses.EntityReference<ProductGroup> ProductGroupReference
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<ProductGroup>("ITTradeModel.FK_Products_ProductGroups", "ProductGroup");
            }
            set
            {
                if ((value != null))
                {
                    ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.InitializeRelatedReference<ProductGroup>("ITTradeModel.FK_Products_ProductGroups", "ProductGroup", value);
                }
            }
        }
    }
    /// <summary>
    /// There are no comments for ITTradeModel.ProductGroup in the schema.
    /// </summary>
    /// <KeyProperties>
    /// ProductGroupID
    /// </KeyProperties>
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName="ITTradeModel", Name="ProductGroup")]
    [global::System.Runtime.Serialization.DataContractAttribute(IsReference=true)]
    [global::System.Serializable()]
    public partial class ProductGroup : global::System.Data.Objects.DataClasses.EntityObject
    {
        /// <summary>
        /// Create a new ProductGroup object.
        /// </summary>
        /// <param name="productGroupID">Initial value of ProductGroupID.</param>
        /// <param name="name">Initial value of Name.</param>
        /// <param name="dateCreation">Initial value of DateCreation.</param>
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public static ProductGroup CreateProductGroup(int productGroupID, string name, global::System.DateTime dateCreation)
        {
            ProductGroup productGroup = new ProductGroup();
            productGroup.ProductGroupID = productGroupID;
            productGroup.Name = name;
            productGroup.DateCreation = dateCreation;
            return productGroup;
        }
        /// <summary>
        /// There are no comments for property ProductGroupID in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public int ProductGroupID
        {
            get
            {
                return this._ProductGroupID;
            }
            set
            {
                this.OnProductGroupIDChanging(value);
                this.ReportPropertyChanging("ProductGroupID");
                this._ProductGroupID = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("ProductGroupID");
                this.OnProductGroupIDChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private int _ProductGroupID;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnProductGroupIDChanging(int value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnProductGroupIDChanged();
        /// <summary>
        /// There are no comments for property Name in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this.OnNameChanging(value);
                this.ReportPropertyChanging("Name");
                this._Name = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false);
                this.ReportPropertyChanged("Name");
                this.OnNameChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private string _Name;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnNameChanging(string value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnNameChanged();
        /// <summary>
        /// There are no comments for property DateCreation in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        public global::System.DateTime DateCreation
        {
            get
            {
                return this._DateCreation;
            }
            set
            {
                this.OnDateCreationChanging(value);
                this.ReportPropertyChanging("DateCreation");
                this._DateCreation = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("DateCreation");
                this.OnDateCreationChanged();
            }
        }
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        private global::System.DateTime _DateCreation;
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnDateCreationChanging(global::System.DateTime value);
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        partial void OnDateCreationChanged();
        /// <summary>
        /// There are no comments for Product in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute("ITTradeModel", "FK_Products_ProductGroups", "Product")]
        [global::System.CodeDom.Compiler.GeneratedCode("System.Data.Entity.Design.EntityClassGenerator", "4.0.0.0")]
        [global::System.Xml.Serialization.XmlIgnoreAttribute()]
        [global::System.Xml.Serialization.SoapIgnoreAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public global::System.Data.Objects.DataClasses.EntityCollection<Product> Product
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedCollection<Product>("ITTradeModel.FK_Products_ProductGroups", "Product");
            }
            set
            {
                if ((value != null))
                {
                    ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.InitializeRelatedCollection<Product>("ITTradeModel.FK_Products_ProductGroups", "Product", value);
                }
            }
        }
    }
}

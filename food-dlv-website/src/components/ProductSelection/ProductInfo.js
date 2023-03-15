const ProductInfo = ({ product }) => {
  // const productImg = require(`../../assets/images/public/Products/${data.products.photo}`);

  return (
    <div>
      <h1>{product.productName}</h1>
      <p>{product.productContent}</p>
      <img src={product.photo} alt={product.productName} />
      <p>Price:{product.unitPrice}NTD</p>
    </div>
  );
};

export default ProductInfo;
